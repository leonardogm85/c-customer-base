using CustomerBase.ApplicationService;
using CustomerBase.Framework;
using CustomerBase.Model;
using CustomerBase.Model.Db;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace CustomerBase.ViewModel
{
    // ViewModel de Cliente / Endereço.
    public class CustomerViewModel : Bindable
    {
        // DAO para acesso a dados.
        private CustomerDao dao;

        public IWindowServices WindowServices { get; set; }

        // Lista de clientes.
        private List<Customer> customers;
        public List<Customer> Customers
        {
            get { return customers; }
            set
            {
                SetValue(ref customers, value);
            }
        }

        // Cliente sendo inserido ou alterado.
        private Customer customer;
        public Customer Customer
        {
            get { return customer; }
            set
            {
                SetValue(ref customer, value);
            }
        }

        // Índice que corresponde ao cliente selecionado na lista de clientes.
        private int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                SetValue(ref selectedIndex, value);

                if (selectedIndex >= 0)
                {
                    // Atribui à propriedade Cliente o objeto cliente selecionado.
                    Customer = Customers[selectedIndex];
                }

                // Ajusta o estado dos comandos.
                NewCustomerCommand.CanExecute = IsViewing;
                DeleteCustomerCommand.CanExecute = IsViewing && selectedIndex >= 0;
                UpdateCustomerCommand.CanExecute = IsViewing && selectedIndex >= 0;
            }
        }

        // Indica se o cliente está em modo de edição.
        private bool isEditing;
        public bool IsEditing
        {
            get { return isEditing; }
            set
            {
                SetValue(ref isEditing, value);

                // Ajusta o estado dos comandos.
                NewCustomerCommand.CanExecute = IsViewing;
                DeleteCustomerCommand.CanExecute = IsViewing && SelectedIndex >= 0;
                UpdateCustomerCommand.CanExecute = IsViewing && SelectedIndex >= 0;
                CancelCustomerCommand.CanExecute = isEditing;
                SaveCustomerCommand.CanExecute = isEditing;

                // Notifica a propriedade IsViewing para ser reavaliada.
                OnPropertyChanged("IsViewing");
            }
        }

        // Parte do nome para pesquisar.
        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set
            {
                SetValue(ref searchText, value);
            }
        }

        // Indica se o cliente está em modo de visualização.
        public bool IsViewing
        {
            get { return !isEditing; }
        }

        // Comandos.
        public Command NewCustomerCommand { get; set; }
        public Command DeleteCustomerCommand { get; set; }
        public Command UpdateCustomerCommand { get; set; }
        public Command SaveCustomerCommand { get; set; }
        public Command CancelCustomerCommand { get; set; }
        public Command ExitCommand { get; set; }
        public Command SearchCommand { get; set; }

        // Construtor.
        public CustomerViewModel()
        {
            try
            {
                NewCustomerCommand = new Command(NewCustomer);
                DeleteCustomerCommand = new Command(DeleteCustomer);
                UpdateCustomerCommand = new Command(UpdateCustomer);
                SaveCustomerCommand = new Command(SaveCustomer);
                CancelCustomerCommand = new Command(CancelCustomer);
                ExitCommand = new Command(Exit);
                SearchCommand = new Command(Search);

                // Cria o DAO.
                dao = DaoFactory.CreateDao<CustomerDao>();

                IsEditing = false;
                SelectedIndex = -1;

                // Obtém a lista inicial de clientes.
                Customers = dao.ListCustomers(null);
            }
            catch (Exception exception)
            {
                // Este catch é necessário para evitar erro no processo de criação do XAML.
                Debug.WriteLine("Error: {0}", exception.Message);
            }
        }

        // Verifica se é preciso salvar os dados antes de sair.
        public void ProcessOutput()
        {
            if (IsEditing && SaveCustomerCommand.CanExecute)
            {
                // Se está em modo de edição e a gravação está habilitada, pergunta se os dados serão salvos.
                bool confirm = true;

                if (WindowServices != null)
                {
                    confirm = WindowServices.ConfirmSave();
                }

                if (confirm)
                {
                    // Grava os dados do cliente.
                    SaveCustomer();
                }
            }
        }

        // Um novo cliente está sendo adicionado.
        private void NewCustomer()
        {
            // Inicia edição.
            IsEditing = true;

            // Não seleciona nenhum cliente da lista.
            SelectedIndex = -1;

            // Cria um novo cliente.
            Customer = new Customer();

            // Registra o interesse nas mudanças dos erros de validação.
            Customer.ErrorsChanged += OnErrorsChanged;
            Customer.Address.ErrorsChanged += OnErrorsChanged;

            if (WindowServices != null)
            {
                // Atualiza os bindings.
                WindowServices.UpdateBindings();

                // Coloca o foco no formulário.
                WindowServices.PutFocusOnForm();
            }
        }

        // Exclui um cliente.
        private void DeleteCustomer()
        {
            bool confirm = true;

            if (WindowServices != null)
            {
                // Pede confirmação.
                confirm = WindowServices.ConfirmDelete();
            }

            if (confirm)
            {
                // Exclui do banco de dados.
                dao.Delete(customer.Id.Value);

                // Atualiza a lista de clientes.
                Customers = dao.ListCustomers(SearchText);

                // Cria um novo cliente para limpar os dados do formulário.
                Customer = new Customer();
            }
        }

        // Edita um cliente existente.
        private void UpdateCustomer()
        {
            // Entra em modo de edição.
            IsEditing = true;

            if (WindowServices != null)
            {
                // Coloca o foco no formulário.
                WindowServices.PutFocusOnForm();
            }
        }

        // Grava as alterações de um cliente.
        private void SaveCustomer()
        {
            if (Customer.Id == null)
            {
                // Se o cliente não tiver um Id, é uma inserção.
                dao.Insert(Customer);
            }
            else
            {
                // Se o cliente tiver um Id, é uma alteração.
                dao.Update(Customer);
            }

            // Entra em modo de visualização.
            IsEditing = false;

            // Atualiza a lista de clientes.
            Customers = dao.ListCustomers(SearchText);

            // Cria um novo cliente para limpar os dados do formulário.
            Customer = new Customer();
        }

        // Cancela a edição de um cliente.
        private void CancelCustomer()
        {
            // Entra em modo de visualização.
            IsEditing = false;

            if (Customer.Id == null)
            {
                // Remove o interesse na mudança dos erros de validação.
                Customer.ErrorsChanged -= OnErrorsChanged;
                Customer.Address.ErrorsChanged -= OnErrorsChanged;

                // Se o cliente não tem Id, é porque não existe no banco de dados. Então basta descartar os dados.
                Customer = new Customer();
            }
            else
            {
                // Se o cliente tem Id, é porque ele existe no banco de dados. Então lê seus dados do banco de dados novamente.
                dao.ReadCustomer(Customer.Id.Value, Customer);

                // Remove o interesse na mudança dos erros de validação.
                Customer.ErrorsChanged -= OnErrorsChanged;
                Customer.Address.ErrorsChanged -= OnErrorsChanged;
            }
        }

        // Inicia a saída da aplicação.
        private void Exit()
        {
            // Faz o processamento final, gravando os dados se necessário.
            ProcessOutput();

            if (WindowServices != null)
            {
                // Fecha a janela.
                WindowServices.CloseWindow();
            }
        }

        // Pesquisa clientes de acordo com o texto fornecido.
        private void Search()
        {
            // Atualiza a lista de acordo com o texto fornecido.
            Customers = dao.ListCustomers(SearchText);
        }

        private void OnErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
        {
            // Se os erros de validação mudarem, verifica se a gravação é ativada ou não.
            SaveCustomerCommand.CanExecute = !Customer.HasErrors && !Customer.Address.HasErrors;
        }
    }
}

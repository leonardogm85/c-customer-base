using CustomerBase.ApplicationService;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CustomerBase.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IWindowServices
    {
        // Indica se a janela deve ser fechada.
        private bool closeWindow;

        public MainWindow()
        {
            InitializeComponent();

            // Faz com que o ViewModel referencie a janela como um IWindowServices.
            viewModel.WindowServices = this;
        }

        public void UpdateBindings()
        {
            // Atualiza os bindings do formulário manualmente.
            // Isto é necessário para que os erros de validação já apareçam assim que o formulário é ativado.
            txtName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtEmail.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtPhoneNumber.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtStreet.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtNumber.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtComplement.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtDistrict.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtZipCode.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        public void PutFocusOnForm()
        {
            // Coloca o foco no campo de nome do formulário.
            txtName.Focus();
        }

        public bool ConfirmDelete()
        {
            // Exibe um dialog perguntando se realmente é para excluir o cliente.
            MessageBoxResult result = MessageBox.Show(
                this,
                "Do you want to delete the customer?",
                "Delete?",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        public bool ConfirmSave()
        {
            // Exibe um dialog perguntando se é preciso salvar.
            MessageBoxResult result = MessageBox.Show(
                this,
                "Do you want to save the customer changes?",
                "Save changes?",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        public void CloseWindow()
        {
            // Fecha a janela.
            closeWindow = true;
            Close();
        }

        private void HandleSpecialChars(object sender, TextCompositionEventArgs e)
        {
            // Trata a digitação de caracteres especiais em alguns campos do formulário.

            // Converte o objeto que gerou o evento para um TextBox.
            TextBox textBox = sender as TextBox;

            if (e.Text.Length > 0)
            {
                bool allowed = false;

                if (textBox == txtNumber)
                {
                    // Permite apenas números no campo "número".
                    if (char.IsDigit(e.Text, e.Text.Length - 1))
                    {
                        allowed = true;
                    }
                }
                else if (textBox == txtZipCode)
                {
                    // Permite apenas números e um "-" no campo "CEP".
                    char c = e.Text[e.Text.Length - 1];

                    if (char.IsDigit(c))
                    {
                        allowed = true;
                    }
                    else if (c == '-' && !txtZipCode.Text.Contains("-"))
                    {
                        allowed = true;
                    }
                }
                else if (textBox == txtPhoneNumber)
                {
                    // Permite apenas números, "-", "(" e ")" no campo "telefone".
                    char c = e.Text[e.Text.Length - 1];

                    if (char.IsDigit(c))
                    {
                        allowed = true;
                    }
                    else if (c == '-' && !txtPhoneNumber.Text.Contains("-"))
                    {
                        allowed = true;
                    }
                    else if (c == '(' && !txtPhoneNumber.Text.Contains("("))
                    {
                        allowed = true;
                    }
                    else if (c == ')' && !txtPhoneNumber.Text.Contains(")"))
                    {
                        allowed = true;
                    }
                }

                // Indica se o evento foi tratado ou não. Se o evento for tratado, o caractere não aparece no campo do formulário.
                e.Handled = !allowed;
            }
        }

        private void HandleSpaceChar(object sender, KeyEventArgs e)
        {
            // Não permite espaços em brancos nos campos "telefone", "CEP" e "número".
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!closeWindow)
            {
                // Se a janela está fechando, primeiro dá a chance do ViewModel de fazer as últimas checagens.
                viewModel.ProcessOutput();
            }
        }
    }
}

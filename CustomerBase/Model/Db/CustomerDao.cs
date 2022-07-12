using CustomerBase.Framework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace CustomerBase.Model.Db
{
    // Métodos de acesso a dados para cliente / endereço.
    public class CustomerDao : Dao
    {
        // Obtém a lista de clientes / endereços com base em um padrão de nome.
        public List<Customer> ListCustomers(string pattern)
        {
            using (DbConnection connection = CreateConnection())
            {
                StringBuilder commandText = new StringBuilder();

                commandText.Append("SELECT Id, Name, Email, PhoneNumber, Street, Number, Complement, District, ZipCode ");
                commandText.Append("FROM Customers AS c ");
                commandText.Append("INNER JOIN Addresses AS a ON Id = CustomerId ");

                if (!string.IsNullOrWhiteSpace(pattern))
                {
                    commandText.Append("WHERE Name LIKE @Pattern ");
                }

                commandText.Append("ORDER BY Name");

                using (DbCommand command = CreateCommand(connection, commandText.ToString()))
                {
                    if (!string.IsNullOrWhiteSpace(pattern))
                    {
                        AddParameter(command, "@Pattern", $"%{pattern}%");
                    }

                    List<Customer> customers = new List<Customer>();

                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Customer customer = new Customer();
                            SetData(reader, customer);
                            customers.Add(customer);
                        }

                        return customers;
                    }
                }
            }
        }

        // Obtém um cliente / endereço com base no seu Id. Os dados do cliente são adicionados no objeto passado como parâmetro.
        public void ReadCustomer(int id, Customer customer)
        {
            using (DbConnection connection = CreateConnection())
            {
                StringBuilder commandText = new StringBuilder();

                commandText.Append("SELECT Id, Name, Email, PhoneNumber, Street, Number, Complement, District, ZipCode ");
                commandText.Append("FROM Customers AS c ");
                commandText.Append("INNER JOIN Addresses AS a ON Id = CustomerId ");
                commandText.Append("WHERE Id = @Id");

                using (DbCommand command = CreateCommand(connection, commandText.ToString()))
                {
                    AddParameter(command, "@Id", id);

                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            SetData(reader, customer);
                        }
                    }
                }
            }
        }

        // Insere um novo cliente / endereço no banco de dados.
        public void Insert(Customer customer)
        {
            using (DbConnection connection = CreateConnection())
            {
                // Usa uma transação para fazer as duas inserções de forma atômica.
                DbTransaction transaction = connection.BeginTransaction();

                string commandText = "INSERT INTO Customers (Id, Name, Email, PhoneNumber) VALUES (@Id, @Name, @Email, @PhoneNumber)";

                using (DbCommand command = CreateCommand(connection, commandText))
                {
                    command.Transaction = transaction;

                    customer.Id = GetLastId() + 1;

                    AddParameter(command, "@Id", customer.Id);
                    AddParameter(command, "@Name", customer.Name);
                    AddParameter(command, "@Email", customer.Email);
                    AddParameter(command, "@PhoneNumber", customer.PhoneNumber);

                    command.ExecuteNonQuery();
                }

                commandText = "INSERT INTO Addresses (CustomerId, Street, Number, Complement, District, ZipCode) ";
                commandText += "VALUES (@Id, @Street, @Number, @Complement, @District, @ZipCode)";

                using (DbCommand command = CreateCommand(connection, commandText))
                {
                    command.Transaction = transaction;

                    customer.Address.Customer = customer;

                    AddParameter(command, "@Id", customer.Id);
                    AddParameter(command, "@Street", customer.Address.Street);
                    AddParameter(command, "@Number", customer.Address.Number);
                    AddParameter(command, "@Complement", customer.Address.Complement);
                    AddParameter(command, "@District", customer.Address.District);
                    AddParameter(command, "@ZipCode", customer.Address.ZipCode);

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }

        // Alterar os dados de um cliente / endereço.
        public void Update(Customer customer)
        {
            using (DbConnection connection = CreateConnection())
            {
                // Usa uma transação para fazer as duas alterações de forma atômica.
                DbTransaction transaction = connection.BeginTransaction();

                string commandText = "UPDATE Customers SET Name = @Name, Email = @Email, PhoneNumber = @PhoneNumber WHERE Id = @Id";

                using (DbCommand command = CreateCommand(connection, commandText))
                {
                    command.Transaction = transaction;

                    AddParameter(command, "@Id", customer.Id);
                    AddParameter(command, "@Name", customer.Name);
                    AddParameter(command, "@Email", customer.Email);
                    AddParameter(command, "@PhoneNumber", customer.PhoneNumber);

                    command.ExecuteNonQuery();
                }

                commandText = "UPDATE Addresses SET ";
                commandText += "Street = @Street, Number = @Number, Complement = @Complement, District = @District, ZipCode = @ZipCode ";
                commandText += "WHERE CustomerId = @Id";

                using (DbCommand command = CreateCommand(connection, commandText))
                {
                    command.Transaction = transaction;

                    AddParameter(command, "@Id", customer.Id);
                    AddParameter(command, "@Street", customer.Address.Street);
                    AddParameter(command, "@Number", customer.Address.Number);
                    AddParameter(command, "@Complement", customer.Address.Complement);
                    AddParameter(command, "@District", customer.Address.District);
                    AddParameter(command, "@ZipCode", customer.Address.ZipCode);

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }

        public void Delete(int id)
        {
            using (DbConnection connection = CreateConnection())
            {
                // Usa uma transação para fazer as duas exclusões de forma atômica.
                DbTransaction transaction = connection.BeginTransaction();

                // Primeiro exclui o endereço devido a restrição de chave estrangeira na tabela.
                string commandText = "DELETE FROM Addresses WHERE CustomerId = @Id";

                using (DbCommand command = CreateCommand(connection, commandText))
                {
                    command.Transaction = transaction;
                    AddParameter(command, "@Id", id);
                    command.ExecuteNonQuery();
                }

                commandText = "DELETE FROM Customers WHERE Id = @Id";

                using (DbCommand command = CreateCommand(connection, commandText))
                {
                    command.Transaction = transaction;
                    AddParameter(command, "@Id", id);
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }

        // Adiciona um paramêtro à query.
        private void AddParameter(DbCommand command, string name, object? value)
        {
            if (value == null)
            {
                // Se o valor for nulo, atribui DbNull
                value = DBNull.Value;
            }

            CreateParameter(command, name, value);
        }

        // Obtém o maior Id cadastrado para um cliente.
        private int GetLastId()
        {
            using (DbConnection connection = CreateConnection())
            {
                string commandText = "SELECT MAX(Id) FROM Customers";

                using (DbCommand command = CreateCommand(connection, commandText))
                {
                    object? @object = command.ExecuteScalar();

                    if (@object == DBNull.Value)
                    {
                        return 0;
                    }

                    return (int)@object!;
                }
            }
        }

        // Extrair dados de um DbDataReader e os coloca no objeto Cliente / Endereço fornecido.
        private void SetData(DbDataReader reader, Customer customer)
        {
            customer.Id = (int)reader["Id"];
            customer.Name = (string)reader["Name"];
            customer.Email = reader["Email"] == DBNull.Value
                ? null
                : (string)reader["Email"];
            customer.PhoneNumber = reader["PhoneNumber"] == DBNull.Value
                ? null
                : (string)reader["PhoneNumber"];
            customer.Address = new Address
            {
                Street = (string)reader["Street"],
                Number = (int?)reader["Number"],
                Complement = reader["Complement"] == DBNull.Value
                    ? null
                    : (string)reader["Complement"],
                District = reader["District"] == DBNull.Value
                    ? null
                    : (string)reader["District"],
                ZipCode = (string)reader["ZipCode"]
            };
        }
    }
}

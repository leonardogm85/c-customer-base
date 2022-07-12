using Microsoft.Extensions.Configuration;
using System.Data.Common;

namespace CustomerBase.Framework
{
    public abstract class Dao
    {
        protected static DbProviderFactory ProviderFactory;
        protected static string ConnectionString;

        static Dao()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = configuration
                .GetConnectionString("DefaultConnection");
            string providerInvariantName = configuration
                .GetValue<string>("AppSettings:ProviderInvariantName");
            string factoryTypeAssemblyQualifiedName = configuration
                .GetValue<string>("AppSettings:FactoryTypeAssemblyQualifiedName");

            DbProviderFactories.RegisterFactory(
                providerInvariantName,
                factoryTypeAssemblyQualifiedName);

            ConnectionString = connectionString;
            ProviderFactory = DbProviderFactories.GetFactory(providerInvariantName);
        }

        protected DbConnection CreateConnection()
        {
            DbConnection connection = ProviderFactory.CreateConnection()!;
            connection.ConnectionString = ConnectionString;
            connection.Open();
            return connection;
        }

        protected DbCommand CreateCommand(DbConnection connection, string commandText)
        {
            DbCommand command = ProviderFactory.CreateCommand()!;
            command.Connection = connection;
            command.CommandText = commandText;
            return command;
        }

        protected DbParameter CreateParameter(DbCommand command, string name, object value)
        {
            DbParameter parameter = ProviderFactory.CreateParameter()!;
            parameter.ParameterName = name;
            parameter.Value = value;
            command.Parameters.Add(parameter);
            return parameter;
        }
    }
}

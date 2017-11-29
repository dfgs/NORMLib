using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace NORMLib
{
    public class Server:IServer
    {

		private IConnectionFactory connectionFactory;
		private ICommandFactory commandFactory;

		public string name;
		public string Name
		{
			get { return name; }
		}

		public Server(IConnectionFactory ConnectionFactory,ICommandFactory CommandFactory)
		{
			this.connectionFactory = ConnectionFactory;
			this.commandFactory = CommandFactory;
		}

		public void Dispose()
		{

		}

		public void Execute(params IQuery[] Queries)
		{
			throw new NotImplementedException();
		}


		public int Execute<RowType>(IInsert<RowType> Query)
		{
			DbCommand command;

			using (DbConnection connection = connectionFactory.CreateConnectionToDatabase())
			{
				connection.Open();
				command = commandFactory.CreateCommand(Query);
				command.Connection = connection;
				command.ExecuteNonQuery();

				command = commandFactory.CreateIdentityCommand<RowType>();
				command.Connection = connection;
				return command.ExecuteNonQuery();
			}
		}

		public int Execute<RowType>(IUpdate<RowType> Query)
		{
			DbCommand command;

			using (DbConnection connection = connectionFactory.CreateConnectionToDatabase())
			{
				connection.Open();
				command = commandFactory.CreateCommand(Query);
				command.Connection = connection;
				return command.ExecuteNonQuery();
			}
		}

		public int Execute<RowType>(IDelete<RowType> Query)
		{
			DbCommand command;

			using (DbConnection connection = connectionFactory.CreateConnectionToDatabase())
			{
				connection.Open();
				command = commandFactory.CreateCommand(Query);
				command.Connection = connection;
				return command.ExecuteNonQuery();
			}
		}

		public List<RowType> Execute<RowType>(ISelect<RowType> Query)
			where RowType : new()
		{
			DbCommand command;
			DbDataReader reader;
			RowType item;
			object value;
			List<RowType> results;

			results = new List<RowType>();

			using (DbConnection connection = connectionFactory.CreateConnectionToDatabase())
			{
				connection.Open();
				command = commandFactory.CreateCommand(Query);
				command.Connection = connection;
				reader = command.ExecuteReader();
				while (reader.Read())
				{
					item = new RowType();
					foreach (IColumn column in Query.Columns)
					{
						value = commandFactory.ConvertFromDbValue(column, reader[column.Name]);
						column.SetValue(item, value);
					}
					results.Add(item);
				}
				reader.Close();
			}

			return results;
		}

		public bool Execute(IDatabaseExists Query)
		{
			DbCommand command;
			DbDataReader reader;
			bool result;


			using (DbConnection connection = connectionFactory.CreateConnectionToDatabase())
			{
				connection.Open();
				command = commandFactory.CreateCommand(Query);
				command.Connection = connection;
				reader = command.ExecuteReader();
				result = reader.HasRows;
				reader.Close();
			}

			return result;
		}

		public int Execute(ICreateDatabase Query)
		{
			DbCommand command;

			using (DbConnection connection = connectionFactory.CreateConnectionToDatabase())
			{
				connection.Open();
				command = commandFactory.CreateCommand(Query);
				command.Connection = connection;
				return command.ExecuteNonQuery();
			}
		}

		public bool Execute<RowType>(ITableExists<RowType> Query)
		{
			DbCommand command;
			DbDataReader reader;
			bool result;


			using (DbConnection connection = connectionFactory.CreateConnectionToDatabase())
			{
				connection.Open();
				command = commandFactory.CreateCommand(Query);
				command.Connection = connection;
				reader = command.ExecuteReader();
				result = reader.HasRows;
				reader.Close();
			}

			return result;
		}

		public int Execute<RowType>(ICreateTable<RowType> Query)
		{
			DbCommand command;

			using (DbConnection connection = connectionFactory.CreateConnectionToDatabase())
			{
				connection.Open();
				command = commandFactory.CreateCommand(Query);
				command.Connection = connection;
				return command.ExecuteNonQuery();
			}
		}

		public int Execute<RowType>(ICreateColumn<RowType> Query)
		{
			DbCommand command;

			using (DbConnection connection = connectionFactory.CreateConnectionToDatabase())
			{
				connection.Open();
				command = commandFactory.CreateCommand(Query);
				command.Connection = connection;
				return command.ExecuteNonQuery();
			}
		}

		public int Execute<PrimaryRowType, ForeignRowType>(ICreateRelation<PrimaryRowType, ForeignRowType> Query)
		{
			DbCommand command;

			using (DbConnection connection = connectionFactory.CreateConnectionToDatabase())
			{
				connection.Open();
				command = commandFactory.CreateCommand(Query);
				command.Connection = connection;
				return command.ExecuteNonQuery();
			}
		}

	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace NORMLib
{
    public class Database:IDatabase
    {

		private IConnectionFactory connectionFactory;
		private ICommandFactory commandFactory;


		public Database(IConnectionFactory ConnectionFactory,ICommandFactory CommandFactory)
		{
			this.connectionFactory = ConnectionFactory;
			this.commandFactory = CommandFactory;
		}
		public void Dispose()
		{

		}
				

		public void Insert<RowType>(RowType Row)
		{
			DbCommand command;
			object result,key;

			using (DbConnection connection = connectionFactory.CreateConnectionToDatabase())
			{
				connection.Open();
				command = commandFactory.CreateInsertCommand(Row,Table<RowType>.Columns.Where(item=>!item.IsIdentity));
				command.Connection = connection;
				command.ExecuteNonQuery();

				command = commandFactory.CreateIdentityCommand<RowType>();
				command.Connection = connection;
				result = command.ExecuteScalar();
			}

			key = Convert.ToInt32(result);
			Table<RowType>.IdentityColumn.SetValue(Row,key);
		}

		public void Update<RowType>(RowType Row)
		{
			DbCommand command;

			using (DbConnection connection = connectionFactory.CreateConnectionToDatabase())
			{
				connection.Open();
				command = commandFactory.CreateUpdateCommand(Row,Table<RowType>.Columns.Where(item=>(!item.IsPrimaryKey) && (!item.IsIdentity)));
				command.Connection = connection;
				command.ExecuteNonQuery();
			}
		}

		public void Delete<RowType>(RowType Row)
		{
			DbCommand command;

			using (DbConnection connection = connectionFactory.CreateConnectionToDatabase())
			{
				connection.Open();
				command = commandFactory.CreateDeleteCommand(Row);
				command.Connection = connection;
				command.ExecuteNonQuery();
			}
		}

		public List<RowType> Select<RowType>(Filter Filter = null)
			where RowType:new()
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
				command = commandFactory.CreateSelectCommand<RowType>(Table<RowType>.Columns, Filter);
				command.Connection = connection;
				reader = command.ExecuteReader();
				while (reader.Read())
				{
					item = new RowType();
					foreach (IColumn column in Table<RowType>.Columns)
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



	}
}

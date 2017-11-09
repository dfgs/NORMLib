using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace NORMLib
{
    public class Session:ISession
    {

		private IDbConnection connection;
		private ICommandFactory commandFactory;

			
		public Session(IDbConnection Connection,ICommandFactory CommandFactory)
		{
			this.connection = Connection;
			this.commandFactory = CommandFactory;
			this.connection.Open();
		}
		public void Dispose()
		{

			this.connection.Close();
		}
				

		public void Insert<RowType>(RowType Item)
		{
			DbCommand command;
			object result,key;

			command = commandFactory.CreateInsertCommand(Item);
			command.Connection = (DbConnection)connection;
			command.ExecuteNonQuery();

			command = commandFactory.CreateIdentityCommand(Item);
			command.Connection = (DbConnection)connection;
			result=command.ExecuteScalar() ;
			key = Convert.ChangeType(result, Schema<RowType>.IdentityColumn.ColumnType);
			Schema<RowType>.IdentityColumn.SetValue(Item,key);
		}

		public void Update<RowType>(RowType Item)
		{
			DbCommand command;

			command = commandFactory.CreateUpdateCommand(Item);
			command.Connection = (DbConnection)connection;
			command.ExecuteNonQuery();
		}

		public void Delete<RowType>(RowType Item)
		{
			DbCommand command;

			command = commandFactory.CreateDeleteCommand(Item);
			command.Connection = (DbConnection)connection;
			command.ExecuteNonQuery();
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
			command = commandFactory.CreateSelectCommand<RowType>(Filter);
			command.Connection = (DbConnection)connection;
			reader=command.ExecuteReader();
			while(reader.Read())
			{
				item = new RowType();
				foreach(IColumn column in Schema<RowType>.Columns)
				{
					value = commandFactory.ConvertFromDbValue(column, reader[column.Name]);
					column.SetValue(item, value);
				}
				results.Add(item);
			}
			return results;
		}

	}
}

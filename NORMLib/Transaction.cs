using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace NORMLib
{
    public class Transaction:ITransaction
    {
		private IDbTransaction transaction;
		private IDbConnection connection;
		private ICommandFactory commandFactory;

			
		public Transaction(IDbConnection Connection,ICommandFactory CommandFactory)
		{
			this.connection = Connection;
			this.commandFactory = CommandFactory;
			this.connection.Open();
			this.transaction = connection.BeginTransaction();
		}
		public void Dispose()
		{
			if (transaction != null) transaction.Commit();
			this.connection.Close();
		}
				

		public void Insert<RowType>(RowType Item)
		{
			DbCommand command;
			object key;

			try
			{
				command = commandFactory.CreateInsertCommand(Item);
				command.Connection = (DbConnection)connection;
				command.Transaction = (DbTransaction)transaction;
				command.ExecuteNonQuery();

				command = commandFactory.CreateIdentityCommand(Item);
				command.Connection = (DbConnection)connection;
				key = command.ExecuteScalar();
				Schema<RowType>.IdentityColumn.SetValue(Item, key);
			}
			catch (Exception ex)
			{
				transaction.Rollback();
				transaction = null;
				throw (ex);
			}

		}

		public void Update<RowType>(RowType Item)
		{
			DbCommand command;

			command = commandFactory.CreateUpdateCommand(Item);
			command.Connection = (DbConnection)connection;
			command.Transaction = (DbTransaction)transaction;
			try
			{
				command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				transaction.Rollback();
				transaction = null;
				throw (ex);
			}
		}

		public void Delete<RowType>(RowType Item)
		{
			DbCommand command;

			command = commandFactory.CreateDeleteCommand(Item);
			command.Connection = (DbConnection)connection;
			command.Transaction = (DbTransaction)transaction;
			try
			{
				command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				transaction.Rollback();
				transaction = null;
				throw (ex);
			}
		}


		public List<RowType> Select<RowType>(Filter Filter = null)
			where RowType : new()
		{
			DbCommand command;
			DbDataReader reader;
			RowType item;
			object value;
			List<RowType> items;

			command = commandFactory.CreateSelectCommand<RowType>(Filter);
			command.Connection = (DbConnection)connection;
			command.Transaction = (DbTransaction)transaction;
			try
			{
				reader = command.ExecuteReader();
			}
			catch (Exception ex)
			{
				transaction.Rollback();
				transaction = null;
				throw (ex);
			}
			items = new List<RowType>();
			while (reader.Read())
			{
				item = new RowType();
				foreach (IColumn column in Schema<RowType>.Columns)
				{
					value = commandFactory.ConvertFromDbValue(column, reader[column.Name]);
					column.SetValue(item, value);
				}
				items.Add(item) ;
			}
			return items;
			
		}



	}
}

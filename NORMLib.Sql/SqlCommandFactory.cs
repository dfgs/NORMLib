using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib.Sql
{
	public class SqlCommandFactory : CommandFactory<SqlCommand>
	{
		public override DbCommand CreateCommand(IDatabaseExists Query)
		{
			throw new NotImplementedException();
		}

		public override DbCommand CreateCommand(ICreateDatabase Query)
		{
			throw new NotImplementedException();
		}

		public override DbCommand CreateCommand<RowType>(ITableExists<RowType> Query)
		{
			throw new NotImplementedException();
		}

		public override DbCommand CreateCommand<RowType>(ICreateTable<RowType> Query)
		{
			throw new NotImplementedException();
		}

		public override DbCommand CreateCommand<RowType>(ICreateColumn<RowType> Query)
		{
			throw new NotImplementedException();
		}

		public override DbCommand CreateCommand<PrimaryRowType, ForeignRowType,ValueType>(ICreateRelation<PrimaryRowType, ForeignRowType,ValueType> Query)
		{
			throw new NotImplementedException();
		}

		
		public override DbCommand CreateCommand(ISelectIdentity Query)
		{
			return new SqlCommand("SELECT @@identity");
		}

		
		protected override void OnSetParameter(SqlCommand Command, string Name, object Value)
		{
			Command.Parameters.AddWithValue(Name, Value);
		}


	}
}

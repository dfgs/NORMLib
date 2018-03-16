using NORMLib.Columns;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public class CreateRelation<PrimaryRowType, ForeignRowType,ValueType> : Query, ICreateRelation<PrimaryRowType,ForeignRowType,ValueType>
	{

		private IColumn<ValueType> primaryColumn;
		public IColumn<ValueType> PrimaryColumn
		{
			get { return primaryColumn; }
		}
		private IColumn<ValueType> foreignColumn;
		public IColumn<ValueType> ForeignColumn
		{
			get { return foreignColumn; }
		}

		public string PrimaryTableName
		{
			get { return Table<PrimaryRowType>.Name; }
		}

		public string ForeignTableName
		{
			get { return Table<ForeignRowType>.Name; }
		}

		public CreateRelation(IColumn<ValueType> PrimaryColumn, IColumn<ValueType> ForeignColumn)
		{
			this.primaryColumn = PrimaryColumn;this.foreignColumn = ForeignColumn;
		}

		public override DbCommand CreateCommand(ICommandFactory CommandFactory)
		{
			return CommandFactory.CreateCommand(this);
		}
	}

}

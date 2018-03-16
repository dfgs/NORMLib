using NORMLib.Columns;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public class Insert<RowType>:ColumnsQuery<RowType>, IInsert<RowType>
	{

		private RowType item;
		public RowType Item
		{
			get { return item; }
		}
		
		public Insert(RowType Item, params IColumn[] Columns):base(Columns)
		{
			this.item = Item;
		}

		public override DbCommand CreateCommand(ICommandFactory CommandFactory)
		{
			return CommandFactory.CreateCommand(this);
		}

	}

}

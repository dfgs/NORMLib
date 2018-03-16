using NORMLib.Columns;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public class Update<RowType>:ColumnsQuery<RowType>, IUpdate<RowType>
	{
		private RowType item;
		public RowType Item
		{
			get { return item; }
		}

		private Filter filter;
		public Filter Filter
		{
			get { return filter; }
		}

		public Update(RowType Item, params IColumn[] Columns):base(Columns)
		{
			this.item = Item;
		}

		public override DbCommand CreateCommand(ICommandFactory CommandFactory)
		{
			return CommandFactory.CreateCommand(this);
		}

		public Update<RowType> Where(Filter Filter)
		{
			this.filter = Filter;
			return this;
		}


		


	}

}

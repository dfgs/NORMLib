using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public class Select<RowType> : ColumnsQuery<RowType>, ISelect<RowType>
	{
		
		private IColumn[] orders;
		public IEnumerable<IColumn> Orders
		{
			get { return orders; }
		}

		private Filter filter;
		public Filter Filter
		{
			get { return filter; }
		}

		public Select(params IColumn[] Columns):base(Columns)
		{
			
		}

		public IQuery OrderBy(params IColumn[] Columns)
		{
			this.orders = Columns;
			return this;
		}

		public ISelect<RowType> Where(Filter Filter)
		{
			this.filter = Filter;
			return this;
		}

		public override DbCommand CreateCommand(ICommandFactory CommandFactory)
		{
			return CommandFactory.CreateCommand(this);
		}
	}

}

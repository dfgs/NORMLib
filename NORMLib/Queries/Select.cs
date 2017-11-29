using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public class Select<RowType> : FilterQuery<RowType>, ISelect<RowType>
	{
		

		private IColumn[] orders;
		public IEnumerable<IColumn> Orders
		{
			get { return orders; }
		}

		public Select(params IColumn[] Columns):base(Columns)
		{
			
		}

		public IQuery OrderBy(params IColumn[] Columns)
		{
			this.orders = Columns;
			return this;
		}

	}

}

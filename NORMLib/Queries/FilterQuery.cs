using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public abstract class FilterQuery<RowType> : ColumnsQuery<RowType>,IFilterQuery<RowType>
	{
		

		private Filter filter;
		public Filter Filter
		{
			get { return filter; }
		}


		public FilterQuery(params IColumn[] Columns):base(Columns)
		{

		}

		public IFilterQuery<RowType> Where(Filter Filter)
		{
			this.filter = Filter;
			return this;
		}


	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public class AndFilter<DataType>:Filter<DataType>
	{
		private Filter<DataType>[] filters;
		public Filter<DataType>[] Filters
		{
			get { return filters; }
		}

		


		public AndFilter(params Filter<DataType>[] Filters)
		{
			this.filters = Filters;
		}

		public override string ToString()
		{
			return string.Join(" AND ", filters.Select(item=>$"({item})"));
		}


	}
}

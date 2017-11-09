using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public class AndFilter:Filter
	{
		private Filter[] filters;
		public Filter[] Filters
		{
			get { return filters; }
		}

		


		public AndFilter(params Filter[] Filters)
		{
			this.filters = Filters;
		}

		public override string ToString()
		{
			return string.Join(" AND ", filters.Select(item=>$"({item})"));
		}


	}
}

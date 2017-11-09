using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public class OrFilter:Filter
	{
		private Filter[] filters;
		public Filter[] Filters
		{
			get { return filters; }
		}

		


		public OrFilter(params Filter[] Filters)
		{
			this.filters = Filters;
		}

		public override string ToString()
		{
			return string.Join(" OR ", filters.Select(item => $"({item})"));

		}


	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public class OrFilter<DataType>:Filter<DataType>
	{
		private Filter<DataType>[] filters;
		public Filter<DataType>[] Filters
		{
			get { return filters; }
		}

		


		public OrFilter(params Filter<DataType>[] Filters)
		{
			this.filters = Filters;
		}

		public override string ToString()
		{
			return string.Join(" OR ", filters.Select(item => $"({item})"));

		}


	}
}

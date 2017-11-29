using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public interface IFilterQuery<RowType>:IColumnsQuery<RowType>
	{
		

		Filter Filter
		{
			get;
		}
		

		IFilterQuery<RowType> Where(Filter Filter);


	}
}

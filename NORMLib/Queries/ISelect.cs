using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public interface ISelect<RowType>: IColumnsQuery<RowType>,IFilterQuery<RowType>,IOrdersQuery<RowType>
	{
		IEnumerable<IJoin> Joins
		{
			get;
		}

		
	}
}

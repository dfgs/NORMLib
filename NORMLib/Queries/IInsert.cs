using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public interface IInsert<RowType> : IColumnsQuery<RowType>
	{

		RowType Item
		{
			get;
		}



	}
}

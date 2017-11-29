using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public interface ICreateTable<RowType>:IColumnsQuery<RowType>
	{
		IColumn PrimaryKey
		{
			get;
		}

	}
}

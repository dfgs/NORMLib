using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public abstract class TableQuery<RowType> : Query,ITableQuery<RowType>
	{
		public string TableName
		{
			get { return Table<RowType>.Name; }
		}






	}
}

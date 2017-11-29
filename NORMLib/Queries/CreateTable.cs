using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public class CreateTable<RowType> : ColumnsQuery<RowType>, ICreateTable<RowType>
	{
		public IColumn PrimaryKey
		{
			get { return Table<RowType>.PrimaryKey; }
		}

		public CreateTable(params IColumn[] Columns):base(Columns)
		{

		}

		
	}

}

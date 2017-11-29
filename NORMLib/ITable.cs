using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public interface ITable
	{
		string Name
		{
			get;
		}

		int MaxColumnRevision
		{
			get;
		}

		IEnumerable<IColumn> Columns
		{
			get;
		}

		IColumn PrimaryKey
		{
			get;
		}

		IColumn IdentityColumn
		{
			get;
		}

		IEnumerable<IColumn> GetColumns(int MinRevision, int MaxRevision );

		IQuery GetCreateQuery(params IColumn[] Columns);
		IQuery GetCreateQuery(IColumn Column);

	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib.VersionControl
{
	public interface IVersionController
	{
		void Run();

		List<RowType> Select<RowType>(Filter Filter = null)
		   where RowType : new();
		void Insert<RowType>(RowType Item);
		void Update<RowType>(RowType Item);
		void Delete<RowType>(RowType Item);
		void CreateTable<RowType>(params IColumn[] Columns);
		void CreateColumn(IColumn Column);
					
	}
}

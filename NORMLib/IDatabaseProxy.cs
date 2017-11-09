using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public interface IDatabaseProxy:IDisposable
	{
		void Insert<RowType>(RowType Item);
		void Update<RowType>(RowType Item);
		void Delete<RowType>(RowType Item);
		List<RowType> Select<RowType>(Filter Filter=null)
			where RowType:new();

	}
}

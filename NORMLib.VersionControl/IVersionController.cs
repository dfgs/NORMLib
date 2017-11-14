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


		List<RowType> Select<RowType>(Filter Filter)
			where RowType : new();
		void Insert<RowType>(RowType Item);
		void Update<RowType>(RowType Item);
		void Delete<RowType>(RowType Item);

	}
}

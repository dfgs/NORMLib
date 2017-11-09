using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public interface IDatabaseProxy:IDisposable
	{
		void Insert<DataType>(DataType Item);
		void Update<DataType>(DataType Item);
		void Delete<DataType>(DataType Item);
		IEnumerable<DataType> Select<DataType>()
			where DataType:new();

	}
}

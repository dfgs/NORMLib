using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public interface IServer : IDisposable
	{
		bool DatabaseExists();
		void CreateDatabase();

		void ExecuteTransaction(params IQuery[] Queries);
		int ExecuteNonQuery(IQuery Query);
		object ExecuteScalar(IQuery Query);
		IEnumerable<RowType> Execute<RowType>(ISelect<RowType> Query)
			where RowType:new();


	}
}

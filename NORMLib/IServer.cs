using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public interface IServer : IDisposable
	{
		void Execute(params IQuery[] Queries);

		bool Execute(IDatabaseExists Query);
		int Execute(ICreateDatabase Query);

		bool Execute<RowType>(ITableExists<RowType> Query);
		int Execute<RowType>(ICreateTable<RowType> Query);
		int Execute<RowType>(ICreateColumn<RowType> Query);
		int Execute<PrimaryRowType, ForeignRowType>(ICreateRelation<PrimaryRowType, ForeignRowType> Query);

		int Execute<RowType>(IInsert<RowType> Query);
		int Execute<RowType>(IUpdate<RowType> Query);
		int Execute<RowType>(IDelete<RowType> Query);
		List<RowType> Execute<RowType>(ISelect<RowType> Query)
			where RowType:new();


	}
}

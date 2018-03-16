using NORMLib.Columns;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public interface ICommandFactory
	{
		//DbCommand CreateIdentityCommand<RowType>();

		DbCommand CreateCommand(IDatabaseExists Query);
		DbCommand CreateCommand(ICreateDatabase Query);
		DbCommand CreateCommand(ISelectIdentity Query);

		DbCommand CreateCommand<RowType>(ITableExists<RowType> Query);
		DbCommand CreateCommand<RowType>(ICreateTable<RowType> Query);
		DbCommand CreateCommand<RowType>(ICreateColumn<RowType> Query);
		DbCommand CreateCommand<PrimaryRowType, ForeignRowType, ValueType>(ICreateRelation<PrimaryRowType, ForeignRowType, ValueType> Query);

		DbCommand CreateCommand<RowType>(ISelect<RowType> Query);
		DbCommand CreateCommand<RowType>(IDelete<RowType> Query);
		DbCommand CreateCommand<RowType>(IUpdate<RowType> Query);
		DbCommand CreateCommand<RowType>(IInsert<RowType> Query);

		object ConvertToDbValue(IColumn Column, object Row);
		object ConvertFromDbValue(IColumn Column, object Value);

	}
}

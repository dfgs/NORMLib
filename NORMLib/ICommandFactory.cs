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
		DbCommand CreateIdentityCommand<RowType>();
		DbCommand CreateInsertCommand<RowType>(RowType Row, IEnumerable<IColumn> Columns);
		DbCommand CreateUpdateCommand<RowType>(RowType Row, IEnumerable<IColumn> Columns);
		DbCommand CreateDeleteCommand<RowType>(RowType Row);
		DbCommand CreateSelectCommand<RowType>(IEnumerable<IColumn> Columns, Filter Filter );

		object ConvertToDbValue(IColumn Column, object Row);
		object ConvertFromDbValue(IColumn Column, object Value);

		DbCommand CreateSelectDatabaseCommand(string DatabaseName);
		DbCommand CreateCreateDatabaseCommand(string DatabaseName);
		DbCommand CreateDropDatabaseCommand(string DatabaseName);

		DbCommand CreateSelectTableCommand(ITable Table);
		DbCommand CreateCreateTableCommand(ITable Table, IEnumerable<IColumn> Columns);
		DbCommand CreateCreateColumnCommand(ITable Table, IColumn Column);
		DbCommand CreateCreateRelationCommand(IRelation Relation);

	}
}

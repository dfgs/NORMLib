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
		DbCommand CreateIdentityCommand<RowType>(RowType Item);
		DbCommand CreateInsertCommand<RowType>(RowType Item);
		DbCommand CreateUpdateCommand<RowType>(RowType Item);
		DbCommand CreateDeleteCommand<RowType>(RowType Item);
		DbCommand CreateSelectCommand<RowType>(Filter Filter);

		object ConvertToDbValue(IColumn Column, object Component);
		object ConvertFromDbValue(IColumn Column, object Value);

		DbCommand CreateSelectDatabaseCommand(string DatabaseName);
		DbCommand CreateCreateDatabaseCommand(string DatabaseName);
		DbCommand CreateDropDatabaseCommand(string DatabaseName);

		DbCommand CreateSelectTableCommand<RowType>();
		DbCommand CreateCreateTableCommand<RowType>(params IColumn[] Columns);

		DbCommand CreateCreateColumnCommand(IColumn Column);

	}
}

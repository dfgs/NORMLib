using System;
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
		DbCommand CreateIdentityCommand<DataType>(DataType Item);
		DbCommand CreateInsertCommand<DataType>(DataType Item);
		DbCommand CreateUpdateCommand<DataType>(DataType Item);
		DbCommand CreateDeleteCommand<DataType>(DataType Item);
		DbCommand CreateSelectCommand<DataType>();

		object ConvertToDbValue<DataType>(IColumn<DataType> Column, DataType Component);
		object ConvertFromDbValue<DataType>(IColumn<DataType> Column, object Value);
		
			

	}
}

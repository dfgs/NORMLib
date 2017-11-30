using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public interface ICreateRelation<PrimaryRowType,ForeignRowType,ValueType>:IQuery
	{
		IColumn<ValueType> PrimaryColumn
		{
			get;
		}
		IColumn<ValueType> ForeignColumn
		{
			get;
		}

		string PrimaryTableName
		{
			get;
		}

		string ForeignTableName
		{
			get;
		}

	}
}

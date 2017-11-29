using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public interface ICreateRelation<PrimaryRowType,ForeignRowType>:IQuery
	{
		IColumn PrimaryColumn
		{
			get;
		}
		IColumn ForeignColumn
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

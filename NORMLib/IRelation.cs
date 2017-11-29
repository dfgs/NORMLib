using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public interface IRelation
	{
		string Name
		{
			get;
		}
		string PrimaryTable
		{
			get;
		}
		IColumn PrimaryColumn
		{
			get;
		}

		string ForeignTable
		{
			get;
		}
		IColumn ForeignColumn
		{
			get;
		}

		DeleteReferentialAction DeleteReferentialAction
		{
			get;
		}

		IQuery GetCreateQuery();

	}

	public interface IRelation<ValueType>:IRelation
	{
		

		new IColumn<ValueType> PrimaryColumn
		{
			get;
		}

		new IColumn<ValueType> ForeignColumn
		{
			get;
		}

		


	}

}

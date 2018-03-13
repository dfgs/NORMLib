using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public interface IColumn
	{
		string Name
		{
			get;
		}

		string TableName
		{
			get;
		}//*/
		bool IsPrimaryKey
		{
			get;
		}
		bool IsIdentity
		{
			get;
		}
		Type ColumnType
		{
			get;
		}
		
		bool IsNullable
		{
			get;
		}
	
		object DefaultValue
		{
			get;
		}

		object GetValue(object Component);
		void SetValue(object Component, object Value);

		//int Compare(Component A, Component B);
		
		Filter IsEqualTo(object Value);
		Filter IsGreaterOrEqualsThan(object Value);
		Filter IsLowerOrEqualsThan(object Value);
		Filter IsGreaterThan(object Value);
		Filter IsLowerThan(object Value);
	}

	public interface IColumn<ValType>:IColumn
	{
		new ValType DefaultValue
		{
			get;
		}
		new ValType GetValue(object Component);
		void SetValue(object Component,ValType Value);
	}

	public interface IColumn<RowType,ValType> : IColumn<ValType>
	{
		ValType GetValue(RowType Component);
		void SetValue(RowType Component, ValType Value);
	}

}

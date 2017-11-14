using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NORMLib
{
	public class Column<ValType> : IColumn<ValType>
	{
		/*public string TableName
		{
			get { return Table<RowType>.TableName; }
		}*/

		public string Name
		{
			get;
			private set;
		}

		public bool IsIdentity
		{
			get;
			set;
		}

		public bool IsNullable
		{
			get;
			set;
		}

		//private ValType defaultValue;
		public ValType DefaultValue
		{
			get;
			set;
		}

		/*object IColumn.DefaultValue
		{
			get { return DefaultValue; }
		}*/

		//private bool isPrimaryKey;
		public bool IsPrimaryKey
		{
			get;
			set;
		}


		public Type ColumnType
		{
			get { return typeof(ValType); }
		}

		

		

		private static Regex nameRegex = new Regex(@"^(.*)Column$");


		/*private PropertyInfo propertyInfo;
		public PropertyInfo PropertyInfo
		{
			get { return propertyInfo; }
		}*/

		private Dictionary<object, ValType> values;



		public Column([CallerMemberName]string Name = null)
		{
			Match match;

			values = new Dictionary<object, ValType>();

			match = nameRegex.Match(Name);
			if (match.Success) this.Name = match.Groups[1].Value;
			else this.Name = Name;

			//Converter = TypeDescriptor.GetConverter(typeof(ValType));
			DefaultValue = default(ValType);
		}



		public override string ToString()
		{
			return Name;
		}



		void IColumn.SetValue(object Component, object Value)
		{
			SetValue(Component, (ValType)Value);
		}

		public void SetValue(object Component, ValType Value)
		{
			if ((Value == null) && (!IsNullable)) throw (new InvalidOperationException($"NULL values are not allowed for column {Name}"));
			if (values.ContainsKey(Component)) values[Component] = Value;
			else values.Add(Component, Value);
		}

		object IColumn.GetValue(object Component)
		{
			return GetValue(Component);
		}
		public ValType GetValue(object Component)
		{
			ValType value;
			if (!values.TryGetValue(Component, out value)) return DefaultValue;
			return value;
		}


	}
}

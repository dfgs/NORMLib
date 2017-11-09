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
	public abstract class BaseColumn<DataType,ValueType>: IColumn<DataType>
	{
		public string TableName
		{
			get { return Schema<DataType>.TableName; }
		}

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

		//private ValueType defaultValue;
		public ValueType DefaultValue
		{
			get;
			set;
		}

		object IColumn.DefaultValue
		{
			get { return DefaultValue; }
		}

		//private bool isPrimaryKey;
		public bool IsPrimaryKey
		{
			get;
			set;
		}

		
		public abstract Type ColumnType
		{
			get;
		}

		public IColumn ForeignKey
		{
			get;
			set;
		}
		private static Regex nameRegex = new Regex(@"^(.*)Column$");


		/*private PropertyInfo propertyInfo;
		public PropertyInfo PropertyInfo
		{
			get { return propertyInfo; }
		}*/

		private Dictionary<DataType, ValueType> values;


		public BaseColumn(string Name)
		{
			Match match;

			values = new Dictionary<DataType, ValueType>();

			match = nameRegex.Match(Name);
			if (match.Success) this.Name = match.Groups[1].Value;
			else this.Name = Name;

			//Converter = TypeDescriptor.GetConverter(typeof(ValueType));
			DefaultValue = default(ValueType);
		}



		public override string ToString()
		{
			return Name;
		}


		object IColumn<DataType>.GetValue(DataType Component)
		{
			return GetValue(Component);
		}
		
		void IColumn<DataType>.SetValue(DataType Component, object Value)
		{
			SetValue(Component, (ValueType)Value);
		}

		public void SetValue(DataType Component, ValueType Value)
		{
			if ((Value == null) && (!IsNullable)) throw (new InvalidOperationException($"NULL values are not allowed for column {Name}"));
			if (values.ContainsKey(Component)) values[Component] = Value;
			else values.Add(Component, Value);
		}

		public ValueType GetValue(DataType Component)
		{
			ValueType value;
			if (!values.TryGetValue(Component, out value)) return DefaultValue;
			return value;
		}


	}
}

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

		public ValType DefaultValue
		{
			get;
			set;
		}

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
			if ((Value == null) && (!IsNullable) && (!IsIdentity)) throw (new InvalidOperationException($"NULL values are not allowed for column {Name}"));
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

		public Filter IsEqualTo(object Value)
		{
			return new EqualFilter(this, Value);
		}

		public Filter IsGreaterOrEqualsThan(object Value)
		{
			return new GreaterOrEqualFilter(this, Value);
		}

		public Filter IsLowerOrEqualsThan(object Value)
		{
			return new LowerOrEqualFilter(this, Value);
		}

		public Filter IsGreaterThan(object Value)
		{
			return new GreaterFilter(this, Value);
		}

		public Filter IsLowerThan(object Value)
		{
			return new LowerFilter(this, Value);
		}

	}
}

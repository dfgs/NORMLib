using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public abstract class CommandFactory<CommandType> : ICommandFactory
		where CommandType:DbCommand,new()
	{

		#region filters
		protected virtual string OnCreateEqualFilter<DataType>(EqualFilter<DataType> Filter, List<Tuple<string, object>> Parameters)
		{
			Tuple<string, object> parameter;
			parameter = new Tuple<string, object>(OnCreateParameterName(Filter.Column, Parameters.Count), Filter.Value); Parameters.Add(parameter);
			if (Filter.Value == null) return OnFormatColumnName(Filter.Column) + " is null";
			else return OnFormatColumnName(Filter.Column) + " = " + parameter.Item1;
		}
		protected virtual string OnCreateGreaterFilter<DataType>(GreaterFilter<DataType> Filter, List<Tuple<string, object>> Parameters)
		{
			Tuple<string, object> parameter;
			parameter = new Tuple<string, object>(OnCreateParameterName(Filter.Column, Parameters.Count), Filter.Value); Parameters.Add(parameter);
			if (Filter.Value == null) throw (new ArgumentNullException("Value"));
			else return OnFormatColumnName(Filter.Column) + " > " + parameter.Item1;
		}
		protected virtual string OnCreateLowerFilter<DataType>(LowerFilter<DataType> Filter, List<Tuple<string, object>> Parameters)
		{
			Tuple<string, object> parameter;
			parameter = new Tuple<string, object>(OnCreateParameterName(Filter.Column, Parameters.Count), Filter.Value); Parameters.Add(parameter);
			if (Filter.Value == null) throw (new ArgumentNullException("Value"));
			else return OnFormatColumnName(Filter.Column) + " < " + parameter.Item1;
		}
		protected virtual string OnCreateGreaterOrEqualFilter<DataType>(GreaterOrEqualFilter<DataType> Filter, List<Tuple<string, object>> Parameters)
		{
			Tuple<string, object> parameter;
			parameter = new Tuple<string, object>(OnCreateParameterName(Filter.Column, Parameters.Count), Filter.Value); Parameters.Add(parameter);
			if (Filter.Value == null) throw (new ArgumentNullException("Value"));
			else return OnFormatColumnName(Filter.Column) + " >= " + parameter.Item1;
		}
		protected virtual string OnCreateLowerOrEqualFilter<DataType>(LowerOrEqualFilter<DataType> Filter, List<Tuple<string, object>> Parameters)
		{
			Tuple<string, object> parameter;
			parameter = new Tuple<string, object>(OnCreateParameterName(Filter.Column, Parameters.Count), Filter.Value); Parameters.Add(parameter);
			if (Filter.Value == null) throw (new ArgumentNullException("Value"));
			else return OnFormatColumnName(Filter.Column) + " <= " + parameter.Item1;
		}
		protected virtual string OnCreateAndFilter<DataType>(AndFilter<DataType> Filter, List<Tuple<string, object>> Parameters)
		{
			string result;

			if ((Filter.Filters == null) || (Filter.Filters.Length == 0)) throw (new ArgumentNullException("Filters")); ;

			result = "(" + OnCreateFilter<DataType>(Filter.Filters[0], Parameters) + ")";
			for (int t = 1; t < Filter.Filters.Length; t++)
			{
				result += " AND (" + OnCreateFilter<DataType>(Filter.Filters[t], Parameters) + ")";
			}

			return result;
		}
		protected virtual string OnCreateOrFilter<DataType>(OrFilter<DataType> Filter, List<Tuple<string, object>> Parameters)
		{
			string result;

			if ((Filter.Filters == null) || (Filter.Filters.Length == 0)) throw (new ArgumentNullException("Filters")); ;

			result = "(" + OnCreateFilter<DataType>(Filter.Filters[0], Parameters) + ")";
			for (int t = 1; t < Filter.Filters.Length; t++)
			{
				result += " OR (" + OnCreateFilter<DataType>(Filter.Filters[t], Parameters) + ")";
			}

			return result;
		}

		private string OnCreateFilter<DataType>(Filter<DataType> Filter, List<Tuple<string, object>> Parameters)
		{

			if (Filter is EqualFilter<DataType>) return OnCreateEqualFilter((EqualFilter<DataType>)Filter, Parameters);
			else if (Filter is GreaterFilter<DataType>) return OnCreateGreaterFilter((GreaterFilter<DataType>)Filter, Parameters);
			else if (Filter is LowerFilter<DataType>) return OnCreateLowerFilter((LowerFilter<DataType>)Filter, Parameters);
			else if (Filter is GreaterOrEqualFilter<DataType>) return OnCreateGreaterOrEqualFilter((GreaterOrEqualFilter<DataType>)Filter, Parameters);
			else if (Filter is LowerOrEqualFilter<DataType>) return OnCreateLowerOrEqualFilter((LowerOrEqualFilter<DataType>)Filter, Parameters);
			else if (Filter is AndFilter<DataType>) return OnCreateAndFilter((AndFilter<DataType>)Filter, Parameters);
			else if (Filter is OrFilter<DataType>) return OnCreateOrFilter((OrFilter<DataType>)Filter, Parameters);

			else throw (new InvalidOperationException("Unsupported filter"));
		}
		#endregion


		protected virtual string OnFormatColumnName(IColumn Column)
		{
			return "[" + Column.Name + "]";
		}

		protected virtual string OnFormatTableName(string TableName)
		{
			return "[" + TableName + "]";
		}

		protected virtual string OnCreateParameterName<DataType>(IColumn<DataType> Column, int Index)
		{
			return "@_" + Column.Name + Index.ToString();
		}

		protected virtual object OnConvertToDbValue<DataType>(IColumn<DataType> Column, DataType Component)
		{
			object value;

			value = Column.GetValue(Component);
			if (value == null) return DBNull.Value;

			return value;
		}
		
		protected virtual object OnConvertFromDbValue<DataType>(IColumn<DataType> Column, object Value)
		{
			if (Column.ColumnType.IsEnum) return Enum.ToObject(Column.ColumnType, Value);
			if (Value == DBNull.Value) return null;
			return Value;
		}

		public object ConvertToDbValue<DataType>(IColumn<DataType> Column, DataType Component)
		{
			return OnConvertToDbValue<DataType>(Column, Component);
		}

		public object ConvertFromDbValue<DataType>(IColumn<DataType> Column, object Value)
		{
			return OnConvertFromDbValue<DataType>(Column, Value);
		}


		protected abstract void OnSetParameter<DataType>(CommandType Command, string Name, object Value);

		protected abstract CommandType OnCreateIdentityCommand<DataType>(DataType Item);

		public DbCommand CreateIdentityCommand<DataType>(DataType Item)
		{
			return OnCreateIdentityCommand<DataType>(Item) ;
		}

		public DbCommand CreateInsertCommand<DataType>(DataType Item)
		{
			string sql;
			CommandType command;
			IEnumerable<IColumn<DataType>> columns;

			columns = Schema<DataType>.Columns.Where(item => (!item.IsIdentity ));

			sql = "insert into " + OnFormatTableName(Schema<DataType>.TableName);
			sql += " (" + string.Join(",", columns.Select(item => OnFormatColumnName(item))) + ") values (";
			sql += string.Join(",", columns.Select( item => OnCreateParameterName(item, 0))) + ")";

			command = new CommandType();
			command.CommandText = sql;
			foreach (IColumn<DataType> column in columns)
			{
				OnSetParameter<DataType>(command, OnCreateParameterName(column, 0), OnConvertToDbValue(column, Item));
			}

			return command;
		}

		public DbCommand CreateUpdateCommand<DataType>(DataType Item)
		{
			string sql;
			CommandType command;
			IEnumerable<IColumn<DataType>> columns;

			columns = Schema<DataType>.Columns.Where(item => (!item.IsPrimaryKey) && (!item.IsIdentity));

			sql = "update " + OnFormatTableName(Schema<DataType>.TableName) + " set ";
			sql += string.Join(",", columns.Select(item => OnFormatColumnName(item) + "=" + OnCreateParameterName(item, 0)));
			sql += " where " + OnFormatColumnName(Schema<DataType>.PrimaryKey) + "=" + OnCreateParameterName(Schema<DataType>.PrimaryKey, 1);

			command = new CommandType();
			command.CommandText = sql;

			foreach (IColumn<DataType> column in columns)
			{
				OnSetParameter<DataType>(command, OnCreateParameterName(column, 0), OnConvertToDbValue(column, Item));
			}
			OnSetParameter<DataType>(command, OnCreateParameterName(Schema<DataType>.PrimaryKey, 1), OnConvertToDbValue(Schema<DataType>.PrimaryKey, Item));

			return command;
		}

		public DbCommand CreateDeleteCommand<DataType>(DataType Item)
		{
			string sql;
			CommandType command;
			object key;

			key = OnConvertToDbValue(Schema<DataType>.PrimaryKey, Item);

			sql = "delete from " + OnFormatTableName(Schema<DataType>.TableName);
			sql += " where " + OnFormatColumnName(Schema<DataType>.PrimaryKey) + "=" + OnCreateParameterName(Schema<DataType>.PrimaryKey, 0);

			command = new CommandType();
			command.CommandText = sql;
			OnSetParameter<DataType>(command, OnCreateParameterName(Schema<DataType>.PrimaryKey, 0), key);

			return command;
		}

		public DbCommand CreateSelectCommand<DataType>()
		{
			string sql;
			CommandType command;
			IEnumerable<IColumn<DataType>> columns;

			columns = Schema<DataType>.Columns;

			sql = "select " + string.Join(",", columns.Select(item => OnFormatColumnName(item)));
			sql += " from "+OnFormatTableName(Schema<DataType>.TableName);
			
			command = new CommandType();
			command.CommandText = sql;
			

			return command;
		}



	}
}

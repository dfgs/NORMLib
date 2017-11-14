using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public abstract class CommandFactory<CommandType> : ICommandFactory
		where CommandType : DbCommand, new()
	{

		#region filters
		protected virtual string OnCreateEqualFilter(EqualFilter Filter, List<Tuple<string, object>> Parameters)
		{
			Tuple<string, object> parameter;
			parameter = new Tuple<string, object>(OnCreateParameterName(Filter.Column, Parameters.Count), Filter.Value); Parameters.Add(parameter);
			if (Filter.Value == null) return OnFormatColumnName(Filter.Column) + " is null";
			else return OnFormatColumnName(Filter.Column) + " = " + parameter.Item1;
		}
		protected virtual string OnCreateGreaterFilter(GreaterFilter Filter, List<Tuple<string, object>> Parameters)
		{
			Tuple<string, object> parameter;
			parameter = new Tuple<string, object>(OnCreateParameterName(Filter.Column, Parameters.Count), Filter.Value); Parameters.Add(parameter);
			if (Filter.Value == null) throw (new ArgumentNullException("Value"));
			else return OnFormatColumnName(Filter.Column) + " > " + parameter.Item1;
		}
		protected virtual string OnCreateLowerFilter(LowerFilter Filter, List<Tuple<string, object>> Parameters)
		{
			Tuple<string, object> parameter;
			parameter = new Tuple<string, object>(OnCreateParameterName(Filter.Column, Parameters.Count), Filter.Value); Parameters.Add(parameter);
			if (Filter.Value == null) throw (new ArgumentNullException("Value"));
			else return OnFormatColumnName(Filter.Column) + " < " + parameter.Item1;
		}
		protected virtual string OnCreateGreaterOrEqualFilter(GreaterOrEqualFilter Filter, List<Tuple<string, object>> Parameters)
		{
			Tuple<string, object> parameter;
			parameter = new Tuple<string, object>(OnCreateParameterName(Filter.Column, Parameters.Count), Filter.Value); Parameters.Add(parameter);
			if (Filter.Value == null) throw (new ArgumentNullException("Value"));
			else return OnFormatColumnName(Filter.Column) + " >= " + parameter.Item1;
		}
		protected virtual string OnCreateLowerOrEqualFilter(LowerOrEqualFilter Filter, List<Tuple<string, object>> Parameters)
		{
			Tuple<string, object> parameter;
			parameter = new Tuple<string, object>(OnCreateParameterName(Filter.Column, Parameters.Count), Filter.Value); Parameters.Add(parameter);
			if (Filter.Value == null) throw (new ArgumentNullException("Value"));
			else return OnFormatColumnName(Filter.Column) + " <= " + parameter.Item1;
		}
		protected virtual string OnCreateAndFilter(AndFilter Filter, List<Tuple<string, object>> Parameters)
		{
			string result;

			if ((Filter.Filters == null) || (Filter.Filters.Length == 0)) throw (new ArgumentNullException("Filters")); ;

			result = "(" + OnCreateFilter(Filter.Filters[0], Parameters) + ")";
			for (int t = 1; t < Filter.Filters.Length; t++)
			{
				result += " AND (" + OnCreateFilter(Filter.Filters[t], Parameters) + ")";
			}

			return result;
		}
		protected virtual string OnCreateOrFilter(OrFilter Filter, List<Tuple<string, object>> Parameters)
		{
			string result;

			if ((Filter.Filters == null) || (Filter.Filters.Length == 0)) throw (new ArgumentNullException("Filters")); ;

			result = "(" + OnCreateFilter(Filter.Filters[0], Parameters) + ")";
			for (int t = 1; t < Filter.Filters.Length; t++)
			{
				result += " OR (" + OnCreateFilter(Filter.Filters[t], Parameters) + ")";
			}

			return result;
		}

		private string OnCreateFilter(Filter Filter, List<Tuple<string, object>> Parameters)
		{

			if (Filter is EqualFilter) return OnCreateEqualFilter((EqualFilter)Filter, Parameters);
			else if (Filter is GreaterFilter) return OnCreateGreaterFilter((GreaterFilter)Filter, Parameters);
			else if (Filter is LowerFilter) return OnCreateLowerFilter((LowerFilter)Filter, Parameters);
			else if (Filter is GreaterOrEqualFilter) return OnCreateGreaterOrEqualFilter((GreaterOrEqualFilter)Filter, Parameters);
			else if (Filter is LowerOrEqualFilter) return OnCreateLowerOrEqualFilter((LowerOrEqualFilter)Filter, Parameters);
			else if (Filter is AndFilter) return OnCreateAndFilter((AndFilter)Filter, Parameters);
			else if (Filter is OrFilter) return OnCreateOrFilter((OrFilter)Filter, Parameters);

			else throw (new InvalidOperationException("Unsupported filter"));
		}
		#endregion


		protected virtual string OnFormatColumnName(IColumn Column)
		{
			return $"[{Column.Name}]";
		}

		protected virtual string OnFormatTableName(string TableName)
		{
			return $"[{TableName}]";
		}

		protected virtual string OnCreateParameterName(IColumn Column, int Index)
		{
			return $"@_{Column.Name}{Index}";
		}

		protected virtual object OnConvertToDbValue(IColumn Column, object Component)
		{
			object value;

			value = Column.GetValue(Component);
			if (value == null) return DBNull.Value;

			return value;
		}

		protected virtual object OnConvertFromDbValue(IColumn Column, object Value)
		{
			if (Column.ColumnType.IsEnum) return Enum.ToObject(Column.ColumnType, Value);
			if (Value == DBNull.Value) return null;
			return Value;
		}


		public object ConvertToDbValue(IColumn Column, object Component)
		{
			return OnConvertToDbValue(Column, Component);
		}

		public object ConvertFromDbValue(IColumn Column, object Value)
		{
			return OnConvertFromDbValue(Column, Value);
		}


		protected abstract void OnSetParameter<RowType>(CommandType Command, string Name, object Value);


		public abstract DbCommand CreateIdentityCommand<RowType>();
		public abstract DbCommand CreateSelectDatabaseCommand(string DatabaseName);
		public abstract DbCommand CreateCreateDatabaseCommand(string DatabaseName);
		public abstract DbCommand CreateDropDatabaseCommand(string DatabaseName);
		public abstract DbCommand CreateSelectTableCommand(ITable Table);
		public abstract DbCommand CreateCreateTableCommand(ITable Table, IEnumerable<IColumn> Columns);
		public abstract DbCommand CreateCreateColumnCommand(ITable Table, IColumn Column);
		public abstract DbCommand CreateCreateRelationCommand(IRelation Relation);

		public DbCommand CreateInsertCommand<RowType>(RowType Item,IEnumerable<IColumn> Columns)
		{
			string sql;
			CommandType command;

			sql = "insert into " + OnFormatTableName(Table<RowType>.Name);
			sql += " (" + string.Join(",", Columns.Select(item => OnFormatColumnName(item))) + ") values (";
			sql += string.Join(",", Columns.Select(item => OnCreateParameterName(item, 0))) + ")";

			command = new CommandType();
			command.CommandText = sql;
			foreach (IColumn column in Columns)
			{
				OnSetParameter<RowType>(command, OnCreateParameterName(column, 0), OnConvertToDbValue(column, Item));
			}

			return command;
		}

		public DbCommand CreateUpdateCommand<RowType>(RowType Item, IEnumerable<IColumn> Columns)
		{
			string sql;
			CommandType command;

			sql = "update " + OnFormatTableName(Table<RowType>.Name) + " set ";
			sql += string.Join(",", Columns.Select(item => OnFormatColumnName(item) + "=" + OnCreateParameterName(item, 0)));
			sql += " where " + OnFormatColumnName(Table<RowType>.PrimaryKey) + "=" + OnCreateParameterName(Table<RowType>.PrimaryKey, 1);

			command = new CommandType();
			command.CommandText = sql;

			foreach (IColumn column in Columns)
			{
				OnSetParameter<RowType>(command, OnCreateParameterName(column, 0), OnConvertToDbValue(column, Item));
			}
			OnSetParameter<RowType>(command, OnCreateParameterName(Table<RowType>.PrimaryKey, 1), OnConvertToDbValue(Table<RowType>.PrimaryKey, Item));

			return command;
		}

		public DbCommand CreateDeleteCommand<RowType>(RowType Item)
		{
			string sql;
			CommandType command;
			object key;

			key = OnConvertToDbValue(Table<RowType>.PrimaryKey, Item);

			sql = "delete from " + OnFormatTableName(Table<RowType>.Name);
			sql += " where " + OnFormatColumnName(Table<RowType>.PrimaryKey) + "=" + OnCreateParameterName(Table<RowType>.PrimaryKey, 0);

			command = new CommandType();
			command.CommandText = sql;
			OnSetParameter<RowType>(command, OnCreateParameterName(Table<RowType>.PrimaryKey, 0), key);

			return command;
		}

		public DbCommand CreateSelectCommand<RowType>(IEnumerable<IColumn> Columns, Filter Filter)
		{
			string sql;
			CommandType command;
			List<Tuple<string, object>> parameters;

			parameters = new List<Tuple<string, object>>();

			sql = "select " + string.Join(",", Columns.Select(item => OnFormatColumnName(item)));
			sql += " from " + OnFormatTableName(Table<RowType>.Name);

			if (Filter != null)
			{
				sql += " where " + OnCreateFilter(Filter,parameters);
				
			}

			command = new CommandType();
			command.CommandText = sql;

			foreach (Tuple<string, object> parameter in parameters)
			{
				OnSetParameter<CommandType>(command, parameter.Item1, parameter.Item2);
			}

			return command;
		}


	}
}

	

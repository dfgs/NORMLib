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


		protected abstract void OnSetParameter(CommandType Command, string Name, object Value);


		public abstract DbCommand CreateIdentityCommand<RowType>();

		public DbCommand CreateCommand<RowType>(ISelect<RowType> Query)
		{
			string sql;
			CommandType command;
			List<Tuple<string, object>> parameters;

			parameters = new List<Tuple<string, object>>();

			sql = "select " + string.Join(",", Query.Columns.Select(item => OnFormatColumnName(item)));
			sql += " from " + OnFormatTableName(Query.TableName);

			if (Query.Filter != null)
			{
				sql += " where " + OnCreateFilter(Query.Filter, parameters);
			}

			if (Query.Orders != null)
			{
				sql += " order by " + string.Join(",", Query.Orders.Select(item => OnFormatColumnName(item)));
			}

			command = new CommandType();
			command.CommandText = sql;

			foreach (Tuple<string, object> parameter in parameters)
			{
				OnSetParameter(command, parameter.Item1, parameter.Item2);
			}

			return command;
		}

		public DbCommand CreateCommand<RowType>(IInsert<RowType> Query)
		{
			string sql;
			CommandType command;

			sql = "insert into " + OnFormatTableName(Query.TableName);
			sql += " (" + string.Join(",", Query.Columns.Select(item => OnFormatColumnName(item))) + ") values (";
			sql += string.Join(",", Query.Columns.Select(item => OnCreateParameterName(item, 0))) + ")";

			command = new CommandType();
			command.CommandText = sql;
			foreach (IColumn column in Query.Columns)
			{
				OnSetParameter(command, OnCreateParameterName(column, 0), OnConvertToDbValue(column, Query.Item));
			}

			return command;
		}

		public DbCommand CreateCommand<RowType>(IUpdate<RowType> Query)
		{
			string sql;
			CommandType command;
			List<Tuple<string, object>> parameters;

			parameters = new List<Tuple<string, object>>();

			sql = "update " + OnFormatTableName(Query.TableName) + " set ";
			sql += string.Join(",", Query.Columns.Select(item => OnFormatColumnName(item) + "=" + OnCreateParameterName(item, 0)));

			if (Query.Filter != null)
			{
				sql += " where " + OnCreateFilter(Query.Filter, parameters);
			}

			command = new CommandType();
			command.CommandText = sql;

			foreach (IColumn column in Query.Columns)
			{
				OnSetParameter(command, OnCreateParameterName(column, 0), OnConvertToDbValue(column, Query.Item));
			}

			foreach (Tuple<string, object> parameter in parameters)
			{
				OnSetParameter(command, parameter.Item1, parameter.Item2);
			}

			return command;
		}

		public DbCommand CreateCommand<RowType>(IDelete<RowType> Query)
		{
			string sql;
			CommandType command;
			List<Tuple<string, object>> parameters;

			parameters = new List<Tuple<string, object>>();

			sql = "delete from " + OnFormatTableName(Query.TableName);

			if (Query.Filter != null)
			{
				sql += " where " + OnCreateFilter(Query.Filter, parameters);
			}

			command = new CommandType();
			command.CommandText = sql;

			foreach (Tuple<string, object> parameter in parameters)
			{
				OnSetParameter(command, parameter.Item1, parameter.Item2);
			}

			return command;
		}

		public abstract DbCommand CreateCommand(IDatabaseExists Query);
		public abstract DbCommand CreateCommand(ICreateDatabase Query);
		public abstract DbCommand CreateCommand<RowType>(ITableExists<RowType> Query);
		public abstract DbCommand CreateCommand<RowType>(ICreateTable<RowType> Query);
		public abstract DbCommand CreateCommand<RowType>(ICreateColumn<RowType> Query);
		public abstract DbCommand CreateCommand<PrimaryRowType, ForeignRowType>(ICreateRelation<PrimaryRowType, ForeignRowType> Query);
	}
}

	

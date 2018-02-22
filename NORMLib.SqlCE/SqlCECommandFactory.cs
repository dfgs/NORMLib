using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib.SqlCE
{
	public class SqlCECommandFactory : CommandFactory<SqlCeCommand>
	{
		protected override string OnFormatColumnName(IColumn Column)
		{
			return $"[{Column.Name}]";
		}
		protected override string OnFormatTableName(string TableName)
		{
			return $"[{TableName}]";
		}

		protected override void OnSetParameter(SqlCeCommand Command, string Name, object Value)
		{
			Command.Parameters.AddWithValue(Name, Value);
		}

		private string QuoteDefaultValue(IColumn Column)
		{
			Type type;

			type = Column.ColumnType;
			if (type.IsGenericType) type = type.GenericTypeArguments[0];

			switch (type.Name)
			{
				case "String":
					return $"'{Column.DefaultValue}'";
				case "DateTime":
					return $"'{Column.DefaultValue}'";
				case "TimeSpan":
					return $"'{Column.DefaultValue}'";
				default:
					return Column.DefaultValue.ToString();
			}
		}

		private string GetTypeName(IColumn Column)
		{
			string result;
			Type type;

			type = Column.ColumnType;
			if (type.IsGenericType) type = type.GenericTypeArguments[0];

			switch (type.Name)
			{
				case "String":
					result = "nvarchar(4000)";
					break;
				case "Byte":
					result = "tinyint";
					break;
				case "Single":
					result = "real";
					break;
				case "Int32":
					result = "int";
					break;
				case "Int64":
					result = "bigint";
					break;
				case "Boolean":
					result = "bit";
					break;
				case "DateTime":
					result = "datetime";
					break;
				case "TimeSpan":
					result = "time";
					break;
				default: throw (new NotImplementedException("Cannot convert CLR type " + Column.ColumnType.Name));

			}
			return result;
		}




		public override DbCommand CreateCommand(IDatabaseExists Query)
		{
			return new SqlCeCommand("SELECT 1");
		}

		public override DbCommand CreateCommand(ICreateDatabase Query)
		{
			throw (new NotImplementedException());
		}

		public override DbCommand CreateCommand<RowType>(ITableExists<RowType> Query)
		{
			SqlCeCommand command = new SqlCeCommand("SELECT table_name FROM information_schema.tables where table_name=@Name");
			command.Parameters.AddWithValue("@Name", Query.TableName);
			return command;
		}

		public override DbCommand CreateCommand<RowType>(ICreateTable<RowType> Query)
		{
			string sql;

			sql = "CREATE TABLE " + OnFormatTableName(Query.TableName) + " (" + string.Join(",", Query.Columns.Select(column =>
			{
				return $"{OnFormatColumnName(column)} {GetTypeName(column)}{(column.IsNullable ? " NULL" : " NOT NULL")}{(column.IsIdentity ? " IDENTITY" : "")}{(column.DefaultValue == null ? "" : $" default {QuoteDefaultValue(column)}")}";
			})) + $",PRIMARY KEY ({Query.PrimaryKey.Name}))";


			return new SqlCeCommand(sql);
		}

		public override DbCommand CreateCommand<RowType>(ICreateColumn<RowType> Query)
		{
			return new SqlCeCommand($"ALTER TABLE {Query.TableName} ADD COLUMN ({OnFormatColumnName(Query.Column)} {GetTypeName(Query.Column)} {(Query.Column.IsNullable ? " NULL" : " NOT NULL")} {(Query.Column.IsIdentity ? " IDENTITY" : "")}{(Query.Column.DefaultValue == null ? "" : $" default {QuoteDefaultValue(Query.Column)}")})");
		}

		public override DbCommand CreateCommand<PrimaryRowType, ForeignRowType, ValueType>(ICreateRelation<PrimaryRowType, ForeignRowType, ValueType> Query)
		{
			return new SqlCeCommand($"ALTER TABLE {OnFormatTableName(Query.ForeignTableName)} ADD CONSTRAINT {Query.PrimaryTableName}To{Query.ForeignTableName} FOREIGN KEY ({OnFormatColumnName(Query.ForeignColumn)}) REFERENCES {OnFormatTableName(Query.PrimaryTableName)}  ( {OnFormatColumnName(Query.PrimaryColumn)})");
		}

		public override DbCommand CreateCommand(ISelectIdentity Query)
		{
			return new SqlCeCommand("select @@identity");
		}
	}
}

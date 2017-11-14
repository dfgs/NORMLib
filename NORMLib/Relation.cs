using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NORMLib
{
	public class Relation<PrimaryRowType,ForeignRowType,ValueType> : IRelation<ValueType>
	{
		private static Regex nameRegex = new Regex(@"^(.*)Table$");

		private string name;
		public string Name
		{
			get { return name; }
		}

		private DeleteReferentialAction deleteReferentialAction;
		public DeleteReferentialAction DeleteReferentialAction
		{
			get { return deleteReferentialAction; }
		}

		public string PrimaryTable
		{
			get { return Table<PrimaryRowType>.Name; }
		}

		private IColumn<ValueType> primaryColumn;
		public IColumn<ValueType> PrimaryColumn
		{
			get { return primaryColumn; }
		}

		IColumn IRelation.PrimaryColumn
		{
			get { return primaryColumn; }
		}

		public string ForeignTable
		{
			get { return Table<ForeignRowType>.Name; }
		}

		private IColumn<ValueType> foreignColumn;
		public IColumn<ValueType> ForeignColumn
		{
			get { return foreignColumn; }
		}

		IColumn IRelation.ForeignColumn
		{
			get { return foreignColumn; }
		}


		public Relation(IColumn<ValueType> PrimaryColumn, IColumn<ValueType> ForeignColumn, DeleteReferentialAction DeleteReferentialAction = DeleteReferentialAction.Delete, [CallerMemberName]string Name = null)
		{
			Match match;

			match = nameRegex.Match(Name);
			if (match.Success) name = match.Groups[1].Value;
			else name = Name;
			this.deleteReferentialAction = DeleteReferentialAction;
			this.primaryColumn = PrimaryColumn; this.foreignColumn = ForeignColumn;
		}

	}

}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	
	public abstract class Filter
	{

		public AndFilter And(Filter Other)
		{
			AndFilter otherAnd;
			AndFilter thisAnd;
			List<Filter> items;

			items = new List<Filter>();
			thisAnd = this as AndFilter;
			otherAnd = Other as AndFilter;

			if (thisAnd != null) items.AddRange(thisAnd.Filters);
			else items.Add(this);

			if (otherAnd != null) items.AddRange(otherAnd.Filters);
			else items.Add(Other);

			return new AndFilter(items.ToArray());
		}

		public OrFilter Or(Filter Other)
		{
			OrFilter otherOr;
			OrFilter thisOr;
			List<Filter> items;

			items = new List<Filter>();
			thisOr = this as OrFilter;
			otherOr = Other as OrFilter;

			if (thisOr != null) items.AddRange(thisOr.Filters);
			else items.Add(this);

			if (otherOr != null) items.AddRange(otherOr.Filters);
			else items.Add(Other);

			return new OrFilter(items.ToArray());
		}


	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public interface IDelete<RowType> : IFilterQuery<RowType>
	{
		/*RowType Item
		{
			get;
		}*/

		

		IDelete<RowType> Where(Filter Filter);
	}
}

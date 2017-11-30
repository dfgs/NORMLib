﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public interface IFilterQuery<RowType>:ITableQuery<RowType>
	{
		
		Filter Filter
		{
			get;
		}

	}
}

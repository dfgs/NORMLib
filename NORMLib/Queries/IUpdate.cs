﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NORMLib
{
	public interface IUpdate<RowType> : IFilterQuery<RowType>
	{
		
		RowType Item
		{
			get;
		}
	}
}

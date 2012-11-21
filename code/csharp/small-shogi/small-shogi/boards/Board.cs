using System;
using System.Collections.Generic;

namespace smallshogi
{
	public abstract class Board
	{
		protected int files, columns;

		public Board ()
		{
		}

		public abstract int gamePosition();

		public abstract List<Board> children();
	}
}


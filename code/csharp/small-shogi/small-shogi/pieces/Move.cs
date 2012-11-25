using System;
using System.Collections;

namespace smallshogi
{
	public class Move
	{
		public int f, c;
        public bool isRanged;

		public Move (int f, int c)
		{
            this.f = f;
            this.c = c;
		}
	}
}


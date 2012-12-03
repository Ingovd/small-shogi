using System;
using System.Collections;

namespace smallshogi
{
	public class Move
	{
		public int c, f;
        public bool isRanged;

		public Move (int c, int f, bool isRanged = false)
		{
			this.c = c;
            this.f = f;
			this.isRanged = isRanged;
		}

		public Move switchSide()
		{
            return new Move(c * -1, f * -1, isRanged);
		}
	}
}


using System;
using System.Collections.Generic;
using System.Collections;

namespace smallshogi
{
	public class Board3x4 : Board
	{
		UInt16 wPawn, wBish, wRook, wKing,
		bPawn, bBish, bRook, bKing;


		public Board3x4 ()
		{
			files = 4;
			columns = 3;

			wPawn = 128;
			wBish = 512;
			wRook = 2048;
			wKing = 1024;
			bPawn = 16;
			bBish = 4;
			bRook = 1;
			bKing = 2;
		}

		public string show ()
		{
			return "Nothing";
		}

		public override int gamePosition ()
		{
			return -1;
		}

		public override List<Board> children ()
		{
			return new List<Board>();
		}
	}
}


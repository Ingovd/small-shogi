using System;
using System.Collections.Generic;

namespace smallshogi
{
	public enum Type
	{
		King, Gold, Silver, Knight, Lance, Pawn, Rook, Bishop, None,
		PSilver, PKnight, PLance, Tokin, PRook, PBishop
	};

	public class Piece
	{
		private List<Move> moves;
		private List<Move> pmoves;


		public Type type;
		public Type ptype;
		public bool isRanged;

		public Piece (bool isRanged, List<Move> moves, Type type,
		              List<Move> pmoves, Type ptype = Type.None)
		{
			this.moves = moves;
			this.isRanged = isRanged;
			this.type = type;
			this.pmoves = pmoves;
			this.ptype = ptype;
		}

		//public List<BitArray>
	}
}


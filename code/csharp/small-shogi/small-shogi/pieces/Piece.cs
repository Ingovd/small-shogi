using System;
using System.Collections;
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

		private bool ranged;
        private bool pranged;

		public Piece (bool isRanged, List<Move> moves, Type type,
		              List<Move> pmoves = null, Type ptype = Type.None)
		{
			this.moves = moves;
			this.type = type;
			this.pmoves = pmoves;
			this.ptype = ptype;

            foreach (Move m in moves)
                ranged = ranged || m.isRanged;
            if (pmoves == null)
                pmoves = new List<Move>();
            foreach (Move m in pmoves)
                pranged = pranged || m.isRanged;
		}

        public bool isRanged(bool b)
        {
            return (!b && ranged) || (b && pranged);
        }

        public Dictionary<BitArray, BitArray> generateMoves(int files, int columns)
        {
            var dic = new Dictionary<BitArray, BitArray>();
            for (int i = 0; i < files; ++i)
                for (int j = 0; j < columns; ++j)
                {
                    var pos = new BitArray(files * columns);

                }
            return dic;
        }
	}
}


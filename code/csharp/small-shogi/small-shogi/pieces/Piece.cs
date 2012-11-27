using System;
using System.Collections;
using System.Collections.Generic;

namespace smallshogi
{
	public enum Type
	{
		King,
		Gold,
		Silver,
		Knight,
		Lance,
		Pawn,
		Rook,
		Bishop,
		None,
		PSilver,
		PKnight,
		PLance,
		Tokin,
		PRook,
		PBishop
	}
	;

	public class Piece
	{
		public Type type;
		public Type ptype;
		private List<Move> moves;
		private List<Move> pmoves;
		private bool ranged = false;
		private bool pranged = false;

		public Piece (List<Move> moves, Type type,
		              List<Move> pmoves = null, Type ptype = Type.None)
		{
			this.moves = moves;
			this.type = type;
			this.pmoves = pmoves;
			this.ptype = ptype;

			// Flag if this piece has ranged moves
			foreach (Move m in moves)
				ranged = ranged || m.isRanged;
			// Flag if this piece has ranged moves after promoption
			if (pmoves != null)
				foreach (Move m in pmoves)
					pranged = pranged || m.isRanged;
		}

		public bool isRanged (bool b)
		{
			return (!b && ranged) || (b && pranged);
		}

		public Dictionary<BitBoard, BitBoard> generateMoves (int files, int columns, bool p)
		{
			var dic = new Dictionary<BitBoard, BitBoard> ();
			List<Move> selectedMoves = p ? pmoves : moves;
			if(selectedMoves == null)
				return null;

			for (int j = 0; j < files; ++j)
				for (int i = 0; i < columns; ++i) {
				var position = new BitBoard ();
				position.Set (i + j * columns);
					var moveBoard = new BitBoard ();
					foreach (Move m in selectedMoves) {
						if (i + m.c < columns &&
							i + m.c >= 0 &&
							j + m.f < files &&
							j + m.f >= 0)
							moveBoard.Set (i + m.c + (j + m.f) * columns);
					}
					dic.Add (position, moveBoard);
				}
			dic.Add (new BitBoard(), new BitBoard());
			return dic;
		}
	}
}


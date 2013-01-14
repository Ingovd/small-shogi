using System;
using System.Collections;
using System.Collections.Generic;

namespace smallshogi
{
    using Bits = System.UInt32;
    using B = BitBoard;

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

		// Check if this piece is ranged if it is (false) unpromoted or (true) promoted
		public bool isRanged (bool b)
		{
			return (!b && ranged) || (b && pranged);
		}

		public void switchSide ()
		{
            var switchedMoves = new List<Move>();
			foreach (var m in moves)
				switchedMoves.Add(m.switchSide());
            moves = switchedMoves;
            if (pmoves != null)
            {
                var switchedPMoves = new List<Move>();
                foreach (var m in pmoves)
                    switchedPMoves.Add(m.switchSide());
                pmoves = switchedPMoves;
            }
		}

		public Dictionary<Bits, Bits> generateMoves (int files, int columns, bool p)
		{
			var dic = new Dictionary<Bits, Bits> ();
			List<Move> selectedMoves = p ? pmoves : moves;
			if (selectedMoves == null)
				return null;

			for (int j = 0; j < files; ++j)
				for (int i = 0; i < columns; ++i) {
					Bits position = 0;
					B.Set (ref position, i + j * columns);
					Bits moveBoard = 0;
					foreach (Move m in selectedMoves) {
						if (i + m.c < columns &&
							i + m.c >= 0 &&
							j + m.f < files &&
							j + m.f >= 0)
							B.Set (ref moveBoard, i + m.c + (j + m.f) * columns);
					}
				/*Console.WriteLine(Piece.showType (p ? ptype : type));
                Console.WriteLine(B.ToString(position, 3, 12));
				Console.WriteLine("---");
                Console.WriteLine(B.ToString(moveBoard, 3, 12));*/
                    dic.Add(position, moveBoard);
				}
			dic.Add (0, 0);
			return dic;
		}

		public static string showType (Type t)
		{
			switch (t) {
			case		Type.King:
				return "k";
			case		Type.Gold:
				return "g";
			case		Type.Silver:
				return "s";
			case		Type.Knight:
				return "n";
			case		Type.Lance:
				return "l";
			case		Type.Pawn:
				return "p";
			case		Type.Rook:
				return "r";
			case		Type.Bishop:
				return "b";
			case		Type.None:
				return " ";
			case		Type.PSilver:
				return "S";
			case		Type.PKnight:
				return "K";
			case		Type.PLance:
				return "L";
			case		Type.Tokin:
				return "T";
			case		Type.PRook:
				return "r";
			case		Type.PBishop:
				return "B";
			default:
				return "?";
			}
		}
	}
}

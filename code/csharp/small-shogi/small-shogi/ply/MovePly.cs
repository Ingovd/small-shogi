using System;

namespace smallshogi
{
	public class MovePly : Ply
	{
		// Index piece moved and piece captured
		int mI, cI;
		// A bitboard for moving, promoting and capturing
		BitBoard piece, promo, capt;
		public MovePly (int c, int mI, BitBoard piece)
		{
			this.c = c;
			this.mI = mI;
			this.piece = piece;
			this.promo = new BitBoard();

			// Default value
			cI = -1;
		}

		public override void apply (BitBoard[] position)
		{
			// Move the piece in question
			position [pieceI  (c, mI)].And (piece);
			// Possibly promote it
			position [piecePI (c, mI)].And (promo);
			// If a piece is captured update hand and enemy piece
			if (cI >= 0) {
				position [pieceEI (c, cI)].And (capt);
				var mask = new BitBoard(Game.handMask[Game.demote[cI]]);
				position[handI(c)].PushMasked(mask);
			}
		}
	}
}


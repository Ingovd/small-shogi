using System;

namespace smallshogi
{
	public class MovePly : Ply
	{
		// Index piece moved and piece captured
		int mI, cI;
		// Bool indicating whether this move leads to a promotion
		bool promo = false;
		// A bitboard for initial square and moved square
		BitBoard square, move;
		public MovePly (int c, int mI, BitBoard square, BitBoard move)
		{
			this.c = c;
			this.mI = mI;
			this.square = square;
			this.move = move;

			// Default value
			cI = -1;
		}

		public MovePly branchPromotion ()
		{
			var promoPly = new MovePly(c, mI, square, move);
			promoPly.setPromotion();
			return promoPly;
		}

		public void setPromotion() {
			this.promo = true;
		}

		public void setCaptureIndex (int cI)
		{
			this.cI = cI;
		}

		public override BitBoard[] apply (BitBoard[] position)
		{
			var result = new BitBoard[position.Length];
			Array.Copy (position, result, position.Length);
			// Remove the piece from initial position
			var pI = pieceI (c, mI);
			result [pI] = new BitBoard (result [pI]);
			result [pI].Xor (square);
			// Put it on new position, possibly promoting
			if (promo) {
				var pPI = piecePI (c, mI);
				result [pPI] = new BitBoard(result [pPI]);
				result [pPI].Xor (move);
			}
			else
				result [pieceI (c, mI)].Xor (move);
			// If a piece is captured update hand and enemy piece
			if (cI >= 0) {
				// Create copies of pieceEI and handI
				var pEI = pieceEI (c, cI);
				result [pEI] = new BitBoard(result [pEI]);
				var hI = handI (c);
				result[hI] = new BitBoard(result[hI]);
				// Update the bitboards
				result [pEI].Xor (move);
				var mask = new BitBoard(Game.handMask[Game.demote[cI]]);
				result[hI].PushMasked(mask);
			}
			return result;
		}

		public override int pieceMoved ()
		{
			return mI;
		}
	}
}
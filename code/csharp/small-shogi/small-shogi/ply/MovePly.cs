using System;

namespace smallshogi
{
    using Bits = System.UInt32;
    using B = BitBoard;

	public class MovePly : Ply
	{
		// Index piece moved and piece captured
		int mI, cI;
		// Bool indicating whether this move leads to a promotion
		bool promo = false;
		// A bitboard for initial square and moved square
		Bits square, move;
		public MovePly (int c, int mI, Bits square, Bits move)
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
            promoPly.setCaptureIndex(cI);
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

		public override Bits[] Apply (Bits[] position, Game g)
		{
			var result = new Bits[position.Length];
			Array.Copy (position, result, position.Length);
			// Remove the piece from initial position
			var pI = g.PieceIndex (c, mI);
			result [pI] = result [pI];
            result[pI] ^= square;
			// Put it on new position, possibly promoting
			if (promo) {
				var pPI = g.PromotedIndex (c, mI);
				result [pPI] = result [pPI];
				result [pPI] ^= move;
			}
			else
				result [g.PieceIndex (c, mI)] ^= move;
			// If a piece is captured update hand and enemy piece
			if (cI >= 0) {
				// Create copies of pieceEI and handI
				var pEI = g.PieceIndex (c^1, cI);
				result [pEI] = result [pEI];
				var hI = g.HandIndex (c);
				result[hI] = result[hI];
				// Update the bitboards
				result [pEI] ^= (move);
				var mask = g.handMask[g.demote[cI]];
				B.PushMasked(ref result[hI], mask);
			}
			return result;
		}

		public override int PieceMoved ()
		{
			return mI;
		}
	}
}
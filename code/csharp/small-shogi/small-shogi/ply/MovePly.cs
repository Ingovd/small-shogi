using System;

namespace smallshogi
{
    using Bits = System.UInt16;
    using B = BitBoard;

	public class MovePly : Ply
	{
		// Index piece moved and piece captured
		int movedIndex, capturedIndex;
		// Bool indicating whether this move leads to a promotion
		bool promo = false;
		// A bitboard for initial square and moved square
		Bits square, move;
		public MovePly (int c, int mI, Bits square, Bits move)
		{
			this.c = c;
			this.movedIndex = mI;
			this.square = square;
			this.move = move;

			// Default value
			capturedIndex = -1;
		}

		public MovePly branchPromotion ()
		{
			var promoPly = new MovePly(c, movedIndex, square, move);
            promoPly.setCaptureIndex(capturedIndex);
			promoPly.setPromotion();
			return promoPly;
		}

		public void setPromotion() {
			this.promo = true;
		}

		public void setCaptureIndex (int cI)
		{
			this.capturedIndex = cI;
		}

		public override Bits[] Apply (Bits[] position, Game g)
		{
			var result = new Bits[position.Length];
			Array.Copy (position, result, position.Length);
			// Remove the piece from initial position
			var pI = g.PieceIndex (c, movedIndex);
            result[pI] ^= square;
			// Put it on new position, possibly promoting
			if (promo) {
				var pPI = g.PromotedIndex (c, movedIndex);
				result [pPI] ^= move;
			}
			else
				result [g.PieceIndex (c, movedIndex)] ^= move;
			// If a piece is captured update hand and enemy piece
			if (capturedIndex >= 0) {
				// Get the correct indices
				var pEI = g.PieceIndex (c^1, capturedIndex);
				var hI = g.HandIndex (c);
				// Update the bitboards
				result [pEI] ^= (move);
				var mask = g.handMask[g.demote[capturedIndex]];
				B.PushMasked(ref result[hI], mask);
			}
			return result;
		}

		public override int PieceMoved ()
		{
			return movedIndex;
		}
	}
}
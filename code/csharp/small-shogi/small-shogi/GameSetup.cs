using System;
using System.Collections.Generic;

namespace smallshogi
{
	public class GameSetup
	{
		public int files, columns, promo;
		public Dictionary<int, Type> white = new Dictionary<int, Type> (),
		  	                         black = new Dictionary<int, Type> ();
		List<Type> pieces = new List<Type> ();

		public GameSetup (int files, int columns)
		{
			this.files = files;
			this.columns = columns;
		}

		public GameSetup (int files, int columns, int promo)
		{
			this.files = files;
			this.columns = columns;
			this.promo = promo;
		}

		/*
		 * Adds a piece of type type on column x and file y with (0,0) upper left corner.
		 */ 
		public void AddWhitePiece(int x, int y, Type type)
		{
			white[y*columns + x] = type;
			AddType(type);
		}

		public void AddBlackPiece(int x, int y, Type type)
		{
			black[y*columns + x] = type;
			AddType(type);
		}

		void AddType (Type type)
		{
			if(!pieces.Contains(type))
				pieces.Add(type);
		}

		public void SetPromotionRanks(int r)
		{
			promo = r;
		}

		public Piece[] Pieces ()
		{
			Piece[] pieceObjects = new Piece[pieces.Count];
			for(int i = 0; i < pieces.Count; ++i)
				pieceObjects[i] = Piece.getObject[pieces[i]];
			return pieceObjects;
		}

		static Dictionary<int, Type> type = new Dictionary<int, Type>
		{
			{0, Type.Bishop},
			{1, Type.Rook},
			{2, Type.Pawn},
			{3, Type.Gold},
			{4, Type.Silver},
			{5, Type.Knight}
		};

		public GameSetup (int seed)
		{
			// Initiate the basic setup information
			files = 3; columns = 3; promo = 1;

			// Decode seed
			int[] t  = new int[3];
			seed     = seed % 1372;
			int king = seed / 343;
			int rest = seed % 343;
			t[0]     = rest / 49;
			rest     = rest % 49;
			t[1]     = rest / 7;
			t[2]     = rest % 7;

			// Add all types involved
			AddType (Type.King);
			for(int i = 0; i < 3; ++i)
				if(t[i] != 6)
					AddType (type[t[i]]);

			// Add white and black pieces;
			white[king] = Type.King;
			for(int i = 0; i < 3; ++i)
				if(t[i] != 6)
					white[king<=i?i+1:i] = type[t[i]];

			black[8-king] = Type.King;
			for(int i = 0; i < 3; ++i)
				if(t[i] != 6)
					black[8-(king<=i?i+1:i) ] = type[t[i]];
		}
	}
}


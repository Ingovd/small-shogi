using System;
using System.Collections;
using System.Collections.Generic;

namespace smallshogi
{
    using Bits = System.UInt16;
    using B = BitBoard;

	public class Game
	{
		// Size of game board
		int files, columns;
		// Initial setup
		public Bits[] startingPos;
		// A list of pieces used for move generation
		Piece[] pieces; 
		// Map from piece type to piece index
		public Dictionary<Type, int> index;
		// Attack boards for pieces
        public Dictionary<int, Dictionary<Bits, Bits>> moveSets;
		// Number of movesets (pieces + promoted pieces)
		public int l;
		// Map from piece moveset index to hand mask
        public Dictionary<int, Bits> handMask;
		// Map from piece moveset index to unpromoted moveset index and vv
		public Dictionary<int, int> demote;
		public Dictionary<int, int> promote;
		// Promotion masks for both players
        Bits[] promoMask;

		public Game (Dictionary<int, Type> white, Dictionary<int, Type> black,
		             int files, int columns, int promo, Piece[] pieces)
		{
			Initialise (white, black, files, columns, promo, pieces);
		}

		public Game (GameSetup setup)
		{
			Initialise(setup.white, setup.black, setup.files, setup.columns, setup.promo, setup.Pieces());
		}

		private void Initialise (Dictionary<int, Type> white, Dictionary<int, Type> black,
		             int files, int columns, int promo, Piece[] pieces)
		{
			this.files = files;
			this.columns = columns;
			this.pieces = pieces;

			generateInitialSetup (white, black);
			generateMoveSets ();

			// Create the masks for both player's promotion zones
            Bits whitePromo = 0;
            Bits blackPromo = 0;
			for (int j = 0; j < promo; ++j) {
				for (int i = 0; i < columns; ++i) {
					B.Set (ref whitePromo, (files - 1 - j) * columns + i);
					B.Set (ref blackPromo, j * columns + i);
				}
			}
            promoMask = new Bits[2];
			promoMask [0] = whitePromo;
			promoMask [1] = blackPromo;
		}

		private void generateInitialSetup (Dictionary<int, Type> white, Dictionary<int, Type> black)
		{
			// Find out which pieces can be promoted and assign an index to every piece type
			index = new Dictionary<Type, int> ();
			demote = new Dictionary<int, int> ();
			promote = new Dictionary<int, int> ();
			int promotedPieces = 0;
			for (var p = 0; p < pieces.Length; ++p) {
				index.Add (pieces [p].type, p);
				demote.Add (p, p);
				if (pieces [p].ptype != Type.None) {
					var pl = pieces.Length;
					index [pieces [p].ptype] = pl + promotedPieces;
					demote.Add (pl + promotedPieces, p);
					promote.Add (p, pl + promotedPieces);
					promote.Add (pl + promotedPieces, pl + promotedPieces);
					promotedPieces++;
				}
			}
			// Set parameter for easy access
			l = index.Count;

			// Instantiate all the BitBoards for the initial setup
			startingPos = new Bits[l * 2 + 2];
			for (var i = 0; i < l * 2 + 2; ++i)
				startingPos [i] = 0;

			// Keep track of how many pieces of each type are in the game
			int[] pieceCount = new int[pieces.Length];
			// Initiate white pieces
			foreach (KeyValuePair<int, Type> w in white) {
				B.Set (ref startingPos [index [w.Value]], w.Key);
				pieceCount [index [w.Value]]++;
			}
			// Initiate black pieces
			foreach (KeyValuePair<int, Type> b in black) {
				B.Set (ref startingPos [index [b.Value] + l], b.Key);
				pieceCount [index [b.Value]]++;
			}
			// Create a mask for each piece type used for getting
			// the amount of pieces in hand of that type
			handMask = new Dictionary<int, Bits> ();
			int count = 0;
			for (var p = 0; p < pieces.Length; ++p) {
				Bits b = 0;
				for (var pc = 0; pc < pieceCount[p]; ++pc) {
				    B.Set (ref b, count);
					count++;
				}
				handMask.Add (p, b);
			}
		}

		private void generateMoveSets ()
		{
			// Preprocess the moves for each piece
			moveSets = new Dictionary<int, Dictionary<Bits, Bits>> ();
			for (var i = 0; i < pieces.Length; ++i) {
				var p = pieces [i];
				// Unpromoted moveset white
				moveSets.Add (index [p.type], p.generateMoves (files, columns, false));
				// Promoted moveset white
				if (p.ptype != Type.None)
					moveSets.Add (index [p.ptype], p.generateMoves (files, columns, true));

				// Mirror the moves of this piece
				p.switchSide ();
				// Unpromoted moveset black
				moveSets.Add (index [p.type] + l, p.generateMoves (files, columns, false));
				// Promoted moveset black
				if (p.ptype != Type.None)
					moveSets.Add (index [p.ptype] + l, p.generateMoves (files, columns, true));
				// Reset moves of this piece
				p.switchSide ();
			}
		}

		// Returns a list of all possible plies
		public List<Ply> children (Bits[] position, int c)
		{
			var plies = new List<Ply> ();

			// Calculate all squares not occupied by pieces of colour c
			Bits notCPieces = (Bits)((~colourPieces (position, c)) & ((1 << files*columns) - 1));
			// Calculate all squares occupied by pieces of colour (c ^ 1)
			Bits enemyPieces = colourPieces (position, (c ^ 1));

			// Loop through each piece type
			for (int p = 0; p < l; ++p) {
				// Extract each individual piece
                foreach (Bits square in B.allOnes(position[p + c * l]))
                {
					// Check its moves
					var singleMoves = moves (square, p, c, notCPieces);
					// For each move create a ply
					foreach (Bits move in singleMoves) {
						var ply = new MovePly (c, p, square, move);
                        if (B.Overlaps(move, enemyPieces))
							ply.setCaptureIndex (capture (position, move, c));
						// Add ply without promotion
						plies.Add (ply);
						// Check for promotion and branch if it is possible
						if (p < pieces.Length && pieces [p].ptype != Type.None)
						if (B.Overlaps (promoMask [c], square) || B.Overlaps (promoMask [c], move)) {
							plies.Add (ply.branchPromotion ());
						}
					}
				}
			}
			// Get the players hand information
			Bits hand = position [2 * l + c];
			// If it is not empty calculate empty squares
			if (hand != 0) {
				Bits all = (Bits)(notCPieces ^ (enemyPieces));
				// Loop through all piece types
				foreach (var pieceMask in handMask) {
					// Check if the player has this piece
                    if (B.Overlaps(hand, pieceMask.Value))
						// Calculate all positions where it can be dropped
						foreach (Bits square in B.allOnes(all)) {
							// Add a drop ply for each possible drop
							plies.Add (new DropPly (c, pieceMask.Key, square));
					}
				}
			}
			// Return all possible plies
			return plies;
		}

		public List<Bits> moves (Bits square, int p, int c, Bits notCPieces)
		{
			Bits possibleMoves;
			if (pieces [demote [p]].isRanged (p >= pieces.Length))
				// Do some shit for ranged pieces
				possibleMoves = 0;
			else
				// Get the dictionary for the correct piece type
				possibleMoves = moveSets [p + c * l][square];

			// Eliminate squares occupied by the same colour
			possibleMoves &= notCPieces;

            return B.allOnes(possibleMoves);
		}

		public Bits colourPieces (Bits[] position, int c)
		{
			Bits allPieces = 0;
			// Loop through all pieces of colour c
			var colour = c * l;
			for (var i = 0; i < l; ++i)
				allPieces |= position [i + colour];
			return allPieces;
		}

		public int capture (Bits[] position, Bits move, int c)
		{
			var e = (c ^ 1) * l;
			for (int p = 0; p < l; ++p)
				if (B.Overlaps (position [p + e], move))
					return p;
			// Unvalid call to capture
			System.Console.WriteLine ("Warning: unvalid call to capture.");
			return -1;
		}

		// Returns -1 if this is not a terminal position, 0, 1 or 2 for draw, white win, black win
		/*public int gamePosition (Bits[] position)
		{
			var kingIndex = index [Type.King];
			if (position [kingIndex] == 0)
				return 2;
			if (position [kingIndex + l] == 0)
				return 1;
			return -1;
		}*/

		// Returns -1 if this is not a terminal position, 0, 1 or 2 for draw, white win, black win
		public int gamePosition (Bits[] position, int c)
		{
			var kingIndex = index [Type.King];
			// Return if either king is missing
			if (position [kingIndex] == 0)
				return 2;
			if (position [kingIndex + l] == 0)
				return 1;

			// Otherwise check if the moving player can capture the enemy king
			Bits attacks = 0;
			for (int p = 0; p < l; ++p) {
				foreach (Bits square in B.allOnes(position[p + c*l])) {
					attacks |= (moveSets [p] [square]);
				}
			}

			if (B.Subset (position [kingIndex + (c ^ 1) * l], attacks)) {
				return 1 + c;
			}

            if (B.Subset(position[kingIndex], promoMask[0]))
                return 1;
            if (B.Subset(position[kingIndex + l], promoMask[1]))
                return 2;

			// This position is not terminal
			return -1;
		}

		public bool SamePosition (Bits[] p1, Bits[] p2)
		{
			if (p1.Length != p2.Length)
				return false;
			for (int i = 0; i < p1.Length; ++i) {
				if (!p1.Equals (p2))
					return false;
			}
			return true;
		}

		public int PromotedIndex (int c, int i)
		{
			return promote[i] + c * l;
		}

		public int HandIndex (int c)
		{
			return 2 * l + c;
		}

		public int PieceIndex (int c, int i)
		{
			return i + c * l;
		}

		public string prettyPrint (Bits[] position)
		{
			var map = new Dictionary<Bits, int> ();
			for (int i = 0; i < files*columns; ++i) {
				Bits b = 0;
				B.Set (ref b, i);
				map.Add (b, i);
			}
			string[] whites = new string[files * columns];
			string[] blacks = new string[files * columns];
			for (int i = 0; i < l; ++i) {
                foreach (var b in B.allOnes(position[i]))
					whites [map [b]] = Piece.showType (i < pieces.Length ? pieces [i].type : pieces [demote [i]].ptype);
                foreach (var b in B.allOnes(position[i + l]))
					blacks [map [b]] = Piece.showType (i < pieces.Length ? pieces [i].type : pieces [demote [i]].ptype);
			}
			string s = "";

			Bits hand = position [2 * l];
			if (hand != 0) {
				foreach (var pieceMask in handMask) {
					hand = position [2 * l];
                    foreach (var p in B.allOnes((Bits)(hand & pieceMask.Value)))
						s += Piece.showType (pieces [pieceMask.Key].type);
				}
			}
			s += "\n";

			for (int f = 0; f < files; ++f) {
				for (int c = 0; c < columns; ++c)
					s += "+--";
				s += "+\n";
				for (int c = 0; c < columns; ++c) {
					s += "|";
					s += whites [c + f * columns] != null ? whites [c + f * columns] : " ";
					s += blacks [c + f * columns] != null ? blacks [c + f * columns] : " ";
				}
				s += "|\n";
			}
			for (int c = 0; c < columns; ++c)
				s += "+--";
			s += "+\n";

			hand = position [2 * l + 1];
			if (hand != 0) {
				foreach (var pieceMask in handMask) {
					hand = position [2 * l + 1];
                    foreach (var p in B.allOnes((Bits)(hand & pieceMask.Value)))
						s += Piece.showType (pieces [pieceMask.Key].type);
				}
			}
			s += "\n";

			return s;
		}

        static void DisplayBits(Bits bits)
        {
            for (int i = 0; i < 32; i++)
            {
				if(i % 3 == 0 && i != 0)
					Console.WriteLine();
                bool bit = B.Get(bits, i);
                Console.Write(bit ? 1 : 0);
            }
            Console.WriteLine();
        }
	}
}


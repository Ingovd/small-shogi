using System;
using System.Collections;
using System.Collections.Generic;

namespace smallshogi
{
	public class Game
	{
		// Size of game board
		int files, columns;
		// Initial setup
		public BitBoard[] startingPos;
		// A list of pieces used for move generation
		Piece[] pieces; 
		// Attack boards for pieces
		public static Dictionary<int, Dictionary<BitBoard, BitBoard>> moveSets;
		// Number of movesets (pieces + promoted pieces)
		public static int l;
		// Map from piece moveset index to hand mask
		public static Dictionary<int, BitBoard> handMask;
		// Map from piece moveset index to unpromoted moveset index and vv
		public static Dictionary<int, int> demote;
		public static Dictionary<int, int> promote;
		// Promotion masks for both players
		BitBoard[] promoMask;

		public Game (Dictionary<int, Type> white, Dictionary<int, Type> black,
		             int files, int columns, int promo, Piece[] pieces)
		{
			this.files = files;
			this.columns = columns;
			this.pieces = pieces;

			var index = generateInitialSetup (white, black);
			generateMoveSets (index);

			// Create the masks for both player's promotion zones
			var whitePromo = new BitBoard ();
			var blackPromo = new BitBoard ();
			for (int j = 0; j < promo; ++j) {
				for (int i = 0; i < columns; ++i) {
					whitePromo.Set ((files - 1 - j) * columns + i);
					blackPromo.Set (j * columns + i);
				}
			}
			promoMask = new BitBoard[2];
			promoMask [0] = whitePromo;
			promoMask [1] = blackPromo;
		}

		private Dictionary<Type, int> generateInitialSetup
			(Dictionary<int, Type> white, Dictionary<int, Type> black)
		{
			// Find out which pieces can be promoted and assign an index to every piece type
			var index = new Dictionary<Type, int> ();
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
			// Set global parameter for easy access
			l = index.Count;

			// Instantiate all the BitBoards for the initial setup
			startingPos = new BitBoard[l * 2 + 2];
			for (var i = 0; i < l * 2 + 2; ++i)
				startingPos [i] = new BitBoard ();

			// Keep track of how many pieces of each type are in the game
			int[] pieceCount = new int[pieces.Length];
			// Initiate white pieces
			foreach (KeyValuePair<int, Type> w in white) {
				startingPos [index [w.Value]].Set (w.Key);
				pieceCount [index [w.Value]]++;
			}
			// Initiate black pieces
			foreach (KeyValuePair<int, Type> b in black) {
				startingPos [index [b.Value] + l].Set (b.Key);
				pieceCount [index [b.Value]]++;
			}
			// Create a mask for each piece type used for getting
			// the amount of pieces in hand of that type
			handMask = new Dictionary<int, BitBoard> ();
			int count = 0;
			for (var p = 0; p < pieces.Length; ++p) {
				BitBoard b = new BitBoard ();
				for (var pc = 0; pc < pieceCount[p]; ++pc) {
					b.Set (count);
					count++;
				}
				handMask.Add (p, b);
			}
			// Return the map from piece Type to index
			return index;
		}

		private void generateMoveSets (Dictionary<Type, int> index)
		{
			// Preprocess the moves for each piece
			moveSets = new Dictionary<int, Dictionary<BitBoard, BitBoard>> ();
			for (var i = 0; i < pieces.Length; ++i) {
				var p = pieces [i];
				// Unpromoted moveset white
				moveSets.Add (index [p.type], p.generateMoves (files, columns, false));
				// Promoted moveset white
				if (p.ptype != Type.None)
					moveSets.Add (index [p.ptype], p.generateMoves (files, columns, true));
				p.switchSide ();
				// Unpromoted moveset black
				moveSets.Add (index [p.type] + l, p.generateMoves (files, columns, false));
				// Promoted moveset black
				if (p.ptype != Type.None)
					moveSets.Add (index [p.ptype] + l, p.generateMoves (files, columns, true));
			}
		}

		// Returns a BitBoard for every legal move including captures, excluding promotion
		public List<Ply> children (BitBoard[] position, int c)
		{
			var plies = new List<Ply> ();

			// Calculate all squares not occupied by pieces of colour c
			var notCPieces = colourPieces (position, c).Not ();
			// Calculate all squares occupied by pieces of colour (c ^ 1)
			var enemyPieces = colourPieces (position, (c ^ 1));

			// Loop through each piece type
			for (int p = 0; p < l; ++p) {
				// Extract each individual piece
				foreach (BitBoard square in position[p + c * l].allOnes()) {
					// Check its moves
					var singleMoves = moves (square, p, c, notCPieces);
					// For each move create a ply
					foreach (BitBoard move in singleMoves) {
						var ply = new MovePly (c, p, square, move);
						if (move.Overlaps (enemyPieces))
							ply.setCaptureIndex (capture (position, move, c));
						// Add ply without promotion
						plies.Add (ply);
						// Check for promotion and branch if it is possible
						if (p < pieces.Length && pieces [p].type != Type.None)
						if (promoMask [c].Overlaps (square) || promoMask [c].Overlaps (move)) {
							plies.Add (ply.branchPromotion ());
						}
					}
				}
			}
			// Get the players hand information
			var hand = new BitBoard (position [2 * l + c]);
			// If it is not empty calculate empty squares
			if (!hand.NotEmpty ()) {
				var all = notCPieces.Xor (enemyPieces);
				// Loop through all piece types
				foreach (var pieceInHand in handMask)
					// Check if the player has this piece
					if (hand.Overlaps (pieceInHand.Value))
						// Calculate all positions where it can be dropped
						foreach (BitBoard square in hand.And (all).allOnes())
							// Add a drop ply for each possible drop
							plies.Add (new DropPly (c, pieceInHand.Key, square));
			}
			// Return all possible plies
			return plies;
		}

		public List<BitBoard> moves (BitBoard square, int p, int c, BitBoard notCPieces)
		{
			BitBoard possibleMoves;
			if (pieces [p].isRanged (p >= pieces.Length))
				// Do some shit for ranged pieces
				possibleMoves = new BitBoard (0);
			else
				// Get the dictionary for the correct piece type
				possibleMoves = moveSets [p + c * l]
					// Get squares attacked by p with colour c
                    [square];

			// Eliminate squares occupied by the same colour
			possibleMoves.And (notCPieces);

			return possibleMoves.allOnes ();
		}

		public BitBoard colourPieces (BitBoard[] position, int c)
		{
			BitBoard allPieces = new BitBoard ();
			// Loop through all pieces of colour c
			var colour = c * l;
			for (var i = 0; i < l; ++i)
				allPieces.Or (position [i + colour]);
			return allPieces;
		}

		public int capture (BitBoard[] position, BitBoard move, int c)
		{
			var e = (c ^ 1) * l;
			for (int p = 0; p < l; ++p)
				if (position [p + e].Overlaps (move))
					return p;
			// Unvalid call to capture
			System.Console.WriteLine ("Warning: unvalid call to capture.");
			return -1;
		}

		// Returns -1 if this is not a terminal position, 0, 1 or 2 for draw, win, lose
		public int gamePosition (BitBoard[] position)
		{
			return -1;
		}

		public string prettyPrint (BitBoard[] position)
		{
			var map = new Dictionary<BitBoard, int> ();
			for (int i = 0; i < files*columns; ++i) {
				var b = new BitBoard ();
				b.Set (i);
				map.Add (b, i);
			}
			string[] ss = new string[files * columns];
			for (int i = 0; i < l; ++i) {
				foreach (var b in position[i].allOnes())
					ss [map [b]] = Piece.showType (i < pieces.Length ? pieces [i].type : pieces [demote [i]].ptype);
				foreach (var b in position[i+l].allOnes())
					ss [map [b]] = Piece.showType (i < pieces.Length ? pieces [i].type : pieces [demote [i]].ptype);
			}
			string s = "";
			for (int i = 0; i < files * columns; i++) {
				if (i % columns == 0 && i != 0)
					s += "\n";
				s += ss [i] != null ? ss [i] : ".";
			}
			s += "\n\n";
			s += position[2*l].ToString(3, 12);
			s += "\n\n";
			s += position[2*l+1].ToString(3, 12);
			return s;
		}
	}
}


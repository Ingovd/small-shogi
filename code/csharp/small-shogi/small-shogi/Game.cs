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

		public Game (Dictionary<int, Type> white, Dictionary<int, Type> black,
		             int files, int columns, Piece[] pieces)
		{
			this.files = files;
			this.columns = columns;
			this.pieces = pieces;

			// Map from piece type to its index in BitBoard[]
			var index = generateInitialSetup (white, black);
			generateMoveSets (index);
			// Make sure everything is ready to run etc.
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
				demote.Add(p, p);
				if (pieces [p].ptype != Type.None) {
					var pl = pieces.Length;
					index [pieces [p].ptype] = pl + promotedPieces;
					demote.Add(pl + promotedPieces, p);
					promote.Add (p, pl + promotedPieces);
					promote.Add (pl + promotedPieces, pl + promotedPieces);
					promotedPieces++;
				}
			}
			// Set global parameter for easy access
			l = index.Count;

			// Instantiate all the BitBoards for the initial setup
			startingPos = new BitBoard[l * 2];
			for (var i = 0; i < l * 2; ++i)
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
			handMask = new Dictionary<int, BitBoard>();
			int count = 0;
			for (var p = 0; p < pieces.Length; ++p) {
				BitBoard b = new BitBoard();
				for (var pc = 0; pc < pieceCount[p]; ++pc) {
					b.Set(count);
					count++;
				}
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
				// Unpromoted moveset
				moveSets.Add (index [p.type], p.generateMoves (files, columns, false));
				// Promoted moveset
				if (p.ptype != Type.None)
					moveSets.Add (index [p.ptype], p.generateMoves (files, columns, true));
			}
		}

		// Returns a BitBoard for every legal move including captures, excluding promotion
		public List<BitBoard> children (BitBoard[] position, int c)
		{
			var children = new List<BitBoard> ();

			// Calculate all squares not occupied by pieces of colour c
			var colourCnot = colourPieces (position, c).Not ();

			// Do moves of player c (0=white, 1=black) for each piece p
			for (int p = 0; p < l; ++p) {
				// This is incorrect
				var singleMoves = moves (position, p, c);
				children.AddRange (singleMoves);

				/* For position[p + c * l] extract all bits (i.e. single piece of type p)
				 * Check where it can move to and whether it captures or not
				 * Branch on promotion
				 * put it in a moveply
				 */
			}
			/* For each unpromoted piece type check the hand if that piece is present
			 * Check where this piece can be dropped (pawn/knight have restrictions)
			 * Put it in a dropply
			 */

			// Return all possible plies

			return children;
		}

		public List<BitBoard> moves (BitBoard[] position, int p, int c)
		{
			BitBoard possibleMoves;
			if (pieces [p].isRanged (p >= pieces.Length))
				// Do some shit for ranged pieces
				possibleMoves = new BitBoard (0);
			else
				// Get the dictionary for the correct piece type
				possibleMoves = moveSets [p]
					// Get squares attacked by p with colour c
                    [position [p + c * l]];

			// Eliminate squares occupied by the same colour
			possibleMoves.And (colourPieces (position, c).Not ());

			return possibleMoves.allOnes ();
		}

		public BitBoard colourPieces (BitBoard[] position, int c)
		{
			BitBoard allPieces = new BitBoard ();
			for (var i = 0; i < l; ++i)
				allPieces.Or (position [i + c * l]);
			return allPieces;
		}

		// Returns -1 if this is not a terminal position, 0, 1 or 2 for draw, win, lose
		public int gamePosition (BitBoard[] position)
		{
			return -1;
		}
	}
}


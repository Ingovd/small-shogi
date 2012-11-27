using System;
using System.Collections;
using System.Collections.Generic;

namespace smallshogi
{
	public class Game
	{
		public BitBoard[] startingPos; // Initial setup
		public Dictionary<int, Dictionary<BitBoard, BitBoard>> moveSets; // Attack boards for pieces
		int files, columns; // Size of game board
		Piece[] pieces; // A list of pieces used for move generation
		Dictionary<Type, int> index; // Map from piece type to its index in Board/BitBoard[]
		int l;

		public Game (Dictionary<int, Type> white, Dictionary<int, Type> black,
		             int files, int columns, Piece[] pieces)
		{
			this.files = files;
			this.columns = columns;
			this.pieces = pieces;

			generateInitialSetup (white, black);
			generateMoveSets ();
			// Make sure everything is ready to run etc.
		}

		private void generateInitialSetup (Dictionary<int, Type> white, Dictionary<int, Type> black)
		{
			index = new Dictionary<Type, int> ();

			int promotedPieces = 0;
			for (int i = 0; i < pieces.Length; ++i) {
				index.Add (pieces [i].type, i);
				if (pieces [i].ptype != Type.None) {
					index [pieces [i].ptype] = pieces.Length + promotedPieces;
					promotedPieces++;
				}
			}

			startingPos = new BitBoard[index.Count * 2];
			for (int i = 0; i < index.Count * 2; ++i)
				startingPos [i] = new BitBoard ();

			foreach (KeyValuePair<int, Type> w in white) {
				startingPos [index [w.Value]].Set (w.Key);
			}
			foreach (KeyValuePair<int, Type> b in black) {
				startingPos [index [b.Value] + index.Count].Set (b.Key);
			}

			l = index.Count;

			// Debug output
//			for (int i = 0; i < startingPos.Length; ++i) {
//				Console.WriteLine ("Position " + i);
//				Console.WriteLine (startingPos[i].toString (3));
//			}
		}

		private void generateMoveSets ()
		{
			moveSets = new Dictionary<int, Dictionary<BitBoard, BitBoard>> ();
			var l = pieces.Length;
			for (int i = 0; i < l; ++i) {
				var p = pieces[i];
				moveSets.Add (index[p.type ] , p.generateMoves (files, columns, false));
				if(p.ptype != Type.None)
					moveSets.Add (index[p.ptype] , p.generateMoves (files, columns, true ));
			}
		}

		// Returns a BitBoard for every legal move including captures, excluding promotion
		public List<BitBoard> children (BitBoard[] position, int c)
		{
			var children = new List<BitBoard> ();

			// Do moves of player c (0=white, 1=black) for each piece p
			for (int p = 0; p < index.Count; ++p) {
				var singleMoves = moves (position, p, c);
				children.AddRange (singleMoves);
			}

			return children;
		}

		public List<BitBoard> moves (BitBoard[] position, int p, int c)
		{
			BitBoard possibleMoves;
			if (pieces [p].isRanged (p < l)) {
                possibleMoves = moveSets[p] // Get the dictionary for the correct piece type
                    [position[p + c * l]]; // Get the correct attacked squares for the position of piece i
			} else
				possibleMoves = new BitBoard (0);// Do some shit for ranged pieces

			// Eliminate squares occupied by the same colour
			possibleMoves.And (colourPieces (position, c).Not ());

			return possibleMoves.allOnes ();
		}

		public BitBoard colourPieces (BitBoard[] position, int c)
		{
			BitBoard allPieces = new BitBoard ();
			for (int i = 0; i < l; ++i)
				allPieces.Or (position [i + c * l]);
			return allPieces;
		}

		// Returns -1 if this is not a terminal position, 0, 1 or 2 for draw, win, lose
		public int gamePosition (BitBoard[] position)
		{
			return -1;
		}
	}


	// Maybe later
	public struct Square
	{
		int file, column;

		public Square (int file, int column)
		{
			this.file = file;
			this.column = column;
		}

		public override int GetHashCode ()
		{
			return file << 16 + column;
		}
	}
}


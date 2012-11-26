using System;
using System.Collections;
using System.Collections.Generic;

namespace smallshogi
{
	public class Game
	{
		public BitArray[] startingPos; // Initial setup
		public Dictionary<int, Dictionary<int, BitArray>> moveSets; // Attack boards for pieces
		int files, columns; // Size of game board
		Piece[] pieces; // A list of pieces used for move generation 

		public Game (Dictionary<int, Type> white, Dictionary<int, Type> black,
		             int files, int columns, Piece[] pieces)
		{
			this.startingPos = startingPos;
			this.files = files;
			this.columns = columns;
			this.pieces = pieces;

			generateInitialSetup (white, black);
			generateMoveSets ();
			// Make sure everything is ready to run etc.
		}

		private void generateInitialSetup (Dictionary<int, Type> white, Dictionary<int, Type> black)
		{

		}

		private void generateMoveSets ()
		{
			moveSets = new Dictionary<int, Dictionary<int, BitArray>> ();
			for (int i = 0; i < pieces.Length; ++i) {
				moveSets.Add (i, pieces [i].generateMoves (files, columns, false));
				moveSets.Add (i + pieces.Length, pieces [i].generateMoves (files, columns, false));
			}
		}

		// Returns a BitArray for every legal move including captures, excluding promotion
		public List<BitArray> children (BitArray[] position, int c)
		{
            var children = new List<BitArray>();

            // Do moves of player c (0=white, 1=black)
            children.AddRange(moves(position, c)); // Unpromoted pieces
            children.AddRange(moves(position, c)); // Promoted pieces
			return children;
		}

        public List<BitArray> moves(BitArray[] position, int c)
        {
            var moves = new List<BitArray>();
            var l = pieces.Length;

            BitArray possibleMoves;
            for (int i = 0; i < pieces.Length; ++i)
            {
                if (!pieces[i].isRanged(false)) {
//					System.Console.WriteLine("Trying to access moveset" + (i+p*l));
//					var a = position[i + c * l + 2 * p * l];
//					var b = firstSet (a);
//					var d = moveSets[i+p*l];
//					var e = d[b];
//                    possibleMoves = moveSets[i + p * l] // Get the dictionary for the correct piece type
//                        [firstSet(position[i + c * l + 2 * p * l])];           // Get the correct attacked squares for the position of piece i
				}
				else
                    possibleMoves = new BitArray(0);// Do some shit for ranged pieces

                // Eliminate squares occupied by the same colour
                possibleMoves = possibleMoves.And(colourPieces(position, c).Not());
                moves.AddRange(allOnes(possibleMoves));
            }
            return moves;
        }

        public BitArray colourPieces(BitArray[] position, int c)
        {
            BitArray allPieces = new BitArray(position[0]);
            for (int i = 1; i < pieces.Length; ++i)
                allPieces.Or(position[i + c * pieces.Length]);
            return allPieces;
        }

        public static List<BitArray> allOnes(BitArray bits)
        {
            var result = new List<BitArray>();
            for (int i = 0; i < bits.Length; ++i)
                if (bits[i])
                {
                    var bit = new BitArray(bits.Length);
                    bit.Set(i, true);
                    result.Add(bit);
                }
            return result;
        }

		public static int firstSet(BitArray bits)
        {
            for (int i = 0; i < bits.Length; ++i)
                if (bits[i])
					return i;
			return bits.Length;
        }

		// Returns -1 if this is not a terminal position, 0, 1 or 2 for draw, win, lose
		public int gamePosition (BitArray[] position)
		{
			return -1;
		}
	}
}


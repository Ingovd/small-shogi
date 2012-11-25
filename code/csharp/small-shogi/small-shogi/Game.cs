using System;
using System.Collections;
using System.Collections.Generic;

namespace smallshogi
{
	public class Game
	{
		BitArray[] startingPos; // Initial setup
		Dictionary<Type, Dictionary<BitArray, BitArray>> moveSets; // Attack boards for pieces
		int files, columns; // Size of game board
		Piece[] pieces; // A list of pieces used for move generation

		public Game (BitArray[] startingPos, int files, int columns, Piece[] pieces)
		{
			this.startingPos = startingPos;
			this.files = files;
			this.columns = columns;
			this.pieces = pieces;

			// Initialise moveSets (read or generate)
			// Make sure everything is ready to run etc.
		}

		private void generateMoveSets ()
		{
            
		}

		// Returns a BitArray for every legal move including captures, excluding promotion
		public List<BitArray> children (BitArray[] position)
		{
            var children = new List<BitArray>();

            // Do white moves
            children.AddRange(moves(position, 0, 0));
            children.AddRange(moves(position, 0, 1));
            // Do black moves
            children.AddRange(moves(position, 1, 0));
            children.AddRange(moves(position, 1, 1));
			return children;
		}

        public List<BitArray> moves(BitArray[] position, int c, int p)
        {
            var moves = new List<BitArray>();
            var l = pieces.Length;

            BitArray possibleMoves;
            for (int i = 0; i < pieces.Length; ++i)
            {
                if (pieces[i].isRanged(false))
                    possibleMoves = moveSets[pieces[i].type + p * l] // Get the dictionary for the correct piece type
                        [position[i + c * l + 2 * p * l]];           // Get the correct attacked squares for the position of piece i
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

        public List<BitArray> allOnes(BitArray bits)
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

		// Returns -1 if this is not a terminal position, 0, 1 or 2 for draw, win, lose
		public int gamePosition (BitArray[] position)
		{
			return -1;
		}
	}
}


using System;
using System.Collections;
using System.Collections.Generic;

namespace smallshogi
{
	public class Game
	{
		BitArray[] startingPos; // Initial setup
		Dictionary<Type, Dictionary<BitArray, BitArray>> moveSets; // Moveboards for pieces
		int files, columns; // Size of game board
		Piece[] pieces; // A list of pieces used for move generation
		// GameTree tree;

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

		// Returns a BitArray for every legal move suitable for XORing
		public List<BitArray> children (BitArray[] position)
		{
			for (int i = 0; i < pieces.Length; ++i) 
				if (pieces[i].isRanged)
					;//return moveSets[pieces[i].type][position[i]];
				else
				;// Do some shit for ranged pieces
			return new List<BitArray>();
		}

		// Returns -1 if this is not a terminal position, 0, 1 or 2 for draw, win, lose
		public int gamePosition (BitArray[] position)
		{
			return -1;
		}
	}
}


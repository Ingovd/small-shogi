using System;
using System.Collections;
using System.Collections.Generic;

namespace smallshogi
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Func<string, string> prependMoreShit = x => "shit" + x;

			Console.WriteLine (prependMoreShit(prependShit("Hello World!")));

            Piece king = new Piece(false, new List<Move>(), Type.King);
            Piece[] pieces = { king };

            Game g = new Game(null, 4, 3, pieces);

            int[] ints = { 17875465 };
            var bit = new BitArray(ints);
            var bit2 = new BitArray(32, true);
            bit = bit.And(bit2);

            DisplayBitArray(bit);
            foreach (BitArray b in g.allOnes(bit))
                DisplayBitArray(b);

            Console.Read();
		}

		public static string prependShit (string s)
		{
			return "shit" + s;
		}

        static void DisplayBitArray(BitArray bitArray)
        {
            for (int i = 0; i < bitArray.Count; i++)
            {
                bool bit = bitArray.Get(i);
                Console.Write(bit ? 1 : 0);
            }
            Console.WriteLine();
        }
	}
}

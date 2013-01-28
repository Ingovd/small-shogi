using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using smallshogi.search;

namespace smallshogi
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			//Game g = new Game (white, black, 4, 3, 1, pieces);
			/*white [ 0] = Type.Bishop;
			white [ 1] = Type.King;
			white [ 2] = Type.Rook;
			//white [ 4] = Type.Pawn;
			black [ 9] = Type.Rook;
			black [10] = Type.King;
			black [11] = Type.Bishop;
			//black [ 7] = Type.Pawn;*/

			/*GameSetup setup = new GameSetup (3, 3);
			setup.SetPromotionRanks (1);
			setup.AddWhitePiece (0, 0, Type.King);
			setup.AddWhitePiece (1, 0, Type.Bishop);
			setup.AddWhitePiece (2, 0, Type.Rook);
			setup.AddBlackPiece (0, 2, Type.Rook);
			setup.AddBlackPiece (1, 2, Type.Bishop);
			setup.AddBlackPiece (2, 2, Type.King);
			Game g = new Game (setup);*/
			
			//GameSetup setup =  new GameSetup(5, 1);
			//setup.SetPromotionRanks(1);
			//setup.AddWhitePiece(0,0, Type.King);
			//setup.AddBlackPiece(5,0, Type.King);
			//Game g = new Game (setup);

			/*int start = 8;

			Game g = new Game(new GameSetup(start));
			PNSearch search = new PNSearch(true);
			Stopwatch sw = new Stopwatch ();
			sw.Start ();
			search.Prove(g);
			sw.Stop ();
			var root = search.root;
			System.Console.WriteLine ("Done expanding in: " + sw.ElapsedMilliseconds + " milliseconds.");
			System.Console.WriteLine ("Number of nodes:   " + search.transposition.Count);
			//System.Console.WriteLine("Tree depth:        " + root.Height());
			System.Console.WriteLine ("Game value:        " + (root.pn==0?1:root.pn==Int32.MaxValue?-1:0));
			var bestGame = root.GetLongestGame();
					foreach(var node in bestGame)
						Console.WriteLine(g.prettyPrint(node.position));



			Game g2;
			BFSearch search2 = new BFSearch ();
			int[] wins = new int[3];
			for (int i = start; i < 1372; i++) {
				g2 = new Game(new GameSetup (i));
				search2.Prove(g2);
				var root2 = search2.root;
				if(g2.gamePosition(root2.position, 1) < 0) {
                    //Console.WriteLine(root2.WinningStrategySize());
					//root2.Browse	(g2);
                   /* var bestGame2 = root2.GetLongestGame(g2);
					foreach(var node in bestGame2)
						Console.WriteLine(g2.prettyPrint(node.position));
					wins[root2.value+1]++;
					Console.WriteLine("Black: {0}\nWhite: {1}\nNone:  {2}",wins[2], wins[0], wins[1]);
				}
			}*/

			for (int i = 0; i < 1372; i++) {
                Search pngraph, pntree, bfs;
                //pngraph = new PNSearch(true, 5);
                pntree = new PNSearch(false, 5);
                //bfs = new BFSearch(5);

                //RunAndDump(pngraph, i);
                RunAndDump(pntree, i);
                //RunAndDump(bfs, i);

                Console.WriteLine("Done with seed " + i);
				}
			}

        public static void RunAndDump(Search s, int seed)
        {
            Game g = new Game(new GameSetup(seed));
            string exception = "";
            try
            {
                s.Prove(g);
            }
            catch (Exception e)
            {
                exception = e.Message;
            }

            List<String> lines = new List<string>();
            lines.Add("Type:" + s.Name());
            lines.Add("Value:" + s.Value());
            lines.Add("Time:" + s.TimeSpent());
            lines.Add("Count:" + s.NodeCount());
            if (exception.Length > 0)
                lines.Add("Exception:" + exception);
            lines.Add("");
            File.AppendAllLines("Data\\" + seed + ".txt", lines);
        }
	}
}

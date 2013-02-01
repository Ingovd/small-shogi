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
			/*var pnt = new PNSearch (false, 10);
			var game = new Game (new GameSetup (128));
			Console.WriteLine(game.prettyPrint(game.startingPos));
			pnt.Prove (game);
			var bestgame = pnt.BestGame ();
			foreach (var pos in bestgame) {
				Console.WriteLine(game.prettyPrint(pos));
			}*/


			AnalyseData ();

			/*GameSetup setup = new GameSetup (4, 3);
			setup.SetPromotionRanks (1);
			setup.AddWhitePiece (0, 0, Type.Bishop);
			setup.AddWhitePiece (1, 0, Type.King);
			setup.AddWhitePiece (2, 0, Type.Rook);
			setup.AddWhitePiece (1, 1, Type.Pawn);
			setup.AddBlackPiece (2, 3, Type.Bishop);
			setup.AddBlackPiece (1, 3, Type.King);
			setup.AddBlackPiece (0, 3, Type.Rook);
			setup.AddBlackPiece (1, 2, Type.Pawn);
			Game g = new Game(setup);
			Console.WriteLine(g.prettyPrint(g.startingPos));
			var pn = new PNSearch (false, 15);
			pn.Prove (g);
			pn.BestGame ();*/


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

			/*for (int i = 0; i < 1372; i++) {
                Search pngraph, pntree, bfs;
                //pngraph = new PNSearch(true, 5);
                pntree = new PNSearch(false, 5);
                //bfs = new BFSearch(5);

                //RunAndDump(pngraph, i);
                //Console.WriteLine("PN on graph done");
                RunAndDump(pntree, i);
                Console.WriteLine("PN on tree done");
                //RunAndDump(bfs, i);
                //Console.WriteLine("BFS on graph done");

                Console.WriteLine("Done with seed " + i);
				}*/
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
            File.AppendAllLines("Data/" + seed + ".txt", lines);
        }

		public static void AnalyseData ()
		{
			List<String> lines = new List<string> ();
			DataReader dr = new DataReader ("/home/ingo/UU/project/Data");
			DataReader dr2 = new DataReader ("/home/ingo/UU/project/Data2");
			dr.ReadAll ();
			dr2.ReadAll ();
			dr.AdHocMergeData (dr2.data);
			//dr.SortData ((first, second) => {
			//	return second.Count ().CompareTo (first.Count ());});
			//dr.SortData((first, second) => {return second.pngraph.count.CompareTo (first.pngraph.count);});
			//dr.SortData((first, second) => {return second.bfs.count.CompareTo (first.bfs.count);});
			dr.SortData((first, second) => {return second.pntree.count.CompareTo (first.pntree.count);});

			dr.FilterData (sc => sc.Count () > 100);
			dr.FilterData(sc => sc.pntree.count > 3000);

			lines.Add ("Total games:     " + dr.data.Count);
			var counts = dr.SolvedGameCount ();
			lines.Add ("PN graph solved: " + counts.Item1); 
			lines.Add ("PN tree solved:  " + counts.Item2); 
			lines.Add ("BFS solved:      " + counts.Item3); 



			var pntn = dr.NodeCount ("pnt");
			var pngn = dr.NodeCount ("png");
			var bfsn = dr.NodeCount ("bfs");

			lines.Add ("PN graph nodes: " + pntn); 
			lines.Add ("PN tree nodes:  " + pngn); 
			lines.Add ("BFS nodes:      " + bfsn); 

			foreach (var line in lines)
				Console.WriteLine (line);

			dr.FilterData (sc => sc.pntree.value == 0);
		}
	}
}

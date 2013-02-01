using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using smallshogi.search;

namespace smallshogi
{
    using Bits = System.UInt16;

	public class BFSearch : Search
	{
        public static Hashtable transposition = new Hashtable();
        static Queue<BNode> queue = new Queue<BNode>();

		public BNode root;
        int timeLimit, timeSpent;

        public BFSearch()
        {

        }

		public BFSearch (int timeLimit)
		{
            this.timeLimit = timeLimit;
		}

        public void Prove(Game g)
        {
			// Clear the transposition table and queue
			transposition.Clear();
            queue.Clear();

			// Evaluate root and add it to transposition table
			root = new BNode(g.startingPos, 1);
			root.Evaluate (g);
			transposition[root] = root;
            queue.Enqueue(root);
			BNode next;
			int count = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (root.value == 0 && queue.Count > 0)
            {
				next = queue.Dequeue();
                next.Expand(g, transposition, queue);
				BNode.InitiateVisiting();
                next.Update(g);

                if (sw.ElapsedMilliseconds > timeLimit * 60000)
                    break;

				/*if(count % 1000 == 0) {
					Console.WriteLine("Value:         " + n.value);
					Console.WriteLine("Transposition: " + BNode.transposition.Count);
					Console.WriteLine("Queue:         " + BNode.queue.Count);
				}*/
				count++;
            }
            sw.Stop();
            timeSpent = (int) sw.ElapsedMilliseconds;

			/*Console.WriteLine((root.value==1?"Black":root.value==-1?"White":"Nobody") + " won!");
			Console.WriteLine("Transposition: " + transposition.Count);
			Console.WriteLine("Queue:         " + queue.Count);*/
        }

        public void SetTimeLimit(int minutes)
        {
            timeLimit = minutes;
        }

        public int Value()
        {
            return root.value;
        }

        public int TimeSpent()
        {
            return timeSpent;
        }

        public int NodeCount()
        {
            return transposition.Count;
        }

        public List<Bits[]> BestGame()
        {
            var chain = root.GetLongestGame();
            List<Bits[]> result = new List<Bits[]>();
            foreach (var node in chain)
                result.Add(node.position);
            return result;
        }

        public String Name()
        {
            return "BFS";
        }
	}
}


using System;
using System.Collections;
using System.Collections.Generic;
using smallshogi.search;

namespace smallshogi
{
	public class BFSearch
	{
        public static Hashtable transposition = new Hashtable();
        static Queue<BNode> queue = new Queue<BNode>();

		public BNode root;

		public BFSearch ()
		{
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
            while (root.value == 0 && queue.Count > 0)
            {
				next = queue.Dequeue();
                next.Expand(g, transposition, queue);
				BNode.InitiateVisiting();
                next.Update(g);

				/*if(count % 1000 == 0) {
					Console.WriteLine("Value:         " + n.value);
					Console.WriteLine("Transposition: " + BNode.transposition.Count);
					Console.WriteLine("Queue:         " + BNode.queue.Count);
				}*/
				count++;
            }

			Console.WriteLine((root.value==1?"Black":root.value==-1?"White":"Nobody") + " won!");
			Console.WriteLine("Transposition: " + transposition.Count);
			Console.WriteLine("Queue:         " + queue.Count);
        }
	}
}


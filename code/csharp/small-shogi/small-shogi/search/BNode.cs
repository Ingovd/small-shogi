using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace smallshogi.search
{
    using Bits = System.UInt32;
    using B = BitBoard;

    public class BNode
    {
        List<BNode> children = new List<BNode>();
        List<BNode> parents = new List<BNode>();
        public Bits[] position;
        public byte c;
        public int marker, value;

        public static int visitMarker = 0;

        public BNode(Bits[] position, byte c)
        {
            this.position = position;
            this.c = c;
        }

        public void Update (Game g)
		{
			// If this node is already proved or visited, skip updating predecessors
			if (IsVisited () || value != 0)
				return;

			SetVisit ();
			// Determine new value of this node
			int sign = 2 * c - 1;
			int newValue = -sign;
			foreach (var child in children)
				newValue = sign * Math.Max (sign * newValue, sign * child.value);
			// If this node is not proved, skip updating
			if (newValue == 0) {
				UnVisit ();
				return;
			}

			value = newValue;

			foreach (var parent in parents)
				parent.Update (g);

			UnVisit ();
        }

        public void Expand(Game g, Hashtable transposition, Queue<BNode> queue)
        {
            children = new List<BNode>();
            var plies = g.children(position, c);
            foreach (var ply in plies)
            {
                var s = new BNode(ply.Apply (position, g), (byte)(c ^ 1));
                if (transposition.ContainsKey(s)) {
                    s = (BNode)transposition[s];
				}
                else
                {
                    s.Evaluate(g);
                    transposition[s] = s;
					if(s.value == 0)
                    	queue.Enqueue(s);
                }
                s.parents.Add(this);
                children.Add(s);
            }
        }

        public void Evaluate (Game g)
		{
			switch (g.gamePosition (position, c)) {
			case -1:
				value = 0;
				break;
			case 1:
				value = -1;
				break;
			case 2:
				value = 1;
				break;
			}

        }

        public static void InitiateVisiting()
        {
            visitMarker++;
        }

        public void SetVisit()
        {
            this.marker = BNode.visitMarker;
        }

        public void UnVisit()
        {
            this.marker--;
        }

        public bool IsVisited()
        {
            return this.marker == BNode.visitMarker;
        }

        public List<BNode> GetLongestGame()
        {
            InitiateVisiting();
            return DFSearch(value, 0);
        }

		public List<BNode> DFSearch (int win, int depth)
		{
			if (IsVisited ())
				return null;

			SetVisit();
			int sign = win * (- 2 * c + 1);
            sign = 1;
			List<BNode> temp = new List<BNode> ();
			List<BNode> best = new List<BNode> ();
			int bestL = - sign * int.MaxValue;
			foreach (var child in children)
				if (child.value == win) {
				temp = child.DFSearch(win, depth+1);
				if(temp != null && sign * temp.Count > sign * bestL) {
					best = temp;
					bestL = best.Count;
					}
				}
			best.Add(this);
			UnVisit();
			return best;
		}

        public int WinningStrategySize()
        {
            InitiateVisiting();
            int size = Size(value);
            Console.WriteLine(loops);
            return size;
        }

        static int loops = 0;
        private int Size(int win)
        {
            if (IsVisited())
            {
                loops++;
                return 0;
            }

            SetVisit();
            int size = 1;
            foreach (var child in children)
                if (child.value == win)
                    size += child.Size(win);
            return size;
        }

		public void Browse (Game g)
		{
			Console.WriteLine ("Current position with {0} to move:", c == 1 ? "Black" : "White");
			Console.WriteLine (g.prettyPrint (position));
			Console.WriteLine ("{0} wins", value == 1 ? "Black" : value == -1 ? "White" : "Nobody");
			Console.WriteLine ("{0} possible moves:", children.Count);
			foreach (var child in children) {
				Console.WriteLine ("Value: " + child.value);
				Console.WriteLine (g.prettyPrint (child.position));
			}
			Console.WriteLine ("{0} parents:", parents.Count);
			foreach (var parent in parents) {
				Console.WriteLine ("Value: " + parent.value);
				Console.WriteLine (g.prettyPrint (parent.position));
			}
			var input = Console.ReadLine ();
			if (input.StartsWith ("c")) {
				input = input.Substring (1);
				int i = Int32.Parse (input);
				children [i - 1].Browse (g);
			}
			if (input.StartsWith ("p")) {
				input = input.Substring (1);
				int i = Int32.Parse (input);
				parents [i - 1].Browse (g);
			}
			if (input.StartsWith ("m")) {
	            var plies = g.children(position, c);
	            foreach (var ply in plies)
	            {
	                var s = new BNode(ply.Apply (position, g), (byte)(c ^ 1));
					Console.WriteLine(g.prettyPrint(s.position));
	            }
				Browse (g);
			}
		}

		public override int GetHashCode()
        {
            var hash = 982451653;
            foreach (Bits b in position)
                hash = 997 * hash + (int)b;
            hash ^= c * 2147483647;
            return hash;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;
            //if(System.Object.ReferenceEquals(this, obj))
            //   return true;
            BNode node = obj as BNode;
            if (node == null)
                return false;
            return Equals(node);
        }

        public bool Equals(BNode other)
        {
            if (c != other.c)
                return false;
            if (position.Length != other.position.Length)
                return false;
            for (int i = 0; i < position.Length; ++i)
                if (!position[i].Equals(other.position[i]))
                    return false;
            return true;
        }
    }
}
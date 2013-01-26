using System;
using System.Collections;
using System.Collections.Generic;

namespace smallshogi
{
    using Bits = System.UInt32;
    using B = BitBoard;

    public class Node
    {
        public Bits[] position;
        public List<Node> children;
        public List<Node> parents = new List<Node>();
        public Node previous;
        public byte c;
        public int pn, dn, marker;
        static int visitMarker;

        public Node(Bits[] position, byte colour)
        {
            this.position = position;
            this.c = colour;
        }

        public static void InitiateVisiting()
        {
            visitMarker++;
        }

        public void SetVisit()
        {
            this.marker = Node.visitMarker;
        }

        public void UnVisit()
        {
            this.marker--;
        }

        public bool IsVisited()
        {
            return this.marker == Node.visitMarker;
        }

        public void StartUpdate()
        {
            UpdateNumbers();

            // Update parents
            foreach (var parent in parents)
                parent.Update();
        }

        public void Update()
        {
            if (IsVisited() || (pn != 0 && dn != 0))
                return;
            SetVisit();

            UpdateNumbers();

            // Update parents
            foreach (var parent in parents)
                parent.Update();

			UnVisit ();
        }

		public void StartUpdateTree ()
		{
			UpdateNumbers();

			foreach(var parent in parents)
				parent.UpdateTree (this);
		}

		public void UpdateTree (Node n)
		{
			if (this.Equals (n)) {
				n.pn = Int32.MaxValue;
				n.dn = 0;
				n.Update ();
				return;
			}

			UpdateNumbers();

			foreach(var parent in parents)
				parent.UpdateTree(n);
		}

        public void UpdateNumbers()
        {
			if(children == null)
				return;

            if (c == 1)
            {
                int min = Int32.MaxValue;
                int sum = 0;
                foreach (var child in children)
                {
                    min = Math.Min(child.pn, min);
                    if (sum + child.dn >= sum)
                        sum += child.dn;
                    else
                        sum = Int32.MaxValue;
                }
                pn = min;
                dn = sum;
            }
            else
            {
                int min = Int32.MaxValue;
                int sum = 0;
                foreach (var child in children)
                {
                    min = Math.Min(child.dn, min);
                    if (sum + child.pn >= sum)
                        sum += child.pn;
                    else
                        sum = Int32.MaxValue;
                }
                dn = min;
                pn = sum;
            }
        }

        public void Evaluate(Game g)
        {
            int pos = g.gamePosition(position, c);
            if (pos > 0)
            {
                pn = -1 * (pos - 2) * Int32.MaxValue;
                dn = (pos - 1) * Int32.MaxValue;
            }
            else
            {
                pn = 1;
                dn = 1;
            }
        }

        public void Expand(Game g, Hashtable transposition)
        {
            if(children != null)
            	return;
            children = new List<Node>();
            var plies = g.children(position, c);
            foreach (var ply in plies)
            {
                var s = new Node(ply.Apply(position, g), (byte)(c ^ 1));
                if (transposition.ContainsKey(s))
                    s = (Node)transposition[s];
                else
                {
                    s.Evaluate(g);
                    transposition[s] = s;
                }
                s.parents.Add(this);
                children.Add(s);
            }
        }

        public void ExpandTree(Game g, ref int nodeCount)
        {
            if(children != null)
            	return;
            children = new List<Node>();
            var plies = g.children(position, c);
            foreach (var ply in plies)
            {
                var s = new Node(ply.Apply(position, g), (byte)(c ^ 1));
                s.Evaluate(g);
				nodeCount++;
                s.parents.Add(this);
                children.Add(s);
            }
        }

		public List<Node> GetLongestGame(Game g)
        {
            InitiateVisiting();
            return DFSearch(g, pn, 0);
        }

		public List<Node> DFSearch (Game g, int win, int depth)
		{
			if (IsVisited ())
				return null;

			SetVisit();
			int sign = (win==0?1:-1) * (- 2 * c + 1);
            sign = 1;
			List<Node> temp = new List<Node> ();
			List<Node> best = new List<Node> ();
			int bestL = - sign * int.MaxValue;
			if(children != null)
				foreach (var child in children)
					if (child.pn == win) {
					temp = child.DFSearch(g, win, depth+1);
					if(temp != null && sign * temp.Count > sign * bestL) {
						best = temp;
						bestL = best.Count;
						}
					}
			best.Add(this);
			UnVisit();
			return best;
		}

        public int Size()
        {
            if (IsVisited())
                return 0;
            SetVisit();
            var size = 1;
            if (children != null)
                foreach (var child in children)
                    size += child.Size();
            return size;
        }

        public void Show(Game g)
        {
            Console.WriteLine(g.prettyPrint(position));
            if (children != null)
                foreach (var child in children)
                    child.Show(g);
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
            Node node = obj as Node;
            if (node == null)
                return false;
            return Equals(node);
        }

        public bool Equals(Node other)
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


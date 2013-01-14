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

        public static Hashtable transposition = new Hashtable();

        public Node(Bits[] position, byte colour)
        {
            this.position = position;
            this.c = colour;
        }

        public Node(Ply p, Node parent, byte colour)
        {
            this.position = p.apply(parent.position);
            c = colour;
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

        public void Draw()
        {
            pn = Int32.MaxValue;
            dn = 0;
            Node.InitiateVisiting();
            children = null;
            foreach (var parent in parents)
                parent.Update(null);
        }

        public bool DetectDraw()
        {
            InitiateVisiting();
            return Drawn();
        }

        public bool Drawn()
        {
            // If this node has already been visited its a draw INCORRECT
            if (IsVisited())
                return true;
            SetVisit();
            // Check if this node is proven, return result
            if (pn == 0 || dn == 0)
                return pn > 0;
            // Check if this is an unexpanded node, return no draw
            if (children != null)
            {
                // Return draw value of children
                if (c == 1)
                {
                    foreach (var child in children)
                        if (!child.Drawn())
                            return false;
                    return true;
                }
                foreach (var child in children)
                    if (child.Drawn())
                        return true;
            }
            // Unvisit?
            return false;
        }

        public void StartUpdate()
        {
            UpdateNumbers();

            // Update parents
            foreach (var parent in parents)
                parent.Update(this);
        }

        public void Update(Node origin)
        {
            // DAG/DCG test, dont visit a node more than once
            if (IsVisited())
                return;
            SetVisit();

            UpdateNumbers();

            // Clear some memory maybe
            //if(pn == 0 || dn == 0)
            //	children = null;

            // Update parents
            foreach (var parent in parents)
                parent.Update(origin);
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

        public bool Expand(Game g)
        {
            if(children != null)
            	return false;
            children = new List<Node>();
            var plies = g.children(position, c);
            foreach (var ply in plies)
            {
                var s = new Node(ply, this, (byte)(c ^ 1));
                /*s.Evaluate(g);
                if(!transposition.ContainsKey(s))
                    transposition[s] = s;*/
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
            return true;
        }

        public Node GetCycle()
        {
            InitiateVisiting();
            return Cycle();
        }

        public Node Cycle()
        {
            if (IsVisited())
            {
                foreach (var parent in parents)
                    parent.children.Remove(this);
                return this;
            }
            SetVisit();
            if (children != null)
                foreach (var child in children)
                {
                    var end = child.Cycle();
                    if (end != null)
                    {
                        child.previous = this;
                        return end;
                    }
                }
            UnVisit();
            return null;
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


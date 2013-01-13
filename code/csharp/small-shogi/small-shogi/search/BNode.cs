using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace smallshogi.search
{
    using Bits = System.UInt32;
    using B = BitBoard;

    class BNode
    {
        List<BNode> children;
        List<BNode> parents;
        Bits[] position;
        byte c, value;
        int marker;

        static int visitMarker = 0;
        public static Hashtable transposition = new Hashtable();
        static Queue<BNode> queue = new Queue<BNode>();

        public BNode(Bits[] position, byte c)
        {
            this.position = position;
            this.c = c;
        }

        public BNode(Ply p, BNode parent, byte colour)
        {
            this.position = p.apply(parent.position);
            c = colour;
        }

        public BNode Prove(BNode n, Game g)
        {
            queue.Enqueue(n);
            while (n.value == 0)
            {
                var next = queue.Dequeue();
                next.Expand(g);
                next.Update();
            }


            return n;
        }

        public void Update()
        {
            // Update all paths
        }

        public void Expand(Game g)
        {
            //if (children != null)
            //    return;
            children = new List<BNode>();
            var plies = g.children(position, c);
            foreach (var ply in plies)
            {
                var s = new BNode(ply, this, (byte)(c ^ 1));
                if (transposition.ContainsKey(s))
                    s = (BNode)transposition[s];
                else
                {
                    s.Evaluate(g);
                    transposition[s] = s;
                    queue.Enqueue(s);
                }
                s.parents.Add(this);
                children.Add(s);
            }
        }

        public void Evaluate(Game g)
        {
            value = (byte)g.gamePosition(position, c);
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


    }
}
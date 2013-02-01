using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace smallshogi
{
	public class DataReader
	{
		string dirpath;
		public List<SolveCompare> data = new List<SolveCompare> ();

		public DataReader (string dirpath)
		{
			this.dirpath = dirpath;
		}

		public void ReadAll ()
		{
			string[] lines;
			List<string> dlines = null;
			for (int i = 0; i < 1372; i++) {
				SolveCompare current = new SolveCompare (i);
				lines = File.ReadAllLines (dirpath + "/" + i + ".txt");
				dlines = new List<string> ();
				for (int j = 0; j < lines.Length; ++j) {
					if (lines [j] == "") {
						current.AddDatum (ReadDatum (dlines));
						dlines = new List<string> ();
					} else {
						dlines.Add (lines [j]);
					}
				}
				data.Add (current);
			}
		}

		public void AdHocMergeData (List<SolveCompare> data2)
		{
			List<SolveCompare> newdata = new List<SolveCompare> ();
			foreach (var scold in data) {
				SolveCompare sc = new SolveCompare (scold.seed);
				SolveCompare scnew = data2 [scold.seed];
				if (scnew.pngraph != null)
					sc.AddDatum (scnew.pngraph);
				else
					sc.AddDatum (scold.pngraph);
				if (scnew.pntree != null)
					sc.AddDatum (scnew.pntree);
				else
					sc.AddDatum (scold.pntree);
				if (scnew.bfs != null)
					sc.AddDatum (scnew.bfs);
				else
					sc.AddDatum (scold.bfs);
				newdata.Add (sc);
			}
			data = newdata;
		}

		public void SortData (Comparison<SolveCompare> comparison)
		{
			data.Sort (comparison);
		}

		public void FilterData (Func<SolveCompare, bool> predicate)
		{
			data = data.Where (predicate).ToList ();
		}

		public Tuple<int, int, int, int, int> ResultCount ()
		{
			int error = 0;
			int[] temp = new int[4];
			foreach (var sc in data) {
				int value = -2;
				if (sc.pngraph.exception == null)
					value = sc.pngraph.value;
				if (sc.pntree.exception == null) {
					if (value < -1)
						value = sc.pntree.value;
					else
						if (value != sc.pntree.value) {
						error++;
						continue;
					}
				}
				if (sc.bfs.exception == null) {
					if (value < -1)
						value = sc.bfs.value;
					else
						if (value != sc.bfs.value) {
						error++;
						continue;
					}
				}
				temp[value + 2]++;
			}
			return Tuple.Create(temp[3], temp[1], temp[2], temp[0], error);
		}

		public Tuple<int, int, int> SolvedGameCount ()
		{
				int pngraphc = 0, pntreec = 0, bfsc = 0;
				foreach (var sc in data) {
					if (sc.pngraph.exception == null)
						pngraphc++;
					if (sc.pntree.exception == null)
						pntreec++;
					if (sc.bfs.exception == null)
						bfsc++;
				}
				return Tuple.Create (pngraphc, pntreec, bfsc);
		}

		public int NodeCount (string method)
		{
			Func<SolveCompare, int> f = sc => 0;
				switch (method) {
				case "pnt":
					f = sc => sc.pntree.count;
					break;
				case "png":
					f = sc => sc.pngraph.count;
					break;
				case "bfs":
					f = sc => sc.bfs.count;
					break;
				}
				int count = 0;
				foreach (var sc in data)
					count += f(sc);
			return count;
		}

		Datum ReadDatum (List<string> lines)
		{
			string type = null, exception = null;
			int value = Int32.MinValue, time = Int32.MinValue, count = Int32.MinValue;
			string[] args;
			foreach (var line in lines) {
				args = line.Split (':');
				if (args.Length == 2)
					switch (args [0]) {
					case "Type":
						type = args [1];
						break;
					case "Value":
						value = Int32.Parse (args [1]);
						break;
					case "Time":
						time = Int32.Parse (args [1]);
						break;
					case "Count":
						count = Int32.Parse (args [1]);
						break;
					case "Exception":
						exception = args [1];
						break;
					}
			}
			Datum d = new Datum (type, value, time, count);
			if (exception != null)
				d.exception = exception;
			return d;
		}

		public class SolveCompare
		{
			public int seed;
			public Datum pngraph, pntree, bfs;

			public SolveCompare (int seed)
			{
				this.seed = seed;
			}

			public int Count ()
			{
				int count = 0;
				if (pngraph != null && pngraph.exception == null)
					count += pngraph.count;
				if (pntree != null && pntree.exception == null)
					count += pntree.count;
				if (bfs != null && bfs.exception == null)
					count += bfs.count;
				return count;
			}

			public void AddDatum (Datum d)
			{
				switch (d.type) {
				case "PNG":
					pngraph = d;
					break;
				case "PNT":
					pntree = d;
					break;
				case "BFS":
					bfs = d;
					break;
				}
			}

		}

		public class Datum
		{
			public string type, exception = null;
			public int value, time, count;

			public Datum (string type, int value, int time, int count)
			{
				this.type = type;
				this.value = value;
				this.time = time;
				this.count = count;
			}

			public bool ExceptionOccured ()
			{
				return exception != null;
			}
		}
	}
}


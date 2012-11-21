using System;

namespace smallshogi
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Func<string, string> prependMoreShit = x => "shit" + x;

			Console.WriteLine (prependMoreShit(prependShit("Hello World!")));
		}

		public static string prependShit (string s)
		{
			return "shit" + s;
		}
	}
}

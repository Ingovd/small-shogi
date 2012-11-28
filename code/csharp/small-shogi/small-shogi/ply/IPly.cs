using System;

namespace smallshogi
{
	public interface IPly
	{
		void apply (BitBoard[] position);
	}
}


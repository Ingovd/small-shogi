using System;
using System.Collections.Generic;

namespace smallshogi
{
	public class BitBoard
	{
		public UInt32 bits;

		public BitBoard ()
		{
			bits = 0;
		}

		public BitBoard (uint bits)
		{
			this.bits = bits;
		}

		public BitBoard (BitBoard b)
		{
			this.bits = b.bits;
		}

		public BitBoard And (BitBoard b)
		{
			bits &= b.bits;
			return this;
		}

		public BitBoard Or (BitBoard b)
		{
			bits |= b.bits;
			return this;
		}

		public BitBoard Xor (BitBoard b)
		{
			bits ^= b.bits;
			return this;
		}

		public BitBoard Not ()
		{
			bits = ~bits;
			return this;
		}

		public uint LSB ()
		{
			return bits & ~(bits - 1U); // TODO (if needed): should return index
		}

		public BitBoard LSBitBoard ()
		{
			return new BitBoard (bits & ~(bits - 1));
		}

		public BitBoard PushMasked (BitBoard mask)
		{
			// Get masked value
			var masked = new BitBoard(bits & mask.bits);
			// Remove it
			bits ^= masked.bits;
			// Add pushed masked value (disregarding overflow, easily added by masking again)
			bits |= (masked.bits << 1) | mask.LSBitBoard().bits;
			return this;
		}

		public BitBoard PopMasked (BitBoard mask)
		{
			// Get masked value
			var masked = new BitBoard(bits & mask.bits);
			// Remove it
			bits ^= masked.bits;
			// Add popped masked value
			bits |= mask.LSBitBoard ().Not ().And(masked).bits >> 1;
			return this;
		}

		public List<BitBoard> allOnes ()
		{
			var result = new List<BitBoard> ();
			BitBoard workingBits = new BitBoard (bits);
			BitBoard i;
			while (true) {
				i = workingBits.LSBitBoard ();
				if (i.bits != 0) {
					result.Add (i);
					workingBits.Xor (i);
				} else
					break;
			}
			return result;
		}

		public bool Get (int i)
		{
			return (bits & (1 << i)) != 0;
		}

		public void Set (int i)
		{
			bits |= (uint)(1 << i);
		}

		public override int GetHashCode ()
		{
			return (int)bits;
		}

		public override bool Equals (Object obj)
		{
			if (obj == null)
				return false;
			BitBoard b = obj as BitBoard;
			if (b == null)
				return false;
			return bits == b.bits;
		}

		public bool Equals (BitBoard b)
		{
			if (b == null)
				return false;
			return bits == b.bits;
		}

		public string toString (int width, int length)
		{
			string s = "";
			for (int i = 0; i < length; i++) {
				if (i % width == 0 && i != 0)
					s += "\n";
				bool bit = Get (i);
				s += bit ? 1 : 0;
			}
			return s;
		}
	}
}


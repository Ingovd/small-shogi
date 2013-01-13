using System;
using System.Collections.Generic;

namespace smallshogi
{
    using Bits = System.UInt32;

	public class BitBoard
	{
		public Bits bits;

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

        public static Bits LSBit(Bits bits)
        {
            return bits & ~(bits - 1);
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

        public static Bits PushMasked(ref Bits bits, Bits mask)
        {
            // Get masked value
            var masked = bits & mask;
            // Add pushed masked value (disregarding overflow, easily added by masking again)
            bits |= (masked << 1) | LSBit(mask);
            return bits;
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

        public static Bits PopMasked(ref Bits bits, Bits mask)
        {
            // Get masked value
            var masked = bits & mask;
            // Remove it
            bits ^= masked;
            // Add popped masked value
            bits |= (masked >> 1) & mask;
            return bits;
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

        public static List<Bits> allOnes(Bits bits)
        {
            var result = new List<Bits>();
            Bits workingBits = bits;
            Bits i;
            while (true)
            {
                i = LSBit(workingBits);
                if (i != 0)
                {
                    result.Add(i);
                    workingBits ^= i;
                }
                else
                    break;
            }
            return result;
        }

		public bool Get (int i)
		{
			return (bits & (1 << i)) != 0;
		}

        public static bool Get(Bits bits, int i)
        {
            return (bits & (1 << i)) != 0;
        }

		public void Set (int i)
		{
			bits |= (uint)(1 << i);
		}

        public static Bits Set(ref Bits bits, int i)
        {
            return bits |= ((Bits)(1 << i));
        }

        public bool IsEmpty()
        {
            return bits == 0;
        }

		public bool NotEmpty ()
		{
			return bits != 0;
		}

		public bool Overlaps(BitBoard b) {
			return (bits & b.bits) != 0;
		}

        public static bool Overlaps(Bits b1, Bits b2)
        {
            return (b1 & b2) != 0;
        }

		public bool Subset(BitBoard b) {
			return (bits & b.bits) == bits;
		}

        public static bool Subset(Bits b1, Bits b2)
        {
            return (b1 & b2) == b1;
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

		public string ToString (int width, int length)
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

        public static string ToString(Bits bits, int width, int length)
        {
            string s = "";
            for (int i = 0; i < length; i++)
            {
                if (i % width == 0 && i != 0)
                    s += "\n";
                bool bit = Get(bits, i);
                s += bit ? 1 : 0;
            }
            return s;
        }
	}
}


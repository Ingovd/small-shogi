using System;
using System.Collections.Generic;

namespace smallshogi
{
    using Bits = System.UInt16;

	public static class BitBoard
	{
		/// <summary>
		/// Returns the least significant bit of <para>bits</para>.
		/// </summary>
		/// <returns>
		/// A bit pattern of equal length as <para>bits</para> with only the least significant bit of <para>bits</para> set.
		/// </returns>
		/// <param name='bits'>
		/// Bit pattern from which the least significant bit is extracted.
		/// </param>
		public static Bits LSBit (Bits bits)
		{
			return (Bits)(bits & ~(bits - 1));
		}

		/// <summary>
		/// Interprets the masked bits as a stack and pushes a bit on top of it.
		/// </summary>
		/// <param name='bits'>
		/// Bit pattern containing several stacks accessible by masking the correct consecutive bits.
		/// </param>
		/// <param name='mask'>
		/// Mask indicating what stack in <para>bits</para> to use.
		/// </param>
		public static void PushMasked (ref Bits bits, Bits mask)
		{
			// Get masked value
			var masked = (bits & mask);
			// Add pushed masked value (disregarding overflow, easily fixed by masking again)
			bits |= (Bits)((masked << 1) | LSBit (mask));
		}

		/// <summary>
		/// Interprets the masked bits as a stack and pops one bit off of it.
		/// </summary>
		/// <param name='bits'>
		/// Bit pattern containing several stacks accessible by masking the correct consecutive bits.
		/// </param>
		/// <param name='mask'>
		/// Mask indicating what stack in <para>bits</para> to use.
		/// </param>
		public static void PopMasked (ref Bits bits, Bits mask)
		{
			// Get masked value
			Bits masked = (Bits)(bits & mask);
			// Remove it
			bits ^= masked;
			// Add popped masked value
			bits |= (Bits)((masked >> 1) & mask);
		}

		/// <summary>
		/// This method is used to access all the on bits in <para>bits</para> as bit pattern with equal length
		/// as <para>bits</para>.
		/// </summary>
		/// <returns>
		/// A minimal list of bit patterns with exactly one on bit such that their symmetric difference equals <para>bits</para>.
		/// </returns>
		/// <param name='bits'>
		/// The bit pattern from which each individual on bit is extracted.
		/// </param>
		public static List<Bits> allOnes (Bits bits)
		{
			var result = new List<Bits> ();
			Bits workingBits = bits;
			Bits i;
			while (true) {
				i = LSBit (workingBits);
				if (i != 0) {
					result.Add (i);
					workingBits ^= i;
				} else
					break;
			}
			return result;
		}

		/// <summary>
		/// Get the value of bit <para>i</para> in bits.
		/// </summary>
		/// <param name='bits'>
		/// Input bit pattern.
		/// </param>
		/// <param name='i'>
		/// Index of the accessed bit.
		/// </param>
		public static bool Get (Bits bits, int i)
		{
			return (bits & (1 << i)) != 0;
		}

		/// <summary>
		/// Set the value of bit <para>i</para> in bits.
		/// </summary>
		/// <param name='bits'>
		/// Input bit pattern.
		/// </param>
		/// <param name='i'>
		/// Index of the accessed bit.
		/// </param>
		public static Bits Set (ref Bits bits, int i)
		{
			return bits |= ((Bits)(1 << i));
		}

		/// <summary>
		/// This method is used to check whether two bit patterns overlap (i.e. non-empty intersection).
		/// </summary>
		/// <param name='b1'>
		/// First bit pattern.
		/// </param>
		/// <param name='b2'>
		/// Second bit pattern.
		/// </param>
		public static bool Overlaps (Bits b1, Bits b2)
		{
			return (b1 & b2) != 0;
		}

		/// <summary>
		/// This method is used to check whether one bit pattern is a subset of another (i.e. union equals superset).
		/// </summary>
		/// <param name='b1'>
		/// The possible subset bit pattern.
		/// </param>
		/// <param name='b2'>
		/// The possible superset bit pattern.
		/// </param>
		public static bool Subset (Bits sub, Bits super)
		{
			return (sub & super) == sub;
		}

		/// <summary>
		/// Used for outputting a bit pattern as a <para>width</para>x<para>height</para> block.
		/// </summary>
		/// <returns>
		/// A string of 1's and 0's representing the bit pattern <para>bits</para>.
		/// </returns>
		/// <param name='bits'>
		/// Bits to be formatted as a string.
		/// </param>
		/// <param name='width'>
		/// Width of the block.
		/// </param>
		/// <param name='height'>
		/// Height of the block.
		/// </param>
		public static string ToString (Bits bits, int width, int height)
		{
			string s = "";
			for (int i = 0; i < height; i++) {
				if (i % width == 0 && i != 0)
					s += "\n";
				bool bit = Get (bits, i);
				s += bit ? 1 : 0;
			}
			return s;
		}
	}
}


using System;

namespace smallshogi
{
    using Bits = System.UInt32;

	public abstract class Ply
	{
		/// <summary>
		/// The colour of the player associated with this ply.
		/// </summary>
		protected int c;

		/// <summary>
		/// Apply this ply to the <para>position</para> according to game <para>g</para>.
		/// </summary>
		/// <param name='position'>
		/// The position from which this ply is possible
		/// </param>
		/// <param name='g'>
		/// The game containing all the rules.
		/// </param>
		public abstract Bits[] Apply (Bits[] position, Game g);

		/// <summary>
		/// Pieces the moved.
		/// </summary>
		/// <returns>
		/// The index of the piece type.
		/// </returns>
		public abstract int PieceMoved();
	}
}
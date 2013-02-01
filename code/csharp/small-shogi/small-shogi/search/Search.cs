using System;
using System.Collections.Generic;

namespace smallshogi.search
{
    using Bits = System.UInt16;

    public interface Search
    {
        void Prove(Game g);
        void SetTimeLimit(int minutes);

        int Value();
        int TimeSpent();
        int NodeCount();
        List<Bits[]> BestGame();
        String Name();
    }
}

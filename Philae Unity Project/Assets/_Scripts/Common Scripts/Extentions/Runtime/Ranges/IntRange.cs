using System;

namespace hedCommon.extension.runtime.range
{
    [Serializable]
    public struct IntRange
    {
        public int Min;
        public int Max;

        public IntRange(int min, int max) : this()
        {
            Min = min;
            Max = max;
        }

        public int Length
        {
            get
            {
                return (Max - Min) + 1;
            }
        }
    }
}
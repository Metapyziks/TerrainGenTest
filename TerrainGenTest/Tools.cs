using System;

namespace TerrainGenTest
{
    static class Tools
    {
        public static readonly Random Random = new Random();

        public static float NextSingle(this Random rand)
        {
            return (float) rand.NextDouble();
        }

        public static float NextSingle(this Random rand, float max)
        {
            return (float) (max * rand.NextDouble());
        }

        public static float NextSingle(this Random rand, float min, float max)
        {
            return (float) (min + (max - min) * rand.NextDouble());
        }

        public static int Wrap(this int val, int max)
        {
            int offset = val < 0 ? 1 : 0;
            return val - ((val + offset) / max - offset) * max;
        }
    }
}

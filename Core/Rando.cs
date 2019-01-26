using System;
using System.Security.Cryptography;

namespace MarsRoverScratch
{
    public static class Rando
    {
        private readonly static RandomNumberGenerator rng = new RNGCryptoServiceProvider();

        public static Int32 Next(Int32 minValue, Int32 upperExclusiveBound)
        {
            UInt32 max = (UInt32)((Int64)upperExclusiveBound - minValue);

            UInt32 n = Next(max);
            return (Int32)(n + minValue);
        }

        public static UInt32 Next(UInt32 upperExclusiveBound)
        {
            Byte[] bytes = new Byte[4];
            
            UInt32 randNum;
            UInt32 maxRand = (UInt32.MaxValue / upperExclusiveBound) * upperExclusiveBound;
            do
            {
                rng.GetBytes(bytes);
                randNum = BitConverter.ToUInt32(bytes, 0);
            } while (randNum >= maxRand); // Prevents bias against higher values of the number

            return randNum;
        }
    }
}

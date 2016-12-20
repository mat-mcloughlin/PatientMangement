using System;

namespace SeedGenerator
{
    public class Ward
    {
        public static int Get()
        {
            var rand = new Random();

            return rand.Next(1, 20);
        }
    }
}
using System;
using System.Text;

namespace LinkReducer.Utils
{
    public class RandomStringGenerator : IStringGenerator
    {
        private const string ALPHABET = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int ALPHABET_LENGTH = 62;
        private Random _rand;
        public RandomStringGenerator()
        {
            _rand = new Random();
        }

        public string GenerateString(int length)
        {
            StringBuilder result = new StringBuilder(length);
            for(int i = 0; i < length; ++i)
                result.Append(ALPHABET[_rand.Next(ALPHABET_LENGTH)]);
            return result.ToString();
        }
    }
}

using System.Globalization;

namespace ShamelessWordleCloneAPI
{
    public struct GameDictionary
    {
        public string Name;
        public string Description;
        public string Glyph;
        public float H;
        public float S;
        public float L;
        public int Seed;
        public uint Size;
        public string[] Words;
        public string[] WordsShuffled;

        public GameDictionary(string file)
        {
            using (var stream = File.OpenRead(file))
            using (var reader = new StreamReader(stream))
            {
                Name = ReadString(reader);
                Description = ReadString(reader);
                Glyph = ReadString(reader);
                H = ReadFloat(reader);
                S = ReadFloat(reader);
                L = ReadFloat(reader);

                if (reader.ReadLine() is not string seed)
                    throw new Exception("Not a valid dictionary");
                Seed = int.Parse(seed);
                var random = new Random(Seed);

                if (reader.ReadLine() is not string size)
                    throw new Exception("Not a valid dictionary");
                Size = uint.Parse(size);

                Words = new string[Size];
                var wp = 0;

                while (!reader.EndOfStream)
                {
                    var word = reader.ReadLine();
                    if (word == null)
                        break;

                    Words[wp++] = word;
                }

                WordsShuffled = new string[Size];
                Words.CopyTo(WordsShuffled, 0);
                for (int i = 0; i < Size; i++)
                {
                    var j = random.Next(i);
                    (WordsShuffled[i], WordsShuffled[j]) = (WordsShuffled[j], WordsShuffled[i]);
                }

                Console.WriteLine($"{Words[Size - 1]} + {WordsShuffled.Length} + {Size}");
            }
        }

        static string ReadString(StreamReader reader)
        {
            if (reader.ReadLine() is not string s)
                throw new Exception("Not a valid dictionary");

            return s;
        }

        static float ReadFloat(StreamReader reader)
        {
            if (reader.ReadLine() is not string s)
                throw new Exception("Not a valid dictionary");

            return float.Parse(s, new NumberFormatInfo() { NumberDecimalSeparator = "." });
        }
    }
}

using System.Configuration;
using System.Globalization;
using System.Security.Cryptography;

namespace ShamelessWordleCloneAPI
{
    public class GameDictionary
    {
        public string Name;
        public string Description;
        public string Glyph;
        public float H;
        public float S;
        public float V;
        public int Seed;
        public int Size;
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
                V = ReadFloat(reader);

                if (reader.ReadLine() is not string size)
                    throw new Exception("Not a valid dictionary");
                Size = int.Parse(size);

                Words = new string[Size];
                var wp = 0;

                while (!reader.EndOfStream)
                {
                    var word = reader.ReadLine();
                    if (word == null)
                        break;

                    Words[wp++] = word;
                }

                Seed = GetOrMakeSeed();
                Shuffle();
                lastUpdate = DateTime.UtcNow;

                var idx = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 60 / 60 / 24 % Size;
                cachedWord = new((int)idx, WordsShuffled[idx]);

#if DEBUG
                Console.WriteLine($"{Words[Size - 1]} + {WordsShuffled.Length} + {Size}");
#endif
            }
        }

        private void Shuffle()
        {
            var random = new Random(Seed);
            WordsShuffled = new string[Size];
            Words.CopyTo(WordsShuffled, 0);
            for (int i = 0; i < Size; i++)
            {
                var j = random.Next(i);
                (WordsShuffled[i], WordsShuffled[j]) = (WordsShuffled[j], WordsShuffled[i]);
            }
        }

        private int GetOrMakeSeed(bool regenerate = false)
        {
            Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var key = $"dict.{Name}";
            if (!regenerate && config.AppSettings.Settings.AllKeys.Contains(key))
                return int.Parse(config.AppSettings.Settings[key].Value);

            var seed = NewSeed();
            config.AppSettings.Settings.Add(key, $"{seed}");
            config.Save(ConfigurationSaveMode.Modified);
            return seed;
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

        Tuple<int, string>? cachedWord = null;
        DateTime lastUpdate;

        public int NewSeed()
        {
            return BitConverter.ToInt32(RandomNumberGenerator.GetBytes(sizeof(int)));
        }

        public Tuple<int, string> GetWord()
        {
            if (cachedWord == null || (DateTime.UtcNow - lastUpdate).Days > 0)
            {
                lastUpdate = DateTime.UtcNow;
                var idx = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 60 / 60 / 24 % Size;
                cachedWord = new((int)idx + 1, WordsShuffled[idx]);

                if (idx == Size - 1)
                {
                    Seed = GetOrMakeSeed(true);

                    Shuffle();
                }
            }

            return cachedWord;
        }
    }
}

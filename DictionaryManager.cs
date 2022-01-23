namespace ShamelessWordleCloneAPI
{
    public class DictionaryManager
    {
        public static DictionaryManager Instance
        {
            get
            {
                if (instance == null)
                    new DictionaryManager();

                return instance;
            }
        }

        public const string DictionaryExtension = ".dict";

        static DictionaryManager? instance;

        Dictionary<string, GameDictionary> dictionaries = new Dictionary<string, GameDictionary>();

        public DictionaryManager()
        {
            instance = this;
            
            var dictFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*" + DictionaryExtension + ".txt");

            foreach (var file in dictFiles)
            {
                try
                {
                    var name = string.Concat(Path.GetFileNameWithoutExtension(file).SkipLast(DictionaryExtension.Length)) ?? "";
                    if (name.Length == 0)
                        return;
                    
                    Console.WriteLine($"Loading \"{name}\"...");
                    dictionaries.Add(name, new GameDictionary(file));
                }
                catch (Exception)
                {
                    Console.WriteLine($"Warning: {file} is an invalid dictionary.");
                }
            }
        }

        public GameDictionary Get(string dictionary)
        {
            if (Exists(dictionary))
                return dictionaries[dictionary];

            throw new Exception("dictionary not found");
        }

        public string[] GetAll()
        {
            return dictionaries.Keys.ToArray();
        }

        public bool Exists(string dictionary)
        {
            return dictionaries.ContainsKey(dictionary);
        }
    }
}

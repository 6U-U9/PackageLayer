using System.Text.RegularExpressions;

namespace Refactor.Core
{
    public class Input
    {
        public string environment;
        public const string HUMAN_PREFIX = "human_";
        public const string INPUT_PREFIX = "input_";
        public const string LEAP_PREFIX = "leap_";
        public const string REVERSE_PREFIX = "reverse_";
        public List<string[]> dependencies = new();
        public Dictionary<string, int> humanLayers = new();
        public List<(string, string)> leapEdges = new();
        public List<(string, string)> reverseEdges = new();

        public Input(string environment)
        {
            this.environment = environment;
            Package.packages.Clear();
            ReadDependencies();
            ReadHumanLayers();
            ReadLeapEdges();
            ReadReverseEdges();
            foreach (var e in leapEdges)
            {
                if (!humanLayers.ContainsKey(e.Item1))
                    Console.WriteLine("WARN " +environment+"  "+ e.Item1);
                if (!humanLayers.ContainsKey(e.Item2))
                    Console.WriteLine("WARN " + environment + "  " + e.Item2);
            }
            foreach (var e in reverseEdges)
            {
                if (!humanLayers.ContainsKey(e.Item1))
                    Console.WriteLine("WARN " + environment + "  " + e.Item1);
                if (!humanLayers.ContainsKey(e.Item2))
                    Console.WriteLine("WARN " + environment + "  " + e.Item2);
            }
        }
        private string Format(string s)
        {
            return s.Trim().Trim('"', '“', '”', '‘', '’', '\'').Trim();
        }
        public void ReadDependencies()
        {
            string? input = (string?)Resources.ResourceManager.GetObject(INPUT_PREFIX + environment);
            if (input == null)
                throw new NullReferenceException(INPUT_PREFIX + environment);

            StringReader sr = new StringReader(input);
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                line = line.Trim();
                if (!line.Contains("->"))
                {
                    continue;
                }
                string[] sArray = Regex.Split(line, "->");
                for (int i = 0; i < sArray.Length; i++)
                {
                    sArray[i] = Format(sArray[i]);
                }
                dependencies.Add(new string[2] { sArray[0], sArray[1] });
            }
        }
        public void ReadHumanLayers()
        {
            string? input = (string?)Resources.ResourceManager.GetObject(HUMAN_PREFIX + environment);
            if (input == null)
                throw new NullReferenceException(HUMAN_PREFIX + environment);

            StringReader sr = new StringReader(input);
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                line = line.Trim();
                string[] sArray = Regex.Split(line, "\\s+", RegexOptions.IgnoreCase);
                for (int i = 0; i < sArray.Length; i++)
                {
                    sArray[i] = Format(sArray[i]);
                }

                int l;
                if (int.TryParse(sArray[1], out l))
                    humanLayers[sArray[0]] = l;
            }
        }
        public void ReadLeapEdges()
        {
            string? input = (string?)Resources.ResourceManager.GetObject(LEAP_PREFIX + environment);
            if (input == null)
                throw new NullReferenceException(LEAP_PREFIX + environment);

            StringReader sr = new StringReader(input);
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                line = line.Trim();
                if (!line.Contains("->"))
                {
                    continue;
                }
                string[] sArray = Regex.Split(line, "->");
                for (int i = 0; i < sArray.Length; i++)
                {
                    sArray[i] = Format(sArray[i]);
                }
                leapEdges.Add((sArray[0], sArray[1]));
            }
        }
        public void ReadReverseEdges()
        {
            string? input = (string?)Resources.ResourceManager.GetObject(REVERSE_PREFIX + environment);
            if (input == null)
                throw new NullReferenceException(REVERSE_PREFIX + environment);

            StringReader sr = new StringReader(input);
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                line = line.Trim();
                if (!line.Contains("->"))
                {
                    continue;
                }
                string[] sArray = Regex.Split(line, "->");
                for (int i = 0; i < sArray.Length; i++)
                {
                    sArray[i] = Format(sArray[i]);
                }
                reverseEdges.Add((sArray[0], sArray[1]));
            }
        }
    }
}

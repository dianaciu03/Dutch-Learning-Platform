using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Interfaces;

namespace ContentService.ExerciseComponents
{
    public class VocabularyComponent : IExerciseComponent
    {
        public const string INSTRUCTION = "Write down the correct translation of the given words.";
        public int nrOfWordPairs { get; private set; }
        public (string DutchWord, string EnglishWord)[] WordPairs { get; private set; }

        public VocabularyComponent(int numberOfWordPairs, List<(string, string)> wordPairs)
        {
            nrOfWordPairs = numberOfWordPairs;
            WordPairs = new (string, string)[nrOfWordPairs];

            for (int i = 0; i < nrOfWordPairs && i < wordPairs.Count; i++)
            {
                WordPairs[i] = wordPairs[i];
            }
        }

        public void Display()
        {
            Console.WriteLine("\n~ Vocabulary Section ~");
            Console.WriteLine($"\n {INSTRUCTION}");
            foreach (var pair in WordPairs)
            {
                Console.WriteLine($"🔹 {pair.DutchWord} → _____________________");
            }
        }
    }
}

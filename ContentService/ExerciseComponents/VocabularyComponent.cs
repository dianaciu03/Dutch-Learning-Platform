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
        public string Instruction { get; private set; }
        public int NrOfWordPairs { get; private set; }
        public (string DutchWord, string EnglishWord)[] WordPairs { get; private set; }

        public VocabularyComponent()
        {
            Instruction = INSTRUCTION;
        }

        public VocabularyComponent(int numberOfWordPairs, List<(string, string)> wordPairs)
        {
            Instruction = INSTRUCTION;
            NrOfWordPairs = numberOfWordPairs;
            WordPairs = new (string, string)[NrOfWordPairs];

            for (int i = 0; i < NrOfWordPairs && i < wordPairs.Count; i++)
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

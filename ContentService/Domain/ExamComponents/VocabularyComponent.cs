using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Interfaces;

namespace ContentService.Domain.ExamComponents
{
    public class VocabularyComponent : IExamComponent
    {
        public string id { get; set; }
        public string? ExamId { get; set; }
        public ComponentType ComponentType { get; } = ComponentType.Vocabulary;

        public int NrOfWordPairs { get; private set; }
        public (string DutchWord, string EnglishWord)[] WordPairs { get; private set; }

        public VocabularyComponent(int numberOfWordPairs, List<(string, string)> wordPairs)
        {
            NrOfWordPairs = numberOfWordPairs;
            WordPairs = new (string, string)[NrOfWordPairs];

            for (int i = 0; i < NrOfWordPairs && i < wordPairs.Count; i++)
            {
                WordPairs[i] = wordPairs[i];
            }
        }
        public ComponentType GetComponentType() => ComponentType;
    }
}

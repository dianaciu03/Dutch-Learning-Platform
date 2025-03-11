using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Interfaces;

namespace ContentService.Domain.ExerciseComponents
{
    public class ReadingComponent : IExerciseComponent
    {
        public const string INSTRUCTION = "Read the following text and answer the questions below.";
        public string Instruction { get; private set; }
        public string GivenText { get; private set; }
        public List<string> Questions { get; private set; }

        public ReadingComponent()
        {
            Instruction = INSTRUCTION;
        }

        public ReadingComponent(string givenText, List<string> questions)
        {
            Instruction = INSTRUCTION;
            GivenText = givenText;
            Questions = questions;
        }

        public void Display()
        {
            Console.WriteLine("\n~ Reading Section ~");
            Console.WriteLine($"\n {INSTRUCTION}");
            Console.WriteLine($"\n {GivenText}");
            Console.WriteLine("\nQuestions:");
            for (int i = 0; i < Questions.Count; i++)
            {
                Console.WriteLine($"\n{i + 1}. {Questions[i]}");
            }
        }
    }
}

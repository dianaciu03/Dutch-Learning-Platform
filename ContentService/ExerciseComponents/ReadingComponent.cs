using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Interfaces;

namespace ContentService.ExerciseComponents
{
    public class ReadingComponent: IExerciseComponent
    {
        public const string INSTRUCTION = "Read the following text and answer the questions below.";
        public static string Instruction => INSTRUCTION;
        public string givenText { get; private set; }
        public List<string> questions { get; private set; }

        public ReadingComponent() 
        { 
        }

        public ReadingComponent(string givenText, List<string> questions)
        {
            this.givenText = givenText;
            this.questions = questions;
        }

        public void Display()
        {
            Console.WriteLine("\n~ Reading Section ~");
            Console.WriteLine($"\n {INSTRUCTION}");
            Console.WriteLine($"\n {givenText}");
            Console.WriteLine("\nQuestions:");
            for (int i = 0; i < questions.Count; i++)
            {
                Console.WriteLine($"\n{i + 1}. {questions[i]}");
            }
        }
    }
}

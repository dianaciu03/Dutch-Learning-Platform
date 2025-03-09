using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Interfaces;

namespace ContentService.ExerciseComponents
{
    public class GrammarComponent : IExerciseComponent
    {
        public GrammarComponent()
        {
        }

        public void Display()
        {
            Console.WriteLine("Displaying Grammar Component");
        }
    }
}

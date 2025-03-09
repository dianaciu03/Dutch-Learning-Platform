using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Interfaces;

namespace ContentService.ExerciseComponents
{
    public class SpeakingComponent : IExerciseComponent
    {
        public SpeakingComponent()
        {
        }
        public void Display()
        {
            Console.WriteLine("Speaking Component");
        }
    }
}

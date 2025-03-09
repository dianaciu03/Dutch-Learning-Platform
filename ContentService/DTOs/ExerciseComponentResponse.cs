using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Interfaces;

namespace ContentService.DTOs
{
    public class ExerciseComponentResponse
    {
        public IExerciseComponent ExerciseComponent { get; set; }

        public ExerciseComponentResponse(IExerciseComponent exerciseComponent)
        {
            ExerciseComponent = exerciseComponent;
        }
    }
}

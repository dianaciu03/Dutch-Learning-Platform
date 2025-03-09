using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Domain;

namespace ContentService.DTOs
{
    public class CreateExerciseRequest
    {
        public required List<string> ExerciseTypes { get; set; }
        public required string Level { get; set; }
    }
}

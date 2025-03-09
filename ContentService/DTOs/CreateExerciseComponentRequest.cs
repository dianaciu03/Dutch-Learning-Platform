using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentService.DTOs
{
    public class CreateExerciseComponentRequest
    {
        public required string ComponentType { get; set; }
    }
}

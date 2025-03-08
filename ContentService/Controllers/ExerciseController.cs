using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ContentService.Domain;
using ContentService.Interfaces;
using ContentService.Managers;
using Microsoft.AspNetCore.Mvc;

namespace ContentService.Controllers
{
    [ApiController]
    [Route("api/exercises")]
    public class ExerciseController : ControllerBase
    {
        private readonly ExerciseManager _exerciseManager;

        public ExerciseController()
        {
            _exerciseManager = new ExerciseManager();
        }

        // Create
        [HttpPost]
        public IActionResult AddExercise(int id, List<ExerciseType> types, CEFRLevel level, IEnumerable<IExerciseComponent> exerciseComponents)
        {
            _exerciseManager.AddExercise(id, types, level, exerciseComponents);
            return Ok();
        }

        // Read
        [HttpGet("{id}")]
        public IActionResult GetExerciseById(int id)
        {
            var exercise = _exerciseManager.GetExerciseById(id);
            if (exercise == null)
            {
                return NotFound();
            }
            return Ok(exercise);
        }

        [HttpGet]
        public IActionResult GetAllExercises()
        {
            var exercises = _exerciseManager.GetAllExercises();
            return Ok(exercises);
        }

        // Update
        [HttpPut("{id}")]
        public IActionResult UpdateExercise(int id, List<ExerciseType> types, CEFRLevel level, IEnumerable<IExerciseComponent> exerciseComponents)
        {
            var result = _exerciseManager.UpdateExercise(id, types, level, exerciseComponents);
            if (!result)
            {
                return NotFound();
            }
            return Ok();
        }

        // Delete
        [HttpDelete("{id}")]
        public IActionResult DeleteExercise(int id)
        {
            var result = _exerciseManager.DeleteExercise(id);
            if (!result)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}

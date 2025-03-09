using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ContentService.Domain;
using ContentService.DTOs;
using ContentService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ContentService.Controllers
{
    [ApiController]
    [Route("api/exercises")]
    public class ExerciseController(IExerciseManager exerciseManager) : ControllerBase
    {
        private readonly IExerciseManager _exerciseManager = exerciseManager;

        // Create
        [HttpPost]
        public async Task<IActionResult> CreateExercise([FromBody] CreateExerciseRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            bool createdExercise = await Task.Run(() => _exerciseManager.CreateExercise(request));

            if (!createdExercise)
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return Ok("Exercise created successfully.");
        }

        [HttpPost("components")]
        public async Task<IActionResult> CreateExerciseComponents([FromBody] CreateExerciseComponentRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdComponent = await Task.Run(() => _exerciseManager.CreateExerciseComponent(request));

            if (createdComponent == null)
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return Ok(createdComponent);
        }

        //// Read
        //[HttpGet("{id}")]
        //public IActionResult GetExerciseById(int id)
        //{
        //    var exercise = _exerciseManager.GetExerciseById(id);
        //    if (exercise == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(exercise);
        //}

        //[HttpGet]
        //public IActionResult GetAllExercises()
        //{
        //    var exercises = _exerciseManager.GetAllExercises();
        //    return Ok(exercises);
        //}

        //// Update
        //[HttpPut("{id}")]
        //public IActionResult UpdateExercise(int id, List<ExerciseType> types, CEFRLevel level, IEnumerable<IExerciseComponent> exerciseComponents)
        //{
        //    var result = _exerciseManager.UpdateExercise(id, types, level, exerciseComponents);
        //    if (!result)
        //    {
        //        return NotFound();
        //    }
        //    return Ok();
        //}

        //// Delete
        //[HttpDelete("{id}")]
        //public IActionResult DeleteExercise(int id)
        //{
        //    var result = _exerciseManager.DeleteExercise(id);
        //    if (!result)
        //    {
        //        return NotFound();
        //    }
        //    return Ok();
        //}
    }
}

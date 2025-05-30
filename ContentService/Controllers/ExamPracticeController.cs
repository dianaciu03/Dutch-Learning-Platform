﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ContentService.Domain;
using ContentService.DTOs;
using ContentService.Interfaces;
using ContentService.Managers;
using Microsoft.AspNetCore.Mvc;

namespace ContentService.Controllers
{
    [ApiController]
    [Route("exams")]
    public class ExamPracticeController(IExamPracticeManager examManager) : ControllerBase
    {
        private readonly IExamPracticeManager _examManager = examManager;

        // Create
        [HttpPost]
        public async Task<IActionResult> CreateExamPractice([FromBody] CreateExamRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var examId = await _examManager.CreateExamPracticeAsync(request);

            if (examId == null)
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return Ok(new { message = "Exam practice created successfully.", id = examId });
        }

        [HttpPost("reading")]
        public async Task<IActionResult> CreateReadingComponent([FromBody] CreateReadingComponentRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var componentId = await _examManager.CreateReadingComponentAsync(request);

            if (componentId == null)
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return Ok(new { message = "Reading component created successfully.", id = componentId });
        }

        // Read
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await Task.Run(() => _examManager.GetAllExamPractices()));


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var examResponse = await Task.Run(() => _examManager.GetExamPracticeById(id));
            return examResponse == null ? NotFound() : Ok(examResponse);
        }

        // Update
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExamPractice(int id, [FromBody] UpdateExamRequest request)
        {
            await Task.Run(() => _examManager.UpdateExamPractice(request));
            return NoContent();
        }

        // Delete
        [HttpDelete("{id}")]
        public IActionResult DeleteExamPracticeById(string id)
        {
            var result = _examManager.DeleteExamPracticeById(id);

            if (result)
                return Ok(); // 200 OK
            else
                return NotFound(); // 404 if not found OR 500 if you want (but usually NotFound for missing items)
        }

        [HttpDelete]
        public IActionResult DeleteAllExamPractices()
        {
            var deletedCount = _examManager.DeleteAllExamPractices();

            if (deletedCount >= 0)
                return Ok(new { message = $"{deletedCount} exams deleted successfully." }); // 200 OK with message
            else
                return StatusCode(500, "An error occurred while deleting exams."); // 500 Internal Server Error
        }

    }
}

﻿using ContentService.Domain;
using ContentService.Domain.ExamComponents;
using ContentService.Helpers;
using ContentService.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContentService.Repositories
{
    public class ExamPracticeRepository : IExamPracticeRepository
    {
        private readonly Container _container;

        // Constructor to initialize the repository with a container
        public ExamPracticeRepository(CosmosDBConnection cosmosDBConnection, string databaseName, string containerName)
        {
            // Use CosmosDBConnection to get the container asynchronously
            _container = cosmosDBConnection.GetContainerAsync(databaseName, containerName).Result;
        }

        public async Task<string?> SaveExamPracticeAsync(ExamPractice exam)
        {
            try
            {
                // Use existing id or generate a new one if not provided
                if (string.IsNullOrWhiteSpace(exam.id))
                {
                    exam.id = Guid.NewGuid().ToString();
                }

                // Upsert (create or update)
                var response = await _container.UpsertItemAsync(exam, new PartitionKey(exam.id));

                Console.WriteLine($"Exam with id {exam.id} was upserted successfully. Status code: {response.StatusCode}");
                return exam.id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error upserting exam practice: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> SaveExamComponentAsync(IExamComponent component)
        {
            try
            {
                // Use existing id or generate a new one if not provided  
                if (string.IsNullOrWhiteSpace(component.id))
                {
                    component.id = Guid.NewGuid().ToString();
                }

                var componentType = component.GetComponentType().ToString();

                switch (componentType)
                {
                    case "Reading":
                        component = component as ReadingComponent ?? throw new InvalidCastException("Invalid ReadingComponent.");
                        break;
                    // Add more component types here as needed.  
                    default:
                        throw new NotSupportedException($"Unsupported component type: {componentType}");
                }

                if (string.IsNullOrWhiteSpace(component.ExamId))
                {
                    Console.WriteLine("Component is missing ExamId.");
                    return null;
                }

                var exam = await GetExamPracticeByIdAsync(component.ExamId);
                if (exam == null)
                {
                    Console.WriteLine($"No exam found with ID: {component.ExamId}");
                    return null;
                }

                // Ensure ExamComponents is initialized before adding the component  
                exam.ExamComponents ??= new List<IExamComponent>();

                // Check if component exists already
                var index = exam.ExamComponents.FindIndex(c => c.id == component.id);

                if (index >= 0)
                {
                    // Replace existing component
                    exam.ExamComponents[index] = component;
                }
                else
                {
                    // Add as new component
                    exam.ExamComponents.Add(component);
                }

                // Save updated exam using the centralized method  
                var savedId = await SaveExamPracticeAsync(exam);

                if (savedId == null)
                {
                    Console.WriteLine("Exam could not be updated with the new component.");
                    return null;
                }

                return component.id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving exam component: {ex.Message}");
                return null;
            }
        }

        public async Task<ExamPractice?> GetExamPracticeByIdAsync(string examId)
        {
            try
            {
                var response = await _container.ReadItemAsync<ExamPractice>(examId, new PartitionKey(examId));
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching exam with ID {examId}: {ex.Message}");
                return null;
            }
        }

        public async Task<IExamComponent?> GetComponentByIdAsync(string examId, string componentId)
        {
            var exam = await GetExamPracticeByIdAsync(examId);
            if (exam?.ExamComponents == null)
                return null;

            return exam.ExamComponents.FirstOrDefault(c => c.id == componentId);
        }

        public async Task<List<ExamPractice>> GetAllExamPracticesAsync()
        {
            var query = _container.GetItemQueryIterator<ExamPractice>(
                new QueryDefinition("SELECT * FROM c")
            );

            var results = new List<ExamPractice>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        // DELETE
        public async Task<int> DeleteAllExamPracticesAsync()
        {
            try
            {
                var query = _container.GetItemQueryIterator<ExamPractice>(
                    new QueryDefinition("SELECT * FROM c")
                );

                var examsToDelete = new List<ExamPractice>();

                while (query.HasMoreResults && examsToDelete.Count < 500)
                {
                    var response = await query.ReadNextAsync();
                    examsToDelete.AddRange(response);

                    // Stop if we've gathered 100 items
                    if (examsToDelete.Count >= 500)
                    {
                        examsToDelete = examsToDelete.Take(500).ToList();
                        break;
                    }
                }

                int deleteCount = examsToDelete.Count;

                var deleteTasks = examsToDelete.Select(exam =>
                    _container.DeleteItemAsync<ExamPractice>(exam.id, new PartitionKey(exam.id))
                );

                await Task.WhenAll(deleteTasks);

                Console.WriteLine($"Deleted {deleteCount} exams.");
                return deleteCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting first 500 exams: {ex.Message}");
                return -1;
            }
        }

        public async Task<bool> DeleteExamPracticeByIdAsync(string examId)
        {
            try
            {
                var response = await _container.DeleteItemAsync<ExamPractice>(examId, new PartitionKey(examId));
                Console.WriteLine($"Deleted exam with ID: {examId}, Status code: {response.StatusCode}");
                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine($"Exam with ID: {examId} not found.");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error while deleting exam: {ex.Message}");
                return false;
            }
        }
    }
}

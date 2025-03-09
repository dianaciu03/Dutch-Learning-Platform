using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentService.Domain;
using ContentService.DTOs;
using ContentService.ExerciseComponents;
using ContentService.Helpers;
using ContentService.Interfaces;

namespace ContentService.Managers
{
    public class ExerciseManager : IExerciseManager
    {
        private readonly List<Exercise> _exercises = [];

        // Create
        public bool CreateExercise(CreateExerciseRequest request)
        {
            try
            {
                var exerciseTypes = request.ExerciseTypes.Select(type => EnumConverter.ParseExerciseType(type)).ToList();
                var level = EnumConverter.ParseCEFRLevel(request.Level);
                var exercise = new Exercise(exerciseTypes, level, request.ExerciseComponents);
                _exercises.Add(exercise);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }

        // Read
        public Exercise GetExerciseById(int id)
        {
            return _exercises.FirstOrDefault(e => e.Id == id);
        }

        public List<Exercise> GetAllExercises()
        {
            return _exercises;
        }

        public ComponentResponse CreateExerciseComponent(CreateExerciseComponentRequest request)
        {
            ExerciseType type = EnumConverter.ParseExerciseType(request.ComponentType);

            IExerciseComponent component = type switch
            {
                ExerciseType.Reading => new ReadingComponent(),
                ExerciseType.Vocabulary => new VocabularyComponent(),
                ExerciseType.Listening => new ListeningComponent(),
                ExerciseType.Writing => new WritingComponent(),
                ExerciseType.Speaking => new SpeakingComponent(),
                ExerciseType.Grammar => new GrammarComponent(),
                _ => throw new ArgumentException($"Unsupported exercise type: {request.ComponentType}")
            };

            return new ComponentResponse { Component = component };
        }

        // Update
        public bool UpdateExercise(int id, List<ExerciseType> types, CEFRLevel level, IEnumerable<IExerciseComponent> exerciseComponents)
        {
            var exercise = GetExerciseById(id);
            if (exercise == null)
            {
                return false;
            }

            // Assuming Exercise has methods to update its properties
            exercise = new Exercise(types, level, exerciseComponents);
            return true;
        }

        // Delete
        public bool DeleteExercise(int id)
        {
            var exercise = GetExerciseById(id);
            if (exercise == null)
            {
                return false;
            }

            _exercises.Remove(exercise);
            return true;
        }
    }
}

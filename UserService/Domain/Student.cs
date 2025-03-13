namespace UserService.Domain
{
    public class Student : User
    {
        public List<int> CompletedExercises { get; set; } 

        public Student(int id, string firstName, string lastName, string username)
            : base(id, firstName, lastName, username)
        {
            CompletedExercises = [];
        }

        public override string GetUserRole() => "Student";
    }
}

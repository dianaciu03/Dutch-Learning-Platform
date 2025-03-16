namespace UserService.Domain
{
    public class StudentAccount : UserAccount
    {
        public List<int> CompletedExercises { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public StudentAccount(int id, string firstName, string lastName, string username)
            : base(id, firstName, lastName, username)
        {
            CompletedExercises = [];
        }

        public StudentAccount(string firstName, string lastName, string username)
            : base(firstName, lastName, username)
        {
            CompletedExercises = [];
        }

        public override string GetUserRole() => "Student";
    }
}

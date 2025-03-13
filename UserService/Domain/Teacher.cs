namespace UserService.Domain
{
    public class Teacher : User
    {
        public string EducationalInstitution { get; set; }
        public List<int> CreatedExercises { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public Teacher(int id, string firstName, string lastName, string username, string educationalInstitution) 
            : base(id, firstName, lastName, username)
        {
            EducationalInstitution = educationalInstitution;
            CreatedExercises = [];
        }

        public override string GetUserRole() => "Teacher";
    }
}

using Newtonsoft.Json;

namespace UserService.Domain
{

    public class TeacherAccount : UserAccount
    {
        public string EducationalInstitution { get; set; }
        public List<int> CreatedExercises { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        [JsonConstructor]
        public TeacherAccount(string id, string firstName, string lastName, string username, string educationalInstitution) 
            : base(id, firstName, lastName, username)
        {
            EducationalInstitution = educationalInstitution;
            CreatedExercises = [];
        }

        public TeacherAccount(string firstName, string lastName, string username, string educationalInstitution)
    : base(firstName, lastName, username)
        {
            EducationalInstitution = educationalInstitution;
            CreatedExercises = [];
        }

        public override string GetUserRole() => "Teacher";
    }
}

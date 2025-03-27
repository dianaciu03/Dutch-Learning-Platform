using UserService.Domain;

namespace UserService.Interfaces
{
    public interface IAccountRepository
    {
        Task SaveTeacherAccountAsync(TeacherAccount account);
        Task SaveStudentAccountAsync(StudentAccount account);
        Task<TeacherAccount> GetTeacherAccountByIdAsync(string id);
        Task<StudentAccount> GetStudentAccountByIdAsync(string id);
        Task<List<UserAccount>> GetAllAccountsAsync();
        Task<bool> DeleteAccountAsync(string id);
    }
}

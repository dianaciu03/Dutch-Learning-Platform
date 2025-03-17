using UserService.Domain;

namespace UserService.Interfaces
{
    public interface IAccountRepository
    {
        Task SaveTeacherAccountAsync(TeacherAccount account);
    }
}

using UserService.DTOs;

namespace UserService.Interfaces
{
    public interface IAccountManager
    {
        bool CreateTeacherAccount(CreateTeacherAccountRequest request);
        bool CreateStudentAccount(CreateStudentAccountRequest request);
        AccountResponse GetAllAccounts();
        AccountResponse GetAccountById(int id);
        bool DeleteAccount(int id);

    }
}

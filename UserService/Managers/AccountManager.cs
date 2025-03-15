using UserService.Domain;
using UserService.DTOs;
using UserService.Helpers;
using UserService.Interfaces;

namespace UserService.Managers
{
    public class AccountManager
    {
        private readonly List<UserAccount> _accounts = new List<UserAccount>();
        private readonly LogHelper<AccountManager> _logger;
        private readonly IAccountRepository _accountRepository;

        public AccountManager(LogHelper<AccountManager> logger, IAccountRepository accountRepository)
        {
            _logger = logger;
            _accountRepository = accountRepository;
        }

        // Create Teacher Account
        public bool CreateTeacherAccount(CreateTeacherAccountRequest request)
        {
            try
            {
                var teacher = new TeacherAccount(
                    request.FirstName,
                    request.LastName,
                    request.Username,
                    request.EducationalInstitution
                );

                teacher.Email = request.Email;
                teacher.Password = request.Password;

                _accounts.Add( teacher );

                _logger.LogInfo("Created new teacher account: {0}", teacher.Username);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating teacher account", ex);
                return false;
            }
        }

        // Create Student Account
        public bool CreateStudentAccount(CreateStudentAccountRequest request)
        {
            try
            {
                var student = new StudentAccount(
                    request.FirstName,
                    request.LastName,
                    request.Username
                );
                    
                student.Email = request.Email;
                student.Password = request.Password;

                _logger.LogInfo("Creating new student account: {0}", student.Username);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating student account", ex);
                return false;
            }
        }
    }
}

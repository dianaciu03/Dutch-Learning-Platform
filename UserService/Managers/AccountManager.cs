using UserService.Domain;
using UserService.DTOs;
using UserService.Helpers;
using UserService.Interfaces;

namespace UserService.Managers
{
    public class AccountManager : IAccountManager
    {
        private readonly List<UserAccount> _accounts = new List<UserAccount>();
        private readonly LogHelper<AccountManager> _logger;
        //private readonly IAccountRepository _accountRepository;

        public AccountManager(LogHelper<AccountManager> logger)
        {
            _logger = logger;
            //_accountRepository = accountRepository;
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

        // Get Account By ID
        public AccountResponse GetAccountById(int id)
        {
            try
            {
                _logger.LogInfo("Fetching account with ID: {0}", id);
                var account = _accounts.FirstOrDefault(e => e.Id == id);
                return new AccountResponse { AccountList = new List<UserAccount> { account } };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while fetching account.", ex);
                return new AccountResponse { AccountList = new List<UserAccount>() };
            }
        }

        // Get All Accounts
        public AccountResponse GetAllAccounts()
        {
            try
            {
                return new AccountResponse { AccountList = _accounts };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while getting all accounts.", ex);
                return new AccountResponse { AccountList = new List<UserAccount>() };
            }
        }


        // Delete Account
        public bool DeleteAccount(int id)
        {
            try
            {
                var account = _accounts.FirstOrDefault(e => e.Id == id);
                if (account == null)
                {
                    _logger.LogWarning("Account could not be found.");
                    return false;
                }
                _accounts.Remove(account);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while deleting exam.", ex);
                return false;
            }
        }
    }
}

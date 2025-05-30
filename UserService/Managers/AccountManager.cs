using UserService.Domain;
using UserService.DTOs;
using UserService.Interfaces;
using Serilog;
using UserService.Helpers;

namespace UserService.Managers
{
    public class AccountManager : IAccountManager
    {
        private readonly RabbitMQConnection _rabbitMqConnection;
        private readonly IAccountRepository _accountRepository;

        public AccountManager(RabbitMQConnection rabbitMqConnection, IAccountRepository accountRepository)
        {
            _rabbitMqConnection = rabbitMqConnection;
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

                _accountRepository.SaveTeacherAccountAsync(teacher);

                Log.Information("Created new teacher account: {Username}", teacher.Username);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating teacher account");
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

                _accountRepository.SaveStudentAccountAsync(student);

                Log.Information("Creating new student account: {Username}", student.Username);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating student account");
                return false;
            }
        }

        // Get Account By ID
        public AccountResponse GetTeacherAccountById(string id)
        {
            try
            {
                Log.Information("Fetching account with ID: {AccountId}", id);

                TeacherAccount account = _accountRepository.GetTeacherAccountByIdAsync(id.ToString()).GetAwaiter().GetResult();
                UserAccount userAccount = account as UserAccount;

                if (account == null)
                {
                    return new AccountResponse { AccountList = new List<UserAccount>() };
                }

                return new AccountResponse { AccountList = new List<UserAccount> { userAccount } };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while fetching account");
                return new AccountResponse { AccountList = new List<UserAccount>() };
            }
        }

        public AccountResponse GetStudentAccountById(string id)
        {
            try
            {
                Log.Information("Fetching account with ID: {AccountId}", id);

                StudentAccount account = _accountRepository.GetStudentAccountByIdAsync(id.ToString()).GetAwaiter().GetResult();
                UserAccount userAccount = account as UserAccount;

                if (account == null)
                {
                    return new AccountResponse { AccountList = new List<UserAccount>() };
                }

                return new AccountResponse { AccountList = new List<UserAccount> { userAccount } };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while fetching account");
                return new AccountResponse { AccountList = new List<UserAccount>() };
            }
        }

        // Get All Accounts
        public AccountResponse GetAllAccounts()
        {
            try
            {
                var accounts = _accountRepository.GetAllAccountsAsync().GetAwaiter().GetResult();
                return new AccountResponse { AccountList = accounts };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while getting all accounts");
                return new AccountResponse { AccountList = new List<UserAccount>() };
            }
        }

        // Delete Account
        public bool DeleteAccount(string id)
        {
            try
            {
                Log.Information("Deleting account with ID: {AccountId}", id);

                var success = _accountRepository.DeleteAccountAsync(id).Result;

                if (success)
                {
                    // Publish message to RabbitMQ
                    _rabbitMqConnection.PublishMessage($"Account to delete:{id}", "accountQueue");
                    Log.Information("Published delete message for account: {AccountId}", id);
                }

                return success;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while deleting account");
                return false;
            }
        }
    }
}

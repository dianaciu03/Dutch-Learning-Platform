using UserService.Domain;
using UserService.DTOs;
using UserService.Helpers;
using UserService.Interfaces;

namespace UserService.Managers
{
    public class AccountManager : IAccountManager
    {
        private readonly RabbitMQConnection _rabbitMqConnection;
        private readonly LogHelper<AccountManager> _logger;
        private readonly IAccountRepository _accountRepository;

        public AccountManager(RabbitMQConnection rabbitMqConnection, IAccountRepository accountRepository)
        {
            _rabbitMqConnection = rabbitMqConnection;
            _logger = new LogHelper<AccountManager>();
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

                _logger.LogInfo("Created new teacher account: {0}", teacher.Username);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating teacher account.", ex);
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
        public AccountResponse GetTeacherAccountById(string id)
        {
            try
            {
                _logger.LogInfo("Fetching account with ID: {0}", id);

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
                _logger.LogError("Error while fetching account.", ex);
                return new AccountResponse { AccountList = new List<UserAccount>() };
            }
        }

        public AccountResponse GetStudentAccountById(string id)
        {
            try
            {
                _logger.LogInfo("Fetching account with ID: {0}", id);

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
                _logger.LogError("Error while fetching account.", ex);
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
                _logger.LogError("Error while getting all accounts.", ex);
                return new AccountResponse { AccountList = new List<UserAccount>() };
            }
        }


        // Delete Account
        public bool DeleteAccount(string id)
        {
            try
            {
                // Prepare a message that will be sent to RabbitMQ
                var message = $"Account to delete: {id}";

                // Assuming you have an instance of RabbitMQConnection, e.g. _rabbitMqConnection
                _rabbitMqConnection.PublishMessage(message, "accountQueue");

                var success = _accountRepository.DeleteAccountAsync(id.ToString()).GetAwaiter().GetResult();

                if (!success)
                {
                    _logger.LogWarning("Account could not be found.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while deleting account.", ex);
                return false;
            }
        }
    }
}

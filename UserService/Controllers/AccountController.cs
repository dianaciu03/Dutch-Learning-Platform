using Microsoft.AspNetCore.Mvc;
using UserService.DTOs;
using UserService.Interfaces;
using UserService.Managers;
using Serilog;

namespace UserService.Controllers
{
    [ApiController]
    [Route("accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountManager _accountManager;

        public AccountController(IAccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        // Create Teacher Account
        [HttpPost("teacher")]
        public async Task<IActionResult> CreateTeacherAccount([FromBody] CreateTeacherAccountRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            bool created = await Task.Run(() => _accountManager.CreateTeacherAccount(request));
            return created ? StatusCode(201, "Teacher account created successfully.") : StatusCode(500, "Error creating teacher account.");
        }

        // Create Student Account
        [HttpPost("student")]
        public async Task<IActionResult> CreateStudentAccount([FromBody] CreateStudentAccountRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            bool created = await Task.Run(() => _accountManager.CreateStudentAccount(request));
            return created ? StatusCode(201, "Student account created successfully.") : StatusCode(500, "Error creating student account.");
        }

        // Get Account By ID
        [HttpGet("teacher/{id}")]
        public async Task<IActionResult> GetTeacherById(string id)
        {
            try
            {
                var account = await Task.Run(() => _accountManager.GetTeacherAccountById(id));
                if (account.AccountList == null || !account.AccountList.Any())
                {
                    return NotFound($"No teacher account found with ID: {id}");
                }
                return Ok(account);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching the teacher account with ID: {AccountId}", id);
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpGet("student/{id}")]
        public async Task<IActionResult> GetStudentById(string id)
        {
            try
            {
                var account = await Task.Run(() => _accountManager.GetStudentAccountById(id));
                if (account.AccountList == null || !account.AccountList.Any())
                {
                    return NotFound($"No student account found with ID: {id}");
                }
                return Ok(account);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while fetching the student account with ID: {AccountId}", id);
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        // Get All Accounts
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await Task.Run(() => _accountManager.GetAllAccounts()));

        // Delete Account
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            try
            {
                bool deleted = await Task.Run(() => _accountManager.DeleteAccount(id));
                if (!deleted)
                {
                    return NotFound($"No account found with ID: {id}");
                }
                return Ok($"Account with ID: {id} has been deleted successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while deleting the account with ID: {AccountId}", id);
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }
    }
}

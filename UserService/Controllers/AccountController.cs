using Microsoft.AspNetCore.Mvc;
using UserService.DTOs;
using UserService.Helpers;
using UserService.Interfaces;
using UserService.Managers;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountManager _accountManager;
        private readonly LogHelper<AccountController> _logger;

        public AccountController(IAccountManager accountManager, LogHelper<AccountController> logger)
        {
            _accountManager = accountManager;
            _logger = logger;
        }

        // Create Teacher Account
        [HttpPost("accounts/teacher")]
        public async Task<IActionResult> CreateTeacherAccount([FromBody] CreateTeacherAccountRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            bool created = await Task.Run(() => _accountManager.CreateTeacherAccount(request));
            return created ? StatusCode(201, "Teacher account created successfully.") : StatusCode(500, "Error creating teacher account.");
        }

        // Create Student Account
        [HttpPost("accounts/student")]
        public async Task<IActionResult> CreateStudentAccount([FromBody] CreateStudentAccountRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            bool created = await Task.Run(() => _accountManager.CreateStudentAccount(request));
            return created ? StatusCode(201, "Student account created successfully.") : StatusCode(500, "Error creating student account.");
        }

        // Get Account By ID
        [HttpGet("accounts/teacher/{id}")]
        public async Task<IActionResult> GetTeacherById(string id)
        {
            try
            {
                var account = await Task.Run(() => _accountManager.GetTeacherAccountById(id));

                if (account == null)
                {
                    return NotFound("Account not found.");
                }

                return Ok(account);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching the teacher account with ID {id}: {ex.Message}");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // Get Account By ID
        [HttpGet("accounts/student/{id}")]
        public async Task<IActionResult> GetStudentById(string id)
        {
            try
            {
                var account = await Task.Run(() => _accountManager.GetStudentAccountById(id));

                if (account == null)
                {
                    return NotFound("Account not found.");
                }

                return Ok(account);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while fetching the teacher account with ID.", ex);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // Get All Accounts
        [HttpGet("accounts")]
        public async Task<IActionResult> GetAll()
            => Ok(await Task.Run(() => _accountManager.GetAllAccounts()));

        // Delete Account
        [HttpDelete("accounts/{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            try
            {
                bool deleted = await Task.Run(() => _accountManager.DeleteAccount(id));

                if (deleted)
                {
                    return NoContent(); // 204 response, no body
                }

                return NotFound("Account not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while deleting the account with ID: {0}", ex);
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }
    }
}

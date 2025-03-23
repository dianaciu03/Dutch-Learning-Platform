using Microsoft.AspNetCore.Mvc;
using UserService.DTOs;
using UserService.Interfaces;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountController(IAccountManager accountManager) : ControllerBase
    {
        private readonly IAccountManager _accountManager = accountManager;

        // Create Teacher Account
        [HttpPost("teacher")]
        public async Task<IActionResult> CreateTeacherAccount([FromBody] CreateTeacherAccountRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            bool created = await Task.Run(() => _accountManager.CreateTeacherAccount(request));
            return created ? Ok("Teacher account created successfully.") : StatusCode(500, "Error creating teacher account.");
        }

        // Create Student Account
        [HttpPost("student")]
        public async Task<IActionResult> CreateStudentAccount([FromBody] CreateStudentAccountRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            bool created = await Task.Run(() => _accountManager.CreateStudentAccount(request));
            return created ? Ok("Student account created successfully.") : StatusCode(500, "Error creating student account.");
        }

        // Get Account By ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var account = await Task.Run(() => _accountManager.GetAccountById(id));
            return (account.AccountList.Count > 0) ? Ok(account) : NotFound("Account not found.");
        }

        // Get All Accounts
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await Task.Run(() => _accountManager.GetAllAccounts()));

        // Delete Account
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            bool deleted = await Task.Run(() => _accountManager.DeleteAccount(id));
            return deleted ? NoContent() : NotFound("Account not found.");
        }
    }
}

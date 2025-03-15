using UserService.Domain;

namespace UserService.DTOs
{
    public class AccountResponse
    {
        public List<UserAccount>? AccountList { get; set; }
    }
}

namespace UserService.Domain
{
    public abstract class UserAccount
    {
        public string id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }

        public string Role { get; set; }

        protected UserAccount(string id, string firstName, string lastName, string username)
        {
            this.id = id;
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            Role = GetUserRole();
        }

        protected UserAccount(string firstName, string lastName, string username)
        {
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            Role = GetUserRole();
        }

        protected UserAccount()
        {

        }

        public abstract string GetUserRole();
    }
}

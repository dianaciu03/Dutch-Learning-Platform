namespace UserService.Domain
{
    public abstract class UserAccount
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }

        protected UserAccount(int id, string firstName, string lastName, string username)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Username = username;
        }

        protected UserAccount(string firstName, string lastName, string username)
        {
            FirstName = firstName;
            LastName = lastName;
            Username = username;
        }

        protected UserAccount()
        {

        }

        public abstract string GetUserRole();
    }
}

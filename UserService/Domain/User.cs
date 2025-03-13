namespace UserService.Domain
{
    public abstract class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }

        protected User(int id, string firstName, string lastName, string username)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Username = username;
        }

        protected User()
        {

        }

        public abstract string GetUserRole();
    }
}

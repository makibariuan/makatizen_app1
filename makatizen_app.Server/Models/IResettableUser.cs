namespace makatizen_app.Server.Models
{
    // Define the interface that both UsersSystem and UsersKit must implement
    public interface IResettableUser
    {
        int Id { get; set; }
        string Username { get; set; } // Needed for context/lookup
        string PasswordHash { get; set; }
        bool MustResetPassword { get; set; }
    }
}
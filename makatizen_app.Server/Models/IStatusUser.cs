namespace makatizen_app.Server.Models
{
    /// <summary>
    /// Interface to enforce common properties for user models (UsersSystem and UsersKit)
    /// that are necessary for status management (activation/deactivation).
    /// </summary>
    public interface IStatusUser
    {
        // Unique identifier for the user
        int Id { get; }

        // Username for identification in response messages
        string Username { get; set; }

        // Status flag to lock/unlock the account
        bool IsActive { get; set; }
    }

    /// <summary>
    /// NOTE: Ensure your UsersSystem and UsersKit classes implement this interface (and IResettableUser) 
    /// for the AdminController's generic methods to work correctly.
    /// </summary>
}
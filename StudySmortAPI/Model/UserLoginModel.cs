namespace StudySmortAPI.Model;

public class UserLoginModel
{
    public string Email { get; init; }
    public string Password { get; init; }

    public UserLoginModel(string email, string password)
    {
        Email = email;
        Password = password;
    }
}
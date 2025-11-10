using System;

public class UserModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }

    // Private helper method to clean and normalize strings, capitalizing the first letter and lowercasing the rest
    private string CleanString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        value = value.Trim();

        return char.ToUpper(value[0]) + value.Substring(1).ToLower();
    }

    // Private helper method to normalize email addresses to lowercase
    private string NormalizeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return string.Empty;

        return email.Trim().ToLower();
    }

    // Default constructor
    public UserModel()
    {
        Id = 0;
        Name = string.Empty;
        Lastname = string.Empty;
        Email = string.Empty;
    }

	// Constructor using private methods
	public UserModel(int id, string name, string lastname, string email)
	{
		Id = id;
		Name = CleanString(name);
		Lastname = CleanString(lastname);
		Email = NormalizeEmail(email);
	}

    // Copying a constructor 
    public UserModel(UserModel test)
    {
        if (test == null)
        {
            throw new ArgumentNullException(nameof(test));
        }

        Id = test.Id;
        Name = test.Name;
        Lastname = test.Lastname;
        Email = test.Email;
    }
}

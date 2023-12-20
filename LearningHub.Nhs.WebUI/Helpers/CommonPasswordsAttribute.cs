// <copyright file="CommonPasswordsAttribute.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>
namespace LearningHub.Nhs.WebUI.Helpers
{
  using System.ComponentModel.DataAnnotations;

  /// <summary>
  /// Check password against a list of common passwords.
  /// </summary>
  public class CommonPasswordsAttribute : ValidationAttribute
  {
    // Password list taken from here https://github.com/danielmiessler/SecLists/blob/master/Passwords/Common-Credentials/top-passwords-shortlist.txt
    private static readonly string[] commonPasswords =
    {
              "password",
              "123456",
              "12345678",
              "abc123",
              "querty",
              "monkey",
              "letmein",
              "dragon",
              "111111",
              "baseball",
              "iloveyou",
              "trustno1",
              "1234567",
              "sunshine",
              "master",
              "123123",
              "welcome",
              "shadow",
              "ashley",
              "footbal",
              "jesus",
              "michael",
              "ninja",
              "mustang",
              "password1",
    };

    private readonly string? errorMessage;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommonPasswordsAttribute"/> class.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    public CommonPasswordsAttribute(string? errorMessage = null)
    {
      this.errorMessage = errorMessage;
    }

    /// <summary>
    /// Check if password is in the list of common passwords.
    /// </summary>
    /// <param name="value">The password to check.</param>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>ValidationResult Success if password is valid.</returns>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      if (value == null || (string)value == string.Empty)
      {
        return ValidationResult.Success;
      }

      string lowerCasePassword = value.ToString().ToLower();
      foreach (var commonPassword in commonPasswords)
      {
        if (lowerCasePassword.Contains(commonPassword))
        {
          return new ValidationResult(this.errorMessage);
        }
      }

      return ValidationResult.Success;
    }
  }
}

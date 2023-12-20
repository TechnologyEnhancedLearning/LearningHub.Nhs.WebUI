// <copyright file="CommonValidationValues.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Helpers
{
  /// <summary>
  /// Defines the <see cref="CommonValidationValues" />.
  /// </summary>
  public static class CommonValidationValues
  {
    /// <summary>
    /// second security question Success Message.
    /// </summary>
    public const int PasswordMaxLength = 50;

    /// <summary>
    /// second security question Success Message.
    /// </summary>
    public const int PasswordMinLength = 8;

    /// <summary>
    /// second security question Success Message.
    /// </summary>
    public const string PasswordRegularExpression = @"(?=.*?[^\w\s])(?=.*?[0-9])(?=.*?[A-Z])(?=.*?[a-z]).*";
  }
}

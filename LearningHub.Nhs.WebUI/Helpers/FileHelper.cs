namespace LearningHub.Nhs.WebUI.Helpers
{
  using Microsoft.AspNetCore.StaticFiles;

  /// <summary>
  /// FileHelper.
  /// </summary>
  public static class FileHelper
  {
    /// <summary>
    /// GetContentTypeFromFileName.
    /// </summary>
    /// <param name="fileName">fileName.</param>
    /// <returns>return file stream.</returns>
    public static string? GetContentTypeFromFileName(string fileName)
    {
      return new FileExtensionContentTypeProvider().TryGetContentType(fileName, out var contentType)
          ? contentType
          : null;
    }
  }
}

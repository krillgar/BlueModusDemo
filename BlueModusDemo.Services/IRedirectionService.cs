namespace BlueModusDemo.Services;

/// <summary>
/// An interface for retrieving redirection settings.
/// </summary>
internal interface IRedirectionService
{
    /// <summary>
    /// A method to retrieve the current redirection settings.
    /// </summary>
    /// <returns></returns>
    Task<string> GetRedirections();
}

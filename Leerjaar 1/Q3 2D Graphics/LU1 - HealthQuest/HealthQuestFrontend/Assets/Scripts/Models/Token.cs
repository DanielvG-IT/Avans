using System;

/// <summary>
/// Represents an authentication token with its details.
/// </summary>
[Serializable]
public class Token
{
    /// <summary>
    /// Gets or sets the type of the token.
    /// </summary>
    public string tokenType;

    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    public string accessToken;

    /// <summary>
    /// Gets or sets the duration in seconds for which the token is valid.
    /// </summary>
    public int expiresIn;

    /// <summary>
    /// Gets or sets the refresh token used to obtain a new access token.
    /// </summary>
    public string refreshToken;
}

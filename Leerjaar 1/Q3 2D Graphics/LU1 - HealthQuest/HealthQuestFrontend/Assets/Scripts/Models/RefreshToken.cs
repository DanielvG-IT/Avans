using System;

/// <summary>
/// Represents an authentication token with refresh token details.
/// </summary>
[Serializable]
public class RefreshToken
{
    /// <summary>
    /// Gets or sets the refresh token used to obtain a new access token.
    /// </summary>
    public string refreshToken;
}

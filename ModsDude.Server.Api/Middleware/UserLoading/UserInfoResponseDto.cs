using System.Text.Json.Serialization;

namespace ModsDude.Server.Api.Middleware.CreateUser;

public record UserInfoResponseDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("given_name")] string GivenName,
    [property: JsonPropertyName("family_name")] string FamilyName,
    [property: JsonPropertyName("nickname")] string Nickname,
    [property: JsonPropertyName("preferred_username")] string PreferredUsername,
    [property: JsonPropertyName("picture")] string Picture,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("email_verified")] bool EmailVerified
);

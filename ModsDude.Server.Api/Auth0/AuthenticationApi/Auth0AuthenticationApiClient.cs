namespace ModsDude.Server.Api.Auth0.AuthenticationApi;

public class Auth0AuthenticationApiClient
{
    private readonly HttpClient _httpClient;
    private readonly HttpContext _httpContext;


    public Auth0AuthenticationApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContext = httpContextAccessor.HttpContext!;

        _httpClient.BaseAddress = new Uri("https://modsdude-dev.eu.auth0.com");
    }


    public async Task<UserInfoResponseDto> GetUserInfo()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/userinfo");
        request.Headers.Add("Authorization", _httpContext.Request.Headers.Authorization.First());

        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<UserInfoResponseDto>()
            ?? throw new Exception("Error creating user: User info is null");
    }
}

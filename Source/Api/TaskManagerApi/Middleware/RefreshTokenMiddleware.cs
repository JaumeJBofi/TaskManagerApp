using TaskManagerApi.Repositories.Interfaces;
using TaskManagerApi.Services;

namespace TaskManagerApi.Middleware
{
    public class RefreshTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUserRepository _userRepository;

        public RefreshTokenMiddleware(RequestDelegate next, IJwtTokenService jwtTokenService, IUserRepository userRepository)
        {
            _next = next;
            _jwtTokenService = jwtTokenService;
            _userRepository = userRepository;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the request has an Authorization header with a Bearer token
            var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (authorizationHeader != null && authorizationHeader.StartsWith("Bearer "))
            {
                var accessToken = authorizationHeader.Substring("Bearer ".Length);

                // Check if the access token is expired
                if (_jwtTokenService.IsAccessTokenExpired(accessToken)) // You'll need to implement this in your JwtTokenService
                {
                    // Get the refresh token from the cookie
                    if (context.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
                    {
                        // Validate the refresh token
                        var id = _jwtTokenService.GetUserIdFromToken(accessToken); // Extract username from expired token
                        if (id != null)
                        {
                            var user = await _userRepository.GetByIdAsync(id);
                            if (user != null && await _userRepository.ValidateRefreshToken(user.UserName, refreshToken))
                            {
                                // Generate a new access token                            
                                if (user != null)
                                {
                                    var newAccessToken = _jwtTokenService.GenerateAccessToken(user);

                                    // Set the new access token in the response header
                                    context.Response.Headers["AccessRefresh"] = newAccessToken;
                                    context.Request.Headers["Authorization"] = "Bearer " + newAccessToken;
                                }
                            }
                        }
                    }
       
                }
            }

            // Continue to the next middleware in the pipeline
            await _next(context);
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using src.Features.Auth.Dtos;
using src.Features.Auth.Dtos.Response;
using src.Features.Auth.Interfaces;
using src.Features.Auth.Token;
using src.features.user.entities;
using DbContext = src.contexts.DbContext;

namespace src.Features.Auth.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly DbContext _dbContext;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;
    private readonly IMapper _mapper;

    public AuthService(
        UserManager<User> userManager,
        ILogger<AuthService> logger,
        DbContext dbContext,
        ITokenService tokenService,
        IMapper mapper
    )
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _tokenService = tokenService;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<RegisterUserDto> RegisterUser(RegisterUserDto dto)
    {
        var isUserExist = await _userManager.FindByEmailAsync(dto.Email);

        if (isUserExist != null)
        {
            throw new Exception("User is already registered.");
        }

        //dto => User entity...
        var user = _mapper.Map<User>(dto);

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            foreach (var errors in result.Errors)
            {
                _logger.LogError($"Registration error : {errors.Description}");
            }
        }

        var mappedUser = _mapper.Map<RegisterUserDto>(user);

        return mappedUser;
    }

    public async Task<TokenResponse> LoginUser(LoginUserDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var result = await _userManager.CheckPasswordAsync(user, dto.Password);

        if (!result)
        {
            throw new Exception("Invalid password.");
        }

        var accessToken = _tokenService.GenerateToken(user);

        //buraya mapper gerekli olabilir belki.
        var refreshToken = new RefreshToken
        {
            Token = _tokenService.GenerateRefreshToken(),
            UserId = user.Id,
            ExpiresOnUtc = DateTime.UtcNow.AddDays(3),
            Id = Guid.NewGuid(),
        };

        _dbContext.RefreshTokens.Add(refreshToken);

        await _dbContext.SaveChangesAsync();

        return new TokenResponse { AccessToken = accessToken, RefreshToken = refreshToken.Token };
    }

    public async Task<TokenRequest> LoginWithRefreshToken(TokenRequest request)
    {
        var refreshToken = await _dbContext
            .RefreshTokens.Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken);

        if (refreshToken == null)
        {
            throw new Exception("Invalid refresh token.");
        }

        if (refreshToken.ExpiresOnUtc < DateTime.UtcNow)
        {
            throw new Exception("Refresh token expired.");
        }

        string accessToken = _tokenService.GenerateToken(refreshToken.User);

        refreshToken.Token = _tokenService.GenerateRefreshToken();

        refreshToken.ExpiresOnUtc = DateTime.UtcNow.AddDays(3);

        await _dbContext.SaveChangesAsync();

        return new TokenRequest { RefreshToken = refreshToken.Token };
    }
}

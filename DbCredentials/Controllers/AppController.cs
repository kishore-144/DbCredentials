using DbCredentials.DTOs;
using DbCredentials.Repositories;
using DbCredentials.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repo;
    private readonly TokenService _tokenService;

    public UsersController(IUserRepository repo, TokenService tokenService)
    {
        _repo = repo;
        _tokenService = tokenService;
    }

    [HttpPost("signup")]
    public async Task<ActionResult<ApiResponse>> Signup([FromBody] SignupDto dto)
    {
        try
        {
            Console.WriteLine("1");
            //if (!ModelState.IsValid) return BadRequest(ModelState);

            Console.WriteLine("1");
            if (await _repo.CheckEmailExistsAsync(dto.email))
                return Ok(new ApiResponse
                {
                    status = "Failure",
                    message = "Email already exists with another account"
                });

            Console.WriteLine("1");
            var response = await _repo.CreateUserAsync(dto);

            Console.WriteLine("1");
            return Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine("2");
            return StatusCode(500, new ApiResponse
            {
                status = "Failure",
                message = $"Unexpected error in Signup: {ex.Message}"
            });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _repo.GetByEmailAsync(dto.email);
            if (user == null || !await _repo.VerifyPasswordAsync(user, dto.password))
                return Unauthorized(new AuthResponseDto
                {
                    token = null,
                    status = "Failure",
                    message = "Invalid Credentials"
                });

            var (token, expires) = _tokenService.CreateToken(user);

            return Ok(new AuthResponseDto
            {
                token = token,
                status = "Success",
                message = "Logged In"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new AuthResponseDto
            {
                token = null,
                status = "Failure",
                message = $"Unexpected error in Login: {ex.Message}"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse>> GetById(int id)
    {
        try
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null)
                return NotFound(new ApiResponse
                {
                    status = "Failure",
                    message = "User not found"
                });

            return Ok(new ApiResponse
            {
                status = "Success",
                message = "User retrieved"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse
            {
                status = "Failure",
                message = $"Unexpected error in GetById: {ex.Message}"
            });
        }
    }
}

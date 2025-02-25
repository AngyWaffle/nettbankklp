using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;

//User API point
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    //Defines the db context
    private readonly ApplicationDbContext _context;
    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }
    //Create user API point
    [HttpPost("CreateUser")]
    public async Task<IActionResult> AddUser(Users user)
    {
        //Checks if E-Mail already exists
        if (await _context.Users.AnyAsync(u => u.Mail == user.Mail))
        {
            return BadRequest(new { message = "Email already exists" });
        }
        try
        {
            //Generate salt for hashing password
            var salt = PasswordHash.GenerateSalt();
            //Hashes password
            user.Password = PasswordHash.HashPassword(user.Password, salt);
            //Stores salt in user table
            user.Salt = salt;
            //Saves the user to the db
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            var Sparekonto = new AccountDto
            {
                AccountNumber = 0,
                UserId = user.UserId,
                AccountType = "Sparekonto"
            };
            var Brukskonto = new AccountDto
            {
                AccountNumber = 0,
                UserId = user.UserId,
                AccountType = "Brukskonto"
            };
            var accountController = new AccountController(_context); // Pass the DbContext
            accountController.AddAccount(Sparekonto);
            accountController.AddAccount(Brukskonto);
            await _context.SaveChangesAsync();
            return Ok(true);
        }
        catch (Exception ex)
        {
            // Log the actual exception for debugging purposes
            Console.WriteLine(ex.Message);

            // Return a general error message to the client
            return StatusCode(500, new { message = "An error occurred while creating the user." });
        }
    }
    //Login API point
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Retrieve the user by username
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Mail == request.Mail);
        if (user == null || !PasswordHash.VerifyPassword(request.Password, user.Password, user.Salt))
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        return Ok(user.UserId);
    }
}

//Class for login requests
public class LoginRequest
{
    public string Mail { get; set; }
    public string Password { get; set; }
}
//User API point
using Microsoft.AspNetCore.Mvc;

//Account API point
[Route("api/account/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    //Defines the db context
    private readonly ApplicationDbContext _context;
    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }
}
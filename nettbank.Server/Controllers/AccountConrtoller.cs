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
    [HttpPost("CreateAccount")]
    public async Task<IActionResult> AddAccount([FromBody] AccountDto accountDto)
    {
        // Generate a unique account number
        long accountNumber;
        do
        {
            // Generate a random 11-digit number using Random and scaling
            long min = 10_000_000_000L; // Minimum 11-digit number
            long max = 99_999_999_999L; // Maximum 11-digit number

            Random random = new Random();
            accountNumber = (long)(random.NextDouble() * (max - min + 1)) + min;

        } while (_context.Accounts.Any(a => a.AccountNumber == accountNumber)); // Check for uniqueness

        // Assign the unique account number to the account
        accountDto.AccountNumber = accountNumber;

        // Map AccountDto to Accounts
        var account = new Accounts
        {
            AccountNumber = accountDto.AccountNumber,
            UserId = accountDto.UserId,
            AccountType = accountDto.AccountType
        };

        var accountBalance = new AccountBalance
        {
            AccountNumber = accountDto.AccountNumber,
            Balance = 0
        };

        // Create the account
        try{
            _context.Accounts.Add(account);
            _context.AccountBalance.Add(accountBalance);
            await _context.SaveChangesAsync();
            return Ok(true);
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost("Transaction")]
    public async Task<IActionResult> Transaction([FromBody] TransactionDto transactionDto)
    {
        // Map TransactionDto to Transactions
        var transaction = new Transactions
        {
            Id = transactionDto.Id,
            AccountNumber = transactionDto.AccountNumber,
            AccountNumberReceived = transactionDto.AccountNumberReceived,
            Amount = transactionDto.Amount,
            Type = transactionDto.Type,
            Date = transactionDto.Date
        };

        // Update the account balance
        var accountBalance = _context.AccountBalance.FirstOrDefault(a => a.AccountNumber == transactionDto.AccountNumberReceived);
        var accountBalance2 = _context.AccountBalance.FirstOrDefault(a => a.AccountNumber == transactionDto.AccountNumber);
        if (accountBalance != null&&accountBalance2!=null)
        {
            accountBalance.Balance += transactionDto.Amount;
            accountBalance2.Balance -= transactionDto.Amount;
        }
        else
        {
            return BadRequest("Account not found");
        }

        // Create the transaction
        try{
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return Ok(true);
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    // Get all transactions for an account
    [HttpGet("GetTransactions/{accountNumber}")]
    public async Task<IActionResult> GetTransactions(long accountNumber)
    {
        try
        {
            // Get all transactions for the account
            var transactions = _context.Transactions.Where(t => t.AccountNumber == accountNumber || t.AccountNumberReceived == accountNumber).ToList();
            return Ok(transactions);
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    // Get the account balance
    [HttpGet("GetBalance/{accountNumber}")]
    public async Task<IActionResult> GetBalance(long accountNumber)
    {
        try
        {
            // Get the account balance
            var accountBalance = _context.AccountBalance.FirstOrDefault(a => a.AccountNumber == accountNumber);
            return Ok(accountBalance);
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

//Input format for creating an account
public class AccountDto
{
    public long AccountNumber { get; set; }
    public int UserId { get; set; }
    public string AccountType { get; set; }
}

//Input format for a transaction
public class TransactionDto
{
    public int Id { get; set; }
    public long AccountNumber { get; set; }
    public long AccountNumberReceived { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; }
    public DateTime Date { get; set; }
}
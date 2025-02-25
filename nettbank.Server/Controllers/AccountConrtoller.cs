using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//Account API point
[Route("api/[controller]")]
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
            Balance = 1000 //Should be 0, 1000 for testing
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
        // Validate input
        if (transactionDto.AccountNumber == 0 || transactionDto.AccountNumberReceived == 0 || transactionDto.Amount <= 0)
        {
            return BadRequest("Invalid transaction details. Please provide valid account numbers and amount.");
        }

        // Fetch sender and receiver accounts
        var senderAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == transactionDto.AccountNumber);
        var receiverAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == transactionDto.AccountNumberReceived);

        if (senderAccount == null || receiverAccount == null)
        {
            return BadRequest("One or both accounts do not exist.");
        }

        // Check sender's balance
        var senderBalance = await _context.AccountBalance.FirstOrDefaultAsync(ab => ab.AccountNumber == senderAccount.AccountNumber);
        if (senderBalance == null || senderBalance.Balance < transactionDto.Amount)
        {
            return BadRequest("Insufficient funds.");
        }

        // Update balances
        senderBalance.Balance -= transactionDto.Amount;
        var receiverBalance = await _context.AccountBalance.FirstOrDefaultAsync(ab => ab.AccountNumber == receiverAccount.AccountNumber);
        if (receiverBalance == null)
        {
            receiverBalance = new AccountBalance { AccountNumber = receiverAccount.AccountNumber, Balance = transactionDto.Amount };
            _context.AccountBalance.Add(receiverBalance);
        }
        else
        {
            receiverBalance.Balance += transactionDto.Amount;
        }

        // Create transaction record
        var transaction = new Transactions
        {
            AccountNumber = transactionDto.AccountNumber,
            AccountNumberReceived = transactionDto.AccountNumberReceived,
            Amount = transactionDto.Amount,
            Type = transactionDto.Type,
            Date = DateTime.UtcNow
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Transaction successful." });
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
    [HttpGet("GetAccounts/{userId}")]
    public async Task<IActionResult> GetAccounts(int userId)
    {
        // Fetch accounts linked to the user, including balance
        var accounts = await _context.Accounts
            .Where(a => a.UserId == userId)
            .Select(a => new
            {
                a.AccountNumber,
                a.AccountType,
                Balance = _context.AccountBalance
                    .Where(ab => ab.AccountNumber == a.AccountNumber)
                    .Select(ab => ab.Balance)
                    .FirstOrDefault()
            })
            .ToListAsync();

        if (accounts == null || accounts.Count == 0)
        {
            return NotFound(new { message = "No accounts found for this user." });
        }

        return Ok(accounts);
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
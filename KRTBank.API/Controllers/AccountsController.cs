using KRTBank.Application.DTOs;
using KRTBank.Application.Services;
using KRTBank.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace KRTBank.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly AccountService _accountService;

    public AccountsController(AccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccountDTO>>> GetAll()
    {
        var accounts = await _accountService.GetAllAccountsAsync();
        var accountDtos = accounts.Select(a => new AccountDTO
        {
            Id = a.Id,
            HolderName = a.HolderName,
            CPF = a.CPF,
            Status = a.Status.ToString()
        });
        return Ok(accountDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AccountDTO>> GetById(Guid id)
    {
        try
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            var accountDto = new AccountDTO
            {
                Id = account.Id,
                HolderName = account.HolderName,
                CPF = account.CPF,
                Status = account.Status.ToString()
            };
            return Ok(accountDto);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult<AccountDTO>> Create([FromBody] CreateAccountDTO accountDto)
    {
        try
        {
            var account = await _accountService.CreateAccountAsync(accountDto.HolderName, accountDto.CPF);
            var resultDto = new AccountDTO
            {
                Id = account.Id,
                HolderName = account.HolderName,
                CPF = account.CPF,
                Status = account.Status.ToString()
            };
            return CreatedAtAction(nameof(GetById), new { id = resultDto.Id }, resultDto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] AccountDTO accountDto)
    {
        try
        {
            if (!Enum.TryParse<AccountStatus>(accountDto.Status, out var status))
            {
                return BadRequest("Invalid status value.");
            }
            await _accountService.UpdateAccountAsync(id, accountDto.HolderName, status);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            await _accountService.DeleteAccountAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
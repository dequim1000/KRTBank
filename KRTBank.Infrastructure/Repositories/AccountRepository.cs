using KRTBank.Domain.Entities;
using KRTBank.Domain.Interfaces;
using KRTBank.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace KRTBank.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);

    public AccountRepository(AppDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Account> GetByIdAsync(Guid id)
    {
        string cacheKey = $"Account_{id}";
        if (_cache.TryGetValue(cacheKey, out Account account))
        {
            return account;
        }

        account = await _context.Accounts.FindAsync(id);
        if (account != null)
        {
            _cache.Set(cacheKey, account, _cacheDuration);
        }

        return account;
    }

    public async Task<IEnumerable<Account>> GetAllAsync()
    {
        string cacheKey = "Accounts_All";
        if (_cache.TryGetValue(cacheKey, out IEnumerable<Account> accounts))
        {
            return accounts;
        }

        accounts = await _context.Accounts.ToListAsync();
        _cache.Set(cacheKey, accounts, _cacheDuration);
        return accounts;
    }

    public async Task AddAsync(Account account)
    {
        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();
        _cache.Remove("Accounts_All");
    }

    public async Task UpdateAsync(Account account)
    {
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();
        _cache.Remove($"Account_{account.Id}");
        _cache.Remove("Accounts_All");
    }

    public async Task DeleteAsync(Guid id)
    {
        var account = await _context.Accounts.FindAsync(id);
        if (account != null)
        {
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            _cache.Remove($"Account_{id}");
            _cache.Remove("Accounts_All");
        }
    }
}

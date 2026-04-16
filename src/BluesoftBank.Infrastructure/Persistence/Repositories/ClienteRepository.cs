using BluesoftBank.Application.Common.Interfaces;
using BluesoftBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BluesoftBank.Infrastructure.Persistence.Repositories;

public sealed class ClienteRepository(BankDbContext context) : IClienteRepository
{
    public async Task<Cliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Clientes
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task AddAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        await context.Clientes.AddAsync(cliente, cancellationToken);
    }
}

using BluesoftBank.Domain.Entities;

namespace BluesoftBank.Application.Common.Interfaces;

public interface ICuentaRepository
{
    Task<Cuenta?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Cuenta?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Cuenta cuenta, CancellationToken cancellationToken = default);
    void Update(Cuenta cuenta);
}

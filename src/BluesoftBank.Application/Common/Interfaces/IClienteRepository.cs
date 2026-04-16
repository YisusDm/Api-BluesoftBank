using BluesoftBank.Domain.Entities;

namespace BluesoftBank.Application.Common.Interfaces;

public interface IClienteRepository
{
    Task<Cliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PersonaNatural?> GetPersonaNaturalByCedulaAsync(
        string cedula, CancellationToken cancellationToken = default);

    Task<Empresa?> GetEmpresaByNitAsync(
        string nit, CancellationToken cancellationToken = default);

    Task AddAsync(Cliente cliente, CancellationToken cancellationToken = default);
}

namespace APBD15_Test1.Repositories;

public interface IServicesRepository
{
    public Task<bool> ServiceExistsByNameAsync(CancellationToken token, string name);
}
namespace APBD15_Test1.Repositories;

public interface IPatientsRepository
{
    public Task<bool> ExistsPatientAsync(CancellationToken token, int id);
}
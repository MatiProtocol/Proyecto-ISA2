namespace PharmaGo.IDataAccess;

public abstract class IDatabaseSizeService
{
    public abstract Task<long?> GetSpaceUsedAsync();
}
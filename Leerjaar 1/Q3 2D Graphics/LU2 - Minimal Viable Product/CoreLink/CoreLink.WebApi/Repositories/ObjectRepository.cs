using CoreLink.WebApi.Interfaces;

namespace CoreLink.WebApi.Repositories;

public class ObjectRepository : IObjectRepository
{
    private readonly string sqlConnectionString;

    public ObjectRepository(string sqlDatabaseConnectionString)
    {
        sqlConnectionString = sqlDatabaseConnectionString;
    }

    // TODO ADD METHODES

}
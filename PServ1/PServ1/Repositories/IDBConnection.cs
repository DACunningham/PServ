using Amazon.DynamoDBv2.DataModel;

namespace PServ1.Repositories
{
    public interface IDBConnection
    {
        IDynamoDBContext Context(DynamoDBContextConfig config = null);
    }
}

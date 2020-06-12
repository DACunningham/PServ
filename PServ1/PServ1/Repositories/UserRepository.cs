using Amazon.DynamoDBv2.DataModel;
using PServ1.Models;

namespace PServ1.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDBConnection _dbConnection;
        private readonly DynamoDBContextConfig _config;

        public UserRepository(IDBConnection dbConnection)
        {
            _dbConnection = dbConnection;

            // make context not delete attributes that are null, thus save operation will only
            // update values that have been set by user.
            _config = new DynamoDBContextConfig
            {
                IgnoreNullValues = true
            };
        }

        public int GetUserCount(string userCountId)
        {
            using (var context = _dbConnection.Context())
            {
                AppTotalUsers userCount = context.LoadAsync<AppTotalUsers>(userCountId).Result;
                return userCount.UserRecordCount;
            }
        }
    }
}
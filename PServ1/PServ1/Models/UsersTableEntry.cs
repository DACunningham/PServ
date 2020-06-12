using Amazon.DynamoDBv2.DataModel;

namespace PServ1.Models
{
    [DynamoDBTable("Users")]
    public class UsersTableEntry
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
    }
}

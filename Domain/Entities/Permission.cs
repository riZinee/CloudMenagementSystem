using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Permission
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid StorageId { get; set; }
        public List<PermissionValue> Values { get; set; }

        public User User { get; set; }
        public StorageMetadata Storage { get; set; }

        public Permission() { }

        public Permission(Guid userId, Guid storageId, List<PermissionValue> values)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            StorageId = storageId;
            Values = values;
        }
    }
}

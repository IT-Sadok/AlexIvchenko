using System.Text.Json.Serialization;

namespace LibraryManagement.Domain.Common;

public abstract class BaseEntity
{
    [JsonInclude]
    public Guid Id { get; protected set; } = Guid.NewGuid();
    [JsonInclude]
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    [JsonInclude]
    public DateTime? UpdatedAt { get; protected set; }
    [JsonInclude]
    public bool IsDeleted { get; protected set; }
    [JsonInclude]
    public DateTime? DeletedAt { get; protected set; }

    protected void SetUpdatedAt()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    protected void SetDeletedAt()
    {
        if (IsDeleted)
        {
            return;
        }

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        SetUpdatedAt();
    }
}

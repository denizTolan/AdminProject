namespace AdminProject.Data.Domain.Users;

public interface IEntity<TKey>
{
    TKey Id { get; set; }
}
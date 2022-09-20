namespace EscapeSandBox.Api.Domain
{
    public interface IEntity<T>
    {
        public T Id { get; set; }
    }
}

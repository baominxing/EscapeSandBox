using EscapeSandBox.Api.Domain;

namespace EscapeSandBox.Api.Repository
{
    public interface IDapperRepository<T, I> : ITransient where T : class, IEntity<I>
    {
        void Insert(T entity);

        Task InsertAsync(T entity);

        void Delete(T entity);

        Task DeleteAsync(T entity);

        void Update(T entity);

        Task UpdateAsync(T entity);

        T Get(string executeSql, object param);

        T FirstOrDefault(Func<T, bool> func);

        IEnumerable<T> GetAll();
    }
}

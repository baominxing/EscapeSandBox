using EscapeSandBox.Api.Domain;
using System.Data;
using Microsoft.Data.Sqlite;
using EscapeSandBox.Api.Core;
using Dapper;

namespace EscapeSandBox.Api.Repository
{
    public class DapperRepository<T, I> : IDapperRepository<T, I> where T : class, IEntity<I>
    {
        public void Insert(T entity)
        {
            using (var conn = this.GetDbConnection())
            {
                conn.Insert<T>(entity);
            }
        }

        public async Task InsertAsync(T entity)
        {
            using (var conn = this.GetDbConnection())
            {
                await conn.InsertAsync<T>(entity);
            }
        }

        public void Update(T entity)
        {
            using (var conn = this.GetDbConnection())
            {
                conn.Update<T>(entity);
            }
        }

        public async Task UpdateAsync(T entity)
        {
            using (var conn = this.GetDbConnection())
            {
                await conn.UpdateAsync<T>(entity);
            }
        }
        public void Delete(T entity)
        {
            using (var conn = this.GetDbConnection())
            {
                conn.Delete<T>(entity);
            }
        }

        public async Task DeleteAsync(T entity)
        {
            using (var conn = this.GetDbConnection())
            {
                await conn.DeleteAsync<T>(entity);
            }
        }

        public IEnumerable<T> GetAll()
        {
            IEnumerable<T> rlist;

            using (var conn = this.GetDbConnection())
            {
                rlist = conn.Query<T>($"select * from {typeof(T).Name}");
            }

            return rlist;
        }

        public T Get(string executeSql, object param)
        {
            T r;

            using (var conn = this.GetDbConnection())
            {
                r = conn.QueryFirst<T>(executeSql, param);
            }

            return r;
        }
        public T FirstOrDefault(Func<T, bool> func)
        {
            T r = GetAll().FirstOrDefault(func);

            return r;
        }

        private SqliteConnection GetDbConnection()
        {
            return new SqliteConnection(ApiConfig.DatabaseConnectionString);
        }
    }
}

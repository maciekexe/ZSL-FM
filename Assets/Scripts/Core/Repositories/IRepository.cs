using System.Collections.Generic;

namespace ZSLFM.Core.Repositories
{
    /// <summary>Bazowy interfejs CRUD. Implementację (SQLite) pisze Hubert w warstwie danych.</summary>
    public interface IRepository<T> where T : class
    {
        /// <summary>Zwraca encję lub null, jeśli nie istnieje.</summary>
        T GetById(int id);
        IReadOnlyList<T> GetAll();
        /// <summary>Dodaje encję i zwraca nadane Id.</summary>
        int Add(T entity);
        void Update(T entity);
        void Delete(int id);
    }
}

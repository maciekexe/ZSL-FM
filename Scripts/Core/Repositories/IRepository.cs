using System.Collections.Generic;

namespace LacznoscFM.Core.Repositories
{
    /// <summary>
    /// Podstawowy kontrakt repozytorium.
    /// Potrzebne polaczenie z baza danych - Hubert.
    /// Core zna interfejs, ale nie musi znać szczegółów bazy danych.
    /// </summary>
    public interface IRepository<T>
    {
        T? GetById(int id);
        IReadOnlyList<T> GetAll();
    }
}

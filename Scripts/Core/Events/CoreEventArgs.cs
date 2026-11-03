using System;

namespace LacznoscFM.Core.Events
{
    /// <summary>
    /// Bazowe argumenty zdarzeń Core.
    /// Konkretne zdarzenia mogą dziedziczyć po tej klasie.
    /// </summary>
    public class CoreEventArgs : EventArgs
    {
        // W razie potrzeby dodać wspólne dane zdarzeń,
        // np. identyfikator operacji lub czas wystąpienia.
    }
}

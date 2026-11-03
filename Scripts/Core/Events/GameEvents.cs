using System;

namespace LacznoscFM.Core.Events
{
    /// <summary>
    /// Centralne miejsce na zdarzenia udostępniane przez Core.
    /// To punkt startowy – docelowo zdarzenia mogą być rozdzielone
    /// na osobne serwisy, jeśli projekt będzie tego wymagał.
    /// </summary>
    public static class GameEvents
    {
        /// <summary>Informacja, że stan składu uległ zmianie.</summary>
        public static event EventHandler<SquadChangedEventArgs>? SquadChanged;

        /// <summary>Informacja o zdarzeniu w trakcie meczu.</summary>
        public static event EventHandler<MatchEventArgs>? MatchEventOccurred;

        public static void RaiseSquadChanged(object? sender, SquadChangedEventArgs args)
        {
            SquadChanged?.Invoke(sender, args);
        }

        public static void RaiseMatchEvent(object? sender, MatchEventArgs args)
        {
            MatchEventOccurred?.Invoke(sender, args);
        }
    }
}

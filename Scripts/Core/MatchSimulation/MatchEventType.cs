namespace LacznoscFM.Core.MatchSimulation
{
    /// <summary>
    /// Rodzaje zdarzeń, które mogą wystąpić podczas meczu.
    /// Lista jest wstępna, do rozszrzeszenia potem.
    /// </summary>
    public enum MatchEventType
    {
        Unknown,
        Goal,
        YellowCard,
        RedCard,
        Injury,
        Substitution
        // Dodanie potencjalnych innych wydarzen
    }
}

using System;
using LacznoscFM.Core.Models;

namespace LacznoscFM.Core.Events
{
    /// <summary>
    /// Argumenty zdarzenia informującego o zmianie składu.
    /// UI moze reagowac i zmieniac sie na podstawie tych wydarzen
    /// </summary>
    public class SquadChangedEventArgs : EventArgs
    {
        public Player? Player { get; }
        public string ChangeDescription { get; }

        public SquadChangedEventArgs(Player? player, string changeDescription)
        {
            Player = player;
            ChangeDescription = changeDescription;
        }
    }
}

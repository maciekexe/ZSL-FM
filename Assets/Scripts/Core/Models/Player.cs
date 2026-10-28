using System;

namespace ZSLFM.Core.Models
{
    /// <summary>Zawodnik. Czysta klasa POCO - bez UnityEngine i bez MonoBehaviour.</summary>
    public class Player
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Nationality { get; set; }
        public Position Position { get; set; }

        // Atrybuty (skala 0-100)
        public int Pace { get; set; }
        public int Shooting { get; set; }
        public int Passing { get; set; }
        public int Defending { get; set; }
        public int Physical { get; set; }
        public int Goalkeeping { get; set; }
        public int Potential { get; set; }

        // Stan bieżący (przydatne dla zarządzania składem)
        public int Form { get; set; }               // 0-100
        public int Fitness { get; set; }            // 0-100
        public int InjuryDaysRemaining { get; set; }

        // Ekonomia
        public decimal MarketValue { get; set; }

        /// <summary>Klub zawodnika; null = wolny agent.</summary>
        public int? ClubId { get; set; }

        public string FullName => FirstName + " " + LastName;
        public bool IsInjured => InjuryDaysRemaining > 0;
        public bool IsFreeAgent => ClubId == null;

        /// <summary>Wiek liczony względem daty w grze (nie DateTime.Now!).</summary>
        public int GetAge(DateTime currentDate)
        {
            int age = currentDate.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > currentDate.AddYears(-age).Date) age--;
            return age;
        }
    }
}
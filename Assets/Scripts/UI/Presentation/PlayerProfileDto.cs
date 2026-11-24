using System;

namespace ZSLFM.UI
{
    /// <summary>
    /// Model DTO prezentujący pełny profil zawodnika na potrzeby warstwy UI.
    /// W docelowej integracji (H-9 / M-8) będzie mapowany bezpośrednio z modeli Core i bazy SQLite.
    /// </summary>
    [Serializable]
    public class PlayerProfileDto
    {
        public string FirstName { get; }
        public string LastName { get; }
        public string Position { get; }
        public int OverallSkill { get; }
        public string ClubName { get; }
        public double Price { get; }
        public int Age { get; }
        public double Wage { get; }

        // 6 atrybutów stylu Football Managera (0-99)
        public int Pace { get; }
        public int Shooting { get; }
        public int Passing { get; }
        public int Dribbling { get; }
        public int Defending { get; }
        public int Physicality { get; }

        public PlayerProfileDto(
            string firstName,
            string lastName,
            string position,
            int overallSkill,
            string clubName,
            double price,
            int age,
            double wage,
            int pace,
            int shooting,
            int passing,
            int dribbling,
            int defending,
            int physicality)
        {
            FirstName = firstName;
            LastName = lastName;
            Position = position;
            OverallSkill = overallSkill;
            ClubName = clubName;
            Price = price;
            Age = age;
            Wage = wage;
            Pace = pace;
            Shooting = shooting;
            Passing = passing;
            Dribbling = dribbling;
            Defending = defending;
            Physicality = physicality;
        }

        public string FullName => $"{FirstName} {LastName}";
    }
}

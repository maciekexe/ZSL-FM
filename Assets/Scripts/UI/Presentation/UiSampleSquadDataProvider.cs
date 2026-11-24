using System.Collections.Generic;

namespace ZSLFM.UI
{
    /// <summary>
    /// Tymczasowe dane próbne wyłącznie dla warstwy UI (makieta kadry i rynku transferowego).
    /// W zadaniu H-9/M-8 zostaną zastąpione danymi z bazy SQLite oraz warstwy Core.
    /// </summary>
    public static class UiSampleSquadDataProvider
    {
        public const double DefaultClubBudget = 150000.0;

        public static List<PlayerProfileDto> CreateSampleSquadPlayers()
        {
            return new List<PlayerProfileDto>
            {
                new PlayerProfileDto("Maciej", "Kamiński", "MID", 90, "ZSŁ Gdańsk", 450000.0, 18, 2500.0, 88, 86, 94, 92, 82, 84),
                new PlayerProfileDto("Nataniel", "Żbikowski", "STR", 90, "ZSŁ Gdańsk", 500000.0, 18, 2500.0, 94, 92, 82, 88, 50, 86),
                new PlayerProfileDto("Hubert", "Miłuch", "DEF", 90, "ZSŁ Gdańsk", 420000.0, 18, 2500.0, 84, 62, 85, 80, 94, 92),
                new PlayerProfileDto("Karol", "Stolc", "DEF", 65, "ZSŁ Gdańsk", 45000.0, 18, 500.0, 66, 45, 62, 60, 68, 70),

                new PlayerProfileDto("Kacper", "Nowak", "GK", 75, "ZSŁ Gdańsk", 90000.0, 20, 1200.0, 60, 30, 65, 55, 76, 78),
                new PlayerProfileDto("Tomasz", "Zieliński", "GK", 63, "ZSŁ Gdańsk", 35000.0, 17, 400.0, 55, 25, 58, 50, 64, 65),
                new PlayerProfileDto("Michał", "Wiśniewski", "DEF", 72, "ZSŁ Gdańsk", 75000.0, 22, 1000.0, 72, 48, 68, 65, 74, 75),
                new PlayerProfileDto("Jakub", "Wójcik", "DEF", 69, "ZSŁ Gdańsk", 55000.0, 19, 700.0, 68, 42, 64, 62, 71, 72),
                new PlayerProfileDto("Mateusz", "Kowalczyk", "DEF", 70, "ZSŁ Gdańsk", 60000.0, 21, 850.0, 70, 44, 66, 63, 72, 73),
                new PlayerProfileDto("Oskar", "Kozłowski", "DEF", 64, "ZSŁ Gdańsk", 40000.0, 18, 450.0, 64, 40, 60, 58, 66, 67),
                new PlayerProfileDto("Piotr", "Lewandowski", "MID", 77, "ZSŁ Gdańsk", 110000.0, 23, 1400.0, 76, 72, 79, 78, 68, 72),
                new PlayerProfileDto("Szymon", "Szymański", "MID", 74, "ZSŁ Gdańsk", 85000.0, 20, 1100.0, 75, 70, 76, 75, 65, 70),
                new PlayerProfileDto("Dawid", "Woźniak", "MID", 68, "ZSŁ Gdańsk", 50000.0, 19, 650.0, 68, 62, 70, 69, 60, 65),
                new PlayerProfileDto("Kamil", "Dąbrowski", "MID", 64, "ZSŁ Gdańsk", 40000.0, 18, 450.0, 65, 58, 66, 65, 58, 62),
                new PlayerProfileDto("Filip", "Mazur", "MID", 71, "ZSŁ Gdańsk", 70000.0, 21, 900.0, 72, 66, 73, 72, 62, 68),
                new PlayerProfileDto("Jan", "Krawczyk", "STR", 76, "ZSŁ Gdańsk", 105000.0, 22, 1300.0, 78, 77, 68, 74, 42, 75),
                new PlayerProfileDto("Mikołaj", "Kaczmarek", "STR", 69, "ZSŁ Gdańsk", 55000.0, 19, 700.0, 74, 71, 62, 68, 38, 70),
                new PlayerProfileDto("Igor", "Nowicki", "STR", 66, "ZSŁ Gdańsk", 45000.0, 18, 500.0, 72, 68, 60, 65, 36, 67)
            };
        }

        public static List<PlayerProfileDto> CreateSampleMarketPlayers()
        {
            return new List<PlayerProfileDto>
            {
                new PlayerProfileDto("Camilo", "Mena", "MID", 75, "Lechia Gdańsk", 120000.0, 22, 1800.0, 84, 72, 74, 80, 52, 70),
                new PlayerProfileDto("Maksym", "Khlan", "STR", 74, "Lechia Gdańsk", 95000.0, 21, 1500.0, 82, 75, 70, 78, 45, 68),
                new PlayerProfileDto("Rifet", "Kapić", "MID", 73, "Lechia Gdańsk", 85000.0, 28, 1400.0, 70, 71, 78, 74, 64, 72),
                new PlayerProfileDto("Bogdan", "Sarnavskyi", "GK", 72, "Lechia Gdańsk", 75000.0, 29, 1300.0, 58, 28, 65, 52, 74, 75),
                new PlayerProfileDto("Elias", "Olsson", "DEF", 71, "Lechia Gdańsk", 70000.0, 21, 1200.0, 70, 42, 66, 62, 73, 76),

                new PlayerProfileDto("Karol", "Czubak", "STR", 74, "Arka Gdynia", 110000.0, 24, 1700.0, 78, 78, 64, 70, 40, 82),
                new PlayerProfileDto("Hubert", "Adamczyk", "MID", 72, "Arka Gdynia", 80000.0, 26, 1350.0, 74, 73, 76, 75, 55, 68),
                new PlayerProfileDto("Kacper", "Skóra", "STR", 70, "Arka Gdynia", 70000.0, 21, 1100.0, 80, 71, 66, 73, 42, 66),
                new PlayerProfileDto("Martin", "Dobrotka", "DEF", 71, "Arka Gdynia", 65000.0, 39, 1000.0, 60, 45, 68, 60, 75, 74),
                new PlayerProfileDto("Paweł", "Lenarcik", "GK", 70, "Arka Gdynia", 60000.0, 29, 950.0, 56, 25, 62, 50, 72, 72),

                new PlayerProfileDto("Lucas", "Beraldo", "DEF", 84, "PSG", 950000.0, 20, 25000.0, 78, 55, 79, 76, 86, 82),
                new PlayerProfileDto("Bradley", "Barcola", "STR", 86, "PSG", 1900000.0, 21, 45000.0, 92, 82, 80, 89, 44, 74),
                new PlayerProfileDto("Vitinha", "Silva", "MID", 89, "PSG", 2800000.0, 24, 60000.0, 80, 80, 91, 90, 76, 78),
                new PlayerProfileDto("Gianluigi", "Donnarumma", "GK", 88, "PSG", 3200000.0, 25, 75000.0, 65, 30, 75, 60, 90, 88),

                new PlayerProfileDto("Antonio", "Rüdiger", "DEF", 87, "Real Madryt", 2100000.0, 31, 55000.0, 86, 52, 72, 68, 88, 90),
                new PlayerProfileDto("Thibaut", "Courtois", "GK", 90, "Real Madryt", 3500000.0, 32, 85000.0, 68, 35, 78, 62, 92, 89),
                new PlayerProfileDto("Jude", "Bellingham", "MID", 96, "Real Madryt", 5500000.0, 21, 120000.0, 86, 90, 92, 94, 84, 88),
                new PlayerProfileDto("Vinicius", "Junior", "STR", 95, "Real Madryt", 6000000.0, 24, 110000.0, 98, 89, 85, 96, 42, 78),

                new PlayerProfileDto("Dayot", "Upamecano", "DEF", 85, "Bayern Monachium", 1400000.0, 25, 40000.0, 85, 48, 70, 72, 87, 88),
                new PlayerProfileDto("Joshua", "Kimmich", "MID", 88, "Bayern Monachium", 2600000.0, 29, 65000.0, 74, 76, 92, 86, 84, 82),
                new PlayerProfileDto("Harry", "Kane", "STR", 93, "Bayern Monachium", 4200000.0, 31, 100000.0, 75, 96, 88, 84, 48, 86)
            };
        }
    }
}

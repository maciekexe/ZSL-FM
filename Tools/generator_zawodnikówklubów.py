import sqlite3
import random
import os

db_path = '../Data/Database/zsl_fm.db'

def create_massive_database():
    os.makedirs(os.path.dirname(db_path), exist_ok=True)
    conn = sqlite3.connect(db_path)
    cursor = conn.cursor()

    cursor.execute('''CREATE TABLE IF NOT EXISTS Clubs (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL, Budget REAL, Reputation INTEGER)''')
    cursor.execute('''CREATE TABLE IF NOT EXISTS Players (Id INTEGER PRIMARY KEY AUTOINCREMENT, FirstName TEXT, LastName TEXT, OverallSkill INTEGER, Position TEXT, ClubId INTEGER, FOREIGN KEY(ClubId) REFERENCES Clubs(Id))''')
    
    cursor.execute('DELETE FROM Players')
    cursor.execute('DELETE FROM Clubs')

    # 1. Generowanie 10 klubów
    clubs_data = [
        ('ZSŁ Gdańsk', 150000.0, 50), ('PSG', 5000000.0, 95), ('Lechia Gdańsk', 300000.0, 60),
        ('Arka Gdynia', 250000.0, 55), ('Real Madryt', 8000000.0, 98), ('FC Barcelona', 7500000.0, 96),
        ('Bayern Monachium', 6000000.0, 94), ('Manchester City', 9000000.0, 97), ('Juventus', 4000000.0, 88),
        ('AC Milan', 3500000.0, 85)
    ]
    cursor.executemany('INSERT INTO Clubs (Name, Budget, Reputation) VALUES (?, ?, ?)', clubs_data)
    cursor.execute('SELECT Id FROM Clubs')
    club_ids = [row[0] for row in cursor.fetchall()]

    # 2. Masowe generowanie setek zawodników
    first_names = ["Jan", "Piotr", "Michał", "Kacper", "Jakub", "Mateusz", "Tomasz", "Szymon", "Filip", "Maciej", "Dawid", "Kamil", "Oskar", "Igor", "Mikołaj"]
    last_names = ["Kowalski", "Nowak", "Wiśniewski", "Wójcik", "Kowalczyk", "Kamiński", "Lewandowski", "Zieliński", "Szymański", "Woźniak", "Dąbrowski", "Kozłowski", "Mazur", "Krawczyk", "Kaczmarek"]
    positions = ["GK", "DEF", "MID", "STR"]

    players_data = []
    
    # Dodajemy Waszych kluczowych graczy na sztywno do ZSŁ Gdańsk (Id 1)
    players_data.extend([
        ('Karol', 'Stolc', 85, 'DEF', 1),
        ('Nataniel', 'Żbikowski', 96, 'STR', 1),
        ('Maciej', 'Kamiński', 65, 'MID', 1),
        ('Hubert', 'Miłuch', 88, 'DEF', 1)
    ])

    # Generujemy resztę (po 22 zawodników dla każdego klubu)
    for club_id in club_ids:
        for _ in range(22):
            # Żeby nie dublować Waszych w ZSŁ
            if club_id == 1 and len([p for p in players_data if p[4] == 1]) >= 22:
                break 
            
            fname = random.choice(first_names)
            lname = random.choice(last_names)
            skill = random.randint(50, 90)
            if club_id in [2, 5, 6, 7, 8]: # Giganci mają lepszych graczy
                skill = random.randint(75, 95)
            pos = random.choice(positions)
            players_data.append((fname, lname, skill, pos, club_id))

    cursor.executemany('INSERT INTO Players (FirstName, LastName, OverallSkill, Position, ClubId) VALUES (?, ?, ?, ?, ?)', players_data)
    
    conn.commit()
    conn.close()
    print(f"Wygenerowano masową bazę danych ({len(players_data)} zawodników) w: {db_path}")

if __name__ == '__main__':
    create_massive_database()

import sqlite3
import os

db_path = '../Data/Database/zsl_fm.db'

def create_database():
    os.makedirs(os.path.dirname(db_path), exist_ok=True)
    
    conn = sqlite3.connect(db_path)
    cursor = conn.cursor()

    cursor.execute('''
        CREATE TABLE IF NOT EXISTS Clubs (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            Budget REAL,
            Reputation INTEGER
        )
    ''')

    cursor.execute('''
        CREATE TABLE IF NOT EXISTS Players (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            FirstName TEXT,
            LastName TEXT,
            OverallSkill INTEGER,
            Position TEXT,
            ClubId INTEGER,
            FOREIGN KEY(ClubId) REFERENCES Clubs(Id)
        )
    ''')

    cursor.execute('DELETE FROM Players')
    cursor.execute('DELETE FROM Clubs')

    clubs_data = [
        ('ZSŁ Gdańsk', 150000.0, 50),
        ('PSG', 500000.0, 70),
        ('Lechia Gdańsk', 300000.0, 60)
    ]
    cursor.executemany('INSERT INTO Clubs (Name, Budget, Reputation) VALUES (?, ?, ?)', clubs_data)

    cursor.execute('SELECT Id, Name FROM Clubs')
    clubs = {name: club_id for club_id, name in cursor.fetchall()}

    players_data = [
        ('Karol', 'Stolc', 85, 'DEF', clubs['ZSŁ Gdańsk']),
        ('Nataniel', 'Żbikowski', 96, 'STR', clubs['ZSŁ Gdańsk']),
        ('Maciej', 'Kamiński', 99, 'MID', clubs['ZSŁ Gdańsk']),
        ('Hubert', 'Miłuch', 99, 'DEF', clubs['ZSŁ Gdańsk']),
        ('Jan', 'Zieliński', 20, 'STR', clubs['PSG'])
    ]
    cursor.executemany('INSERT INTO Players (FirstName, LastName, OverallSkill, Position, ClubId) VALUES (?, ?, ?, ?, ?)', players_data)

    conn.commit()
    conn.close()
    
    print(f"Baza danych pomyślnie wygenerowana w: {db_path}")

if __name__ == '__main__':
    create_database()
import re
import sqlite3

conn = sqlite3.connect('emaildb.sqlite')
cur = conn.cursor()

cur.execute('DROP TABLE IF EXISTS Counts')

cur.execute('CREATE TABLE Counts (org TEXT, count INTEGER)')

fname = input('Enter file name: ')
fh = open(fname)

x = 0
for line in fh:
    emails = re.findall('^From: [a-zA-Z0-9]+@([a-zA-Z0-9\.]+)', line)
    if (len(emails) != 0):
        x += 1
        cur.execute('SELECT count FROM Counts Where org = ?', (emails[0],))
        row = cur.fetchone()
        if row is None:
            cur.execute('INSERT INTO Counts (org, count) Values (?, 1)', (emails[0],))
        else:
            cur.execute('UPDATE Counts SET count = count + 1 Where org = ?', (emails[0],))
        if (x == 50):
            conn.commit()
            x = 0

if (x != 0):
    conn.commit()

sqlstr = 'SELECT org, count FROM Counts Order BY count DESC LIMIT 10'

for row in cur.execute(sqlstr):
    print(str(row[0]), row[1])

cur.close()
# SQL

# Contents

+ [MySQL cli](#mysql-cli)
+ [SQL commands](#sql-commands)
	+ [CREATE](#create)
	+ [INSERT](#insert)
	+ [UPDATE](#update)
	+ [DELETE](#delete)
	+ [SELECT](#select)
+ [Data types](#data-types)
+ [Keys and indexes](#keys-and-indexes)
+ [Resources](#resources)

# Intro

Basic commands:
+ Create/Insert data
+ Read/Select data
+ Update data
+ Delete data

Terms:
+ Database
+ Table (or Relation)
+ Row (or Tuple)
+ Column or Row (or Attributes)
+ Schema (metadata, first row of a table - will contains a lot of different
constraints as well)

# MySQL cli

log in with credentials:
```bash
$ mysql -u root -p
```

show databases (MySQL contains 3 internal databases `mysql`,
`information_schema` and `performance_schema` that should not be deleted):
```sql
mysql> show databases;
```

To switch to particular database:
```sql
mysql> use [database_name];
```

Get info about particular table:
```sql
mysql> descrbibe [table_name];
```

# SQL commands

## CREATE

+ **CREATE DATABASE**
```sql
CREATE DATABASE people DEFAULT CHARACTER SET utf8;
```
creates a new database named people and sets wider available letter set via
next command with argument utf8
+ **CREATE TABLE**
```sql
CREATE TABLE Users (
	name VARCHAR(128),
	email VARCHAR(128)
);
```
creates table with two fields and set the maximum length; another example when
establishing relation to a field in another table:
```sql
CREATE TABLE Album (
	album_id INTEGER NOT NULL AUTO_INCREMENT,
	title VARCHAR(255),
	artist_id INTEGER,
	PRIMARY KEY(album_id),

	CONSTRANT FOREIGN KEY (artist_id)
		REFERENCES Artist (artist_id)
		ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB;
```

**ON DELETE** choices:
+ **Default / RESTRICT** - don't allow changes that break the constraint
+ **CASCADE** - adjust child rows by deleting or updating them
+ **SET NULL** - set the foreign key in the child rows to null

Connector table example for many-to-many relationship(combination of two
foreigh keys will be unique for each entry, thus, can act as primary key):
```sql
CREATE TABLE Member (
	account_id INTEGER,
	course_id INTEGER,
	role INTEGER,

	CONSTRANT FOREIGN KEY (account_id)
		REFERENCES Account (accound_id)
		ON DELETE CASCADE ON UPDATE CASCADE,
	CONSTRANT FOREIGN KEY (course_id)
		REFERENCES Account (course_id)
		ON DELETE CASCADE ON UPDATE CASCADE,
	PRIMARY KEY (account_id, course_id)
) ENGINE = InnoDB CHARACTER SET=utf-8;
```

## INSERT

+ **INSERT INTO** ... **VALUES** ...
```sql
INSERT INTO Users (name, email) VALUES ('Kamil', hello@gmail.com);
```
inserts a row to the table

## UPDATE

+ **UPDATE** ... **SET** ... **WHERE** ...
```sql
UPDATE Users SET name='Kamil' WHERE email='hello@gmail.com'
```
updates field in rows that satisfy the condition set by **WHERE** clause

## DELETE

+ **DELETE FROM** ... **WHERE** ...
```sql
FROM Users WHERE email='hello@gmail.com'
```
deteles a row that meets the criteria

## SELECT

+ **SELECT** * **FROM**
```sql
SELECT * FROM Users WHERE email='hello@gmail.com'
```
reads and outputs data from specified table, which also satisfies **WHERE** clause, `*` means all rows

+ **SELECT** [] **FROM** [] **JOIN** [] **ON**
```sql
SELECT Album.title,Artist.name FROM Album JOIN Artist ON Album.artist_id=Artist.artist_id
```
**JOIN** puts together two tables, creating long rows with all possible
combintaions, while **ON** filters out only those, where primary key and
foreign key match.

**SELECT** command has also other features like **ORDER BY** to output result
in different form:
```sql
mysql> SELECT * FROM Users ORDER BY email;
```

Another example with wildcard matching (`%` means some letters):
```sql
mysql> SELECT * FROM Users WHERE name LIKE '%e%';
```

Limit the output of **SELECT**(can take a form of just number of entries or
starting row and number of entries; row numbering starts from 0):
```sql
mysql> SELECT * FROM Users ORDER BY email DESC LIMIT 2;
mysql> SELECT * FROM Users ORDER BY email LIMIT 1,2;
```

Get number of rows instead of actual rows using COUNT function:
```sql
mysql> SELECT COUNT(*) FROM Users ORDER BY email;
```

---

Taking **DELETE FROM** command as the example, the first part of which works as
a loop that would delete everything if not restricted by **WHERE** statement,
which works like an `if` statement.

# Data types

## String fields

+ **CHAR** allocates entire space
+ **VARCHAR** allocates a variable amount of space

## Text fields

Good for blog text, html or just some paragraphs. Aren't good use for indexing
and sorting.

+ **TINYTEXT** up to 255 characters
+ **TEXT** up to 65K
+ **MEDIUMTEXT** up to 16M
+ **LONGTEXT** up to 4G

## Integer

+ **TINYINT** (-128, 128)
+ **SMALLINT** (-32786, 32768)
+ **INT** or **INTEGER** (2 Billion)
+ **BIGINT** around 10\*\*18

## Floating point numbers

+ **FLOAT** 32-bit (7 digit accuracy)
+ **DOUBLE** 64-bit (14 digit accuracy)


## Binary types

Good for smal images (data). Aren't indexed or sorted.

+ **BYTE**
+ **VARBYTE**

## BLOB

Binary Large Object

Good for raw data, files, PDFs, movies, etc.

+ **TINYBLOB(n)** up to 255
+ **BLOB(n)** up to 65K
+ **MEDIUMBLOB(n)** up to 16M
+ **LONGBLOB(n)** up to 4G

## Dates

+ **TIMESTAMP** - 'YYYY-MM-DD HH:MM:SS' (1970, 2037) - is limited in length as
number of seconds on January smth in 1970 and will be no longer good after 2037
+ **DATETIME** - 'YYYY-MM-DD HH:MM:SS'
+ **DATE** - YYYY-MM-DD
+ **TIME** - HH:MM:SS
+ build-in MySQL function **NOW()**

# Keys and indexes

To join multiple tables we need an integer primary key for each row to
efficiently add a reference to a row, thus, table creation looks like this:
```sql
CREATE TABLE Users (
	user_id INT UNSIGNED NOT NULL AUTO_INCREMENT,
	name VARCHAR(128),
	email VARCHAR(128),
	PRIMARY KEY(user_id),
	INDEX(emal)
)
```

**PRIMARY KEY** statement tell db that this field is going to be used a lot,
while **INDEX** tells that **WHERE** clause going to use that one a lot, so
that db can adjust they way it stores this fields.

**PRIMARY KEY** - very little space, exact match

**INDEX** - good for individual row lookup, sorting/grouping result, works best
with exact matches or prefix lookups, generally HASH or BTREE

If forgot to specify on table creation, can still add it later (you can specify
what to use, but rather let database decide):
```sql
ALTER TABLE Users ADD INDEX (email) USING BTREE
```

3 types of keys:
+ primary - generally integer auto-increment field
+ logical - outside world uses for lookup
+ foreign - generally integer key pointing at a row in another table

# Conventions

+ table name start with upper letter - Users
+ primary key be table name lower case letters plus underscore id - users\_id
+ field name are lower case

# Resources

+ https://textbooks.opensuny.org/the-missing-link-an-introduction-to-web-development-and-programming/
+ https://en.wikipedia.org/wiki/B-tree
+ [docker image of web admin for different types of
databases](https://hub.docker.com/_/adminer)

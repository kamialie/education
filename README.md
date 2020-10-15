# SQL

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

| Command			| Example											| Description	|
|:-----------------:|---------------------------------------------------|---------------|
| CREATE DATABASE	| CREATE DATABASE people DEFAULT CHARACTER SET utf8;|
creates a new database named people and sets wider available letter set via
next command with argument utf8	|
| CREATE TABLE		| CREATE TABLE Users ( name VARCHAR(128), email
VARCHAR(128));	| creates table with two fields and set the maximum length	|
| INSERT INTO ... VALUES	| INSERT INTO Users (name, email) VALUES ('Kamil',
hello@gmail.com);	|

# Resources

+ https://textbooks.opensuny.org/the-missing-link-an-introduction-to-web-development-and-programming/

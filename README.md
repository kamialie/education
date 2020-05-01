## Topic links

- Installation:
    + https://stackoverflow.com/questions/20994716/what-is-the-difference-between-pip-and-conda - pip vs conda
    + https://docs.pipenv.org/en/latest/install/ - pipenv package manager (what is it?)
- Modules:
    + https://pypi.org/project/requests/ or https://2.python-requests.org/en/master/user/install/#install - requests module (for http)

- Usefull stuff
    + https://virtualenv.pypa.io/en/latest/ - virtual environment
    + https://stackoverflow.com/questions/27835619/urllib-and-ssl-certificate-verify-failed-error - interesting answer about certification handling
    + https://realpython.com/vim-and-python-a-match-made-in-heaven/ - setting evironment with vim

[Python built-in functions](https://docs.python.org/3/library/functions.html)

## Contents
+ [Automation](#automation)
	+ [Environment](#environment)
	+ [CSV](#csv)
	+ [Examples](#examples)
+ [Regex](#regex)
	+ [Methods](#methods)
	+ [Specifiers](#specifiers)
	+ [Groups](#groups)
+ [Subprocesses](#subprocesses)
+ [Testing](#testing)
+ [Troubleshooting](#troubleshooting)

## Automation

Pareto principle

get disk usage:
```python
import shutil
du = shutil.disk_usage('/')
print(du.free / du.total * 100)
```

get CPU usage(takes seconds, float, as parameter, to calculate average during that period of time):
```python
import psutil
psutil.cpu_percent(1)
```

To work with files and directories [os module](https://docs.python.org/3/library/os.html) is used:

```python
import os
```

| Action						| Command										|
|-------------------------------|-----------------------------------------------|
| remove file					| `os.remove('file.txt')`						|
| rename file					| `os.rename('old_name.txt', 'new_name.txt')`	|
| check if file exists			| `os.path.exists('file.txt')`					|
| get size of file (in bytes)	| `os.path.getsize('file.txt')`					|
| get last modification time(returns unix timestamp; use `datetime` module to convert it to human readable)	| `os.path.getmtime('file.txt')`	|
| construct absolute path		| `os.path.abspath('file.txt')`					|
| get current working directory | `os.path.getcwd()`							|
| create directory				| `os.mkdir('directory')`						|
| change directory				| `os.chdir('directory')`						|
| remove directory (successful only if directory is empty	| `os.rmdir('directory')`	|
| list contents of directory, returns a list	| `os.listdir('directory')`		|
| check file is a directory		| `os.isdir('name')`							|

[documentation on os.path](https://docs.python.org/3/library/os.path.html)

---

### Environment

`environ` dictionary of `os` module provides environment variables of shell (second argument is the default value if key is not present):

```python
import os
var = os.environ.get('HOME', '')
```

To get command-line arguments use `sys` module; the resulting list contains program name, as well as passed arguments:
```python
import sys
print(sys.argv)
```

`?` environment variable holds a status code of the last shell command executed

to get a copy of current shell environment use `copy()` method, which returns a dictionary of variables;
it can further be modified to be passed to a child process (use `pathsep.join()` method, which takes list of variables to construct new PATH):
```python
import os
my_env = os.environ.copy()
my_env = os.pathsep(['/usr/local/bin/', my_env['PATH']])
```

---

### CSV

[docs](https://docs.python.org/3/library/csv.html)
[realpython guide](https://realpython.com/python-csv/)

To work with csv files, import corresponding module:

```python
import cv
```

First, get the instance of the csv reader class:

```python
fh = open('file.txt')
csv_file = csv.reader(fh)
```

Iterating over this instance returns each row in list format, where each index represents the column:

```python
for row in csv_file:
	field1, field2 = row
	print(field1, field2)
```

To write data as csv file create a writer object on fh that is given a write permissions.
`writerow` will write single row into file, while `writerows` will fill in entire data (list of lists):

```python
hosts = [['host1', '196.168.0.0'], ['host2', '196.168.0.1']]
with open('file'.csv', 'w') as fh:
	writer = csv.writer(fh)
	writer.writerows(hosts)
```

If first row has the name of columns, `DictReader` method turns each row into dictionary:

```python
reader = csv.DictReader(fh)
for row in csv_file:
	print(row['column1'], row['column2'])
```

To create a csv from a dictionary you need to have a separate list with column names:

```python
data = [{...}, {...}]
keys = ['column1', 'column2']
with open('file.csv', 'w') as fh:
	writer = csv.DictWriter(fh, fieldnames=keys)
	writer.writeheader()
	writer.writerows(data)
```

### Examples

List contents of the directory and determine the type of its contents:

```python
for name in os.listdir('directory'):
	fullname = os.path.join('directory', name)
	if os.path.isdir(fullname):
		print(f'{fullname} is a directory)
	else:
		print(f'{fullname} is a file')
```

Convert the output of `os.path.getmtime()` to human readable format:

```python
import os
import datetime

timestamp = os.path.getmtime('newfile.txt')
date = datetime.datetime.fromtimestamp(timestamp)
```

---

# Regex

[Regex HOWTO](https://docs.python.org/3/howto/regex.html)

[Play regex](https://regexcrossword.com/)

To work with regular expressions import [re](https://docs.python.org/3/library/re.html) module.

To specify the search pattern pass raw string to re module methods using `r` prefix - `r"pattern"` (this tells Python to send the string as it is).

`re.IGNORECASE` option.

## Methods

| Method							| Description	|
|-----------------------------------|---------------|
| search(pattern, target\_string	| returns re.Match object (with only first match) with info of the index where the match was found and the corresponding string; if nothing was found, returns `None`	|
| findall(pattern, target\_string	| same as previous, but returns all matches	in the form all list	|
| split(pattern, target\_string		| works as string *split* method, while taking regex as first parameter indicating possible delimiters; enclose the regex in parenthesis to include the delimiters in resulting list	|
| sub(pattern, sub\_string, target\_string	| replaces all occurrences of pattern with a second argument, *sub\_string*; *sub\_string* may also be a regex, f.e. using the capturing groups found earlier	|

## Specifiers

Escape special characters with `\` backslash.

[check, analyze regex](https://regex101.com/)


[greedy vs non-greedy mode](https://docs.python.org/3/howto/regex.html#greedy-versus-non-greedy)

| Specifier									| Description	|
|-------------------------------------------|---------------|
| . - dot									| matches any character	|
| [Pp], [a-z], [a-z0-9] - character classes	| matches any characters specified inside brackets, can specify range or multiple ranges; adding `^` in the beginning negates the result	|
| expression|expression - pipe				| matches either of the expression	|
| * - star									| repeated match - preceding character 0 or more times	|
| + - plus									| repeated match - preceding character 1 or more times	|
| ? - question mark							| repeated match - preceding character 0 or one times (that is optional)	|
| {number}, {num1, num2}					| repeated match - preceding character *number* of times, or within range	|
| (expression)								| capture groups - enclosed expression will be captures (returned by methods); can be later references by backslash followed by number notation - `\1`	|
| \b										| specify boundaries, f.e. `r'\b[a-z]{5}\b'` - for exactly 5 letter word	|
| \w										| matches any character, number or underscore	|
| \d										| matches digits	|
| \s										| matches whitespace characters	|

## Groups

Patterns enclosed in parenthesis will be captured as groups. `groups` on result of `search` method will returned these groups.

```python
result = re.search(r'pattern', string)
groups = result.groups()
```

# Subprocesses

[docs](https://docs.python.org/3/library/subprocess.html)

`subprocess` module provide functionality to run shell commands within python.

`run` method takes in a list that consists of command and its arguments.
It store the return code that can be accessed later.
To capture the output of the command pass `capture_ouput=True` argument to the run method.
It will be accessible through `stdout` property of returned variable and be represented by array of bytes,
which needs to be decoded.

`run` method also might take arguments:
+ `env` - dictionary with environment variables
+ `cwd` - working directory for the command
+ `timeout` - in seconds
+ `shell` - boolean, if true, python will first execute default system shell, then run command inside of it (enables shell features like shell expansions)

## Examples

run date command:
```python
import subprocess
result = subprocess.run(['date'])
result.returncode
```

capture output:
```python
import subprocess
result = subprocess.run(['host'], capture_output=True)
print(result.sdout.decode())
```

execute child process with modified environment:
```python
import os
import subprocess
result = subprocess.run(['myapp'], env=my_env)
my_env = os.environ.copy()
my_env = os.pathsep(['/usr/local/bin/', my_env['PATH']])
```

# Testing

[docs, basic example](https://docs.python.org/3/library/unittest.html#basic-example)

[docs, running in cl](https://docs.python.org/3/library/unittest.html#command-line-interface)

[docs, patterns](https://docs.python.org/3/library/unittest.html#organizing-test-code)

[docs, assertRaises](https://docs.python.org/3/library/unittest.html#unittest.TestCase.assertRaises)

[monitoring distributed systems](https://landing.google.com/sre/sre-book/chapters/monitoring-distributed-systems/)

[testing reliability](https://landing.google.com/sre/sre-book/chapters/testing-reliability/)

[performance testing](https://testing.googleblog.com/2007/10/performance-testing.html)

[raising exceptions](https://docs.python.org/3/tutorial/errors.html#raising-exceptions)

Python provides `unittest` module with classes and methods to provide necessary functionality.
For example, `TestCase` class. To use it create you own class and inherit from `TestCase`.
Any method that is prefixed with `test_` will be recognized with testing framework.

[assert](https://docs.python.org/2/reference/simple_stmts.html#the-assert-statement) keyword raises an error with a message, when condition evaluates to false:
```python
assert type(parameter) == str, 'descriptive message"
```

[use of assert in Python](https://stackoverflow.com/questions/5142418/what-is-the-use-of-assert-in-python)

`assertRaises` method can test if the unit is raising the right error with given parameters:
```python
def test_assert_error(self):
	self.asserRaises(ValueError, method_name, parameter1, parameter2)
```

## Examples

```python
import <your function>
import unittest

class TestingSomething(unittest.TestCase):
	def test_basic(self):
		testcase = 'some stuff'
		expected = 'other stuff
		self.assertEqual(<yourfunction>(testcase), expected)

unittest.main()
```

# Troubleshooting

## Commands

### strace

Outputs all systems call that the program made and their return status
```shell
$> strace ./my_program
```

### ltrace

Same as previous but outputs library calls
```shell
$> ltrace ./my_program
```

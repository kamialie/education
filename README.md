# Unix/Linux tools

http://www.yourownlinux.com/2015/04/sed-command-in-linux-append-and-insert-lines-to-file.html

https://stackoverflow.com/questions/25631989/sed-insert-line-command-osx

[Makefile](https://www.gnu.org/software/make/manual/)

man hier - file hierarchy

[Ctrl][Z] - put the current task to background

fg - bring the background process back to foreground

**Double-quoted string is processed first by the shell**

# Contents

+ [Basics](#basics)
+ [ssh](#ssh)
+ [sed](#sed)
+ [diff](#diff)
+ [curl](#curl)
+ [tar](#tar)

## Basics

+ **date**
+ **calendar**
+ **free** - shows free space (only works on linux)
+ **file** - determines file type and some additional info
+ **less** - view file contents
+ **pbcopy**, **pbpaste** - copy or paste to the pastboard

## ssh

### ssh-keygen

+ [simple example](https://help.github.com/en/github/authenticating-to-github/generating-a-new-ssh-key-and-adding-it-to-the-ssh-agent)
```shell
ssh-keygen -t rsa -b 4096
```

### ssh-copy-id

+ [doc](https://www.ssh.com/ssh/copy-id)
```shell
ssh-copy-id -i ~/.ssh/id_rsa.pub user@host
```

### scp

Work as much the same as normal cp command.

+ [example](https://www.simplified.guide/ssh/copy-file)

All directory leading to the last inner one must exist.

```shell
scp myfile.txt remoteuser@remoteserver:/remote/folder/inner_folder
```

## sed

Stream editor

### Flags

**sed [flags] [file ...]**

+ `-i <extenstion>` - edit file in-place, saving backup with the specified extenstion; if zero-length extension is given, no backup will be saved

### Examples

var=smth

sed **-i** '' "\<number\>s/.\*/$var/" \<target_file\> - substitute given line (number) with a variable, second argument is a regex, current example is substitute whole line; if variable contains '/' use different separator - '|'; same with **-a** means append, **-i** means insert

sed **-i** '' '/pattern to match/d' \<target_file\> - delete lines matching patterns

## diff

**diff [flags]... files**
compare files line by line

also see [cmp use](#https://stackoverflow.com/questions/12900538/fastest-way-to-tell-if-two-files-are-the-same-in-unix-linux)

### Flags

+ **-brief**, **-q** - output only whether files differ

## curl

[basic info](https://gist.github.com/subfuzion/08c5d85437d5d4f00e58)

When sending POST request with a **-L** flag (redirection), the follow up request (if needed) will be GET, instead of POST.

### Flags

+ **--include**, **-i**  - include the HTTP-header in the output
+ **--output**, **-o** *\<file\>* - write output to *\<file\>* instead of stdout. Can specify multiple arguments, order does not matter, just the first **-o** is for the first argument(url), second for the second and so on.
+ **--fail**, **-f** - fail silently, suppress error messages that are sent on failed attempt of server to send a document
+ **--location**, **-L** - if requested page has moved to a different location (and indicated it), curl will redo the request to the new location; if used with option **-i** or **-I**, headers from all requested pages will be shown; curl will pass user+password only to the initial host; to limit number of redirect use **--max-redirs** option
+ **--verbose**, **-v** - verbose output, lines starting with '>' means header data sent by curl, '<' - header data recieved by curl, '\*' - additional info. More details my be provided by **--trace** flag
+ **--silent**, **-s** - silent or quiet, dont show progress meter or error messages
+ **--show-error**, **-S** - when used with **-s** makes curl show the error message
+ **--user**, **-u** *\<user:password\>* - specify user, password for server authentication. If only passed user, curl will prompt for password
+ **--data**, **-d** \<data\> - sends specified data in a POST request; can send data directly or read from a file - to specify a while add **@** in front, for example -d "@filename.json"; two formats are available:
	- application/x-www-form-urlencoded - param-value pairs seperated by **&** sign, "param1:value1&param2:value2"
	- application/json - usual json, '{"key":"value"}'

## tar

Manipulate tape archives

### Flags

**tar**
+ `-c, --create [options] [files | directories]` - create a new archive containing specified items
+ `-x, --extract` - extract to disk from the archive; if a file with the same name appears more than once, later once overwrite earlier copies
+ `-f, --file <file>` - read the archive or write the archive to the specified file; the filename can be '-' for standard input or output
+ `-v, --verbose` - produce verbose output
+ `-z, --gunzip, --gzip` - compress the resulting archive with gzip

### Examples

+ `tar -cvf <name.tar> <path_to_directory>` - create an archive of a directory; add `
+ `tar -x` - extract all entries

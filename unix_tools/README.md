# Unix/Linux tools

http://www.yourownlinux.com/2015/04/sed-command-in-linux-append-and-insert-lines-to-file.html
https://stackoverflow.com/questions/25631989/sed-insert-line-command-osx

**Double-quoted string is processed first by the shell**

# Contents

* [sed](#sed)
* [diff](#diff)
* [curl](#curl)

## sed

Stream editor

### Flags

**sed [flags] [file ...]**

* **-i** *\<extenstion\>* - edit file in-place, saving backup with the specified extenstion; if zero-length extension is given, no backup will be saved

### Examples

var=smth
sed -i '' "*\<number\>*s/.\*/$var/" test.txt - substitute given number with a variable; if variable contains '/' user different separator - '|'; same with a instead of a means append, i means insert


## diff

**diff [flags]... files**
compare files line by line

also see [cmp use](#https://stackoverflow.com/questions/12900538/fastest-way-to-tell-if-two-files-are-the-same-in-unix-linux)

### Flags

* **-brief**, **-q** - output only whether files differ

## curl

### Flags

* **-i**, **--include** - include the HTTP-header in the output

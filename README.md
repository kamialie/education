# Unix/Linux tools

http://www.yourownlinux.com/2015/04/sed-command-in-linux-append-and-insert-lines-to-file.html

https://stackoverflow.com/questions/25631989/sed-insert-line-command-osx

[GNU project programs docs](http://www.gnu.org/manual/manual.html)

[Makefile](https://www.gnu.org/software/make/manual/)

man hier - file hierarchy

[Ctrl][Z] - put the current task to background

fg - bring the background process back to foreground

**Double-quoted string is processed first by the shell**

# Contents

+ [Basics](#basics)
	+ [Redirection](#redirection)
+ [Text processing](#text-processing)
	+ [sed](#sed)
	+ [diff](#diff)
+ [Permissions](#permissions)
+ [Processes](#processes)
+ [Configuration and environment](#configuration-and-environment)
+ [Network](#network)
	+ [curl](#curl)
+ [Archive](#archive)
	+ [tar](#tar)
+ [Extra](#extra)
	+ [ssh](#ssh)


## Basics

+ **date**
+ **cal** - calendar
+ **free** - shows free space (only works on linux)
+ **file** \<file\> - determines file type and some additional info
+ **less** \<file\> - view file contents
+ **pbcopy**, **pbpaste** - copy or paste to the pasteboard
	```bash
	pbcopy < file
	pbpaste > file
	```
+ **cp**
	- [-a | --archive] - copy files and directories and all their attributes; normally cp take the default attributes of the user performing the copy
	- [-i | --interactive] - prompt for confirmation otherwise files could be overwritten silently
	- [-r | --recursive]
	- [-u | --update] - only copy files that do not exist or are newer than the existing ones in the destination directory
+ **mv**
	- [-u | --update]
	- [-i | --interactive] 
+ **ln** \<file\> \<link\>
	- [-s] - create soft link
+ **type** \<command\> - displays the kind of command the shell will execute (could be built-in, executable, shell function or alias)
+ **which** \<executable\> - determines the exact location of the given executable (only works for executables)
+ **help** \<builtin\> - man page for built-ins
+ **man** [\<section\>] \<program\> - sections:
	1. User commands
	2. Programming interfaces kernel system calls
	3. Programming interfaces to the C library
	4. Special files such as device nodes and driver
	5. File formats
	6. Games and amusements such as screen savers
	7. Miscellaneous
	8. System administration commands
+ **apropos** \<search\> - search for man pages based on argument (same as `man -k`)
+ **info** \<program\> - GNU project provided man pages; works as a tree structured nodes, which are also hyperlinked(leading asterisk); the above utilities are part of `coreutils` package - `info coreutils` will show a list of programs that are part of it; navigation:
	- `?` - help
	- n - display next node
	- p - previous node
	- u - parent node
	- `Enter` - follow hyperlink
	- q - quit
+ **zless** - special `less` command to view gzip-compressed text files (have .gz extension); some programs have their doc files in `/usr/share/doc` directory
+ **alias** - create custom aliases; exactly as below - no whitespaces around equal sign; `unalias` - to remove alias; alias without argument lists all existing aliases

	alias <name>=<string>

+ **script** \<file\> - start shell session recording

---

### Redirection

Redirection operator `>` takes the sdtout by default. `>>` appends the content. File descriptor 2 refers to `stderr`, thus `2>` redirects the standard error.

Truncate file or create a new one with the following trick:
```bash
$ > file.txt
```

Capture both stdout and stderr by either of the methods (first one works on older versions as well); third option is to append content using more recent version:
```bash
$ ls > file.txt 2>&1
$ ls &> file.txt
$ ls &>> file.txt
```

`/dev/null` - bit bucker 'file', thus suppress any messages:
```bash
$ ls 2> /dev/null
```

Handy way to examine any command output:
```bash
$ ls /usr/bin | less
```

Difference between `>` and `|`:

	command > file
	command | command

Pipelines can include multiple commands, which would act like filters; examples and useful commands below:
```
$ ls /bin /usr/bin | sort | uniq | less
$ ls /bin /usr/bin | sort | uniq | wc -l
```

**uniq** - takes data from stdin or single fill and removes the duplicates
	- `-d` - returns the list of duplicates
**wc** - word count, returns number of lines, words and bytes
	- `-l` - only reports lines
**head**, **tail** - output first, last 10 lines respectively
	- `-n <number>` - change the default 10 line behavior
	- `-f` - output file content in real-time (works with `tail`)
**tee** [\<file\> ...] - acts as an intermediate fitting - sends incoming input both to a file and further to stdout of a pipeline; example below:
```bash
$ ls /usr/bin | tee ls.txt | grep zip
```

---

[back to contents](#contents)

## Text processing

### grep

**grep pattern [files ...]**

File pattern searcher

Default returns the lines containing the pattern

#### Flags

+ `-i` - ignore case (default is case sensitive)
+ `-v` - print only lines that do not match the pattern

---

### diff

**diff [flags]... files**
compare files line by line

also see [cmp use](#https://stackoverflow.com/questions/12900538/fastest-way-to-tell-if-two-files-are-the-same-in-unix-linux)

#### Flags

+ **-brief**, **-q** - output only whether files differ

---

### sed

Stream editor

#### Flags

**sed [flags] [file ...]**

+ `-i <extenstion>` - edit file in-place, saving backup with the specified extension; if zero-length extension is given, no backup will be saved

#### Examples

var=smth

sed **-i** '' "\<number\>s/.\*/$var/" \<target_file\> - substitute given line (number) with a variable, second argument is a regex, current example is substitute whole line; if variable contains '/' use different separator - '|'; same with **-a** means append, **-i** means insert

sed **-i** '' '/pattern to match/d' \<target_file\> - delete lines matching patterns

---

[back to contents](#contents)

## Permissions

Unix system has 3 groups of permissions: user, group, others (world). To check info about your identity use `id` command with no arguments.

**Attributes to to file type**:

+ `-` - regular file
+ `d` - directory
+ `l` - symbolic link (has "dumb" permissions - 777)
+ `c` - character special file, refers to device that handles data as stream of bytes
+ `b` - block special file, refers to device that handles data in blocks

**Permission attributes**:

+ `r` - file can be opened and read, directory contents can be listed if execute attribute is also set
+ `w` - file can be written or truncated (deletion and renaming are determined by directory attributes), directory allows files to be created, deleted and renamed of execute attribute is also set
+ `x` - file can be treated as a program, in scripting languages must also be set readable to be executed, directory can be entered

**Special permissions**:

+ setuid bit - when applied to an exec file, sets the *effective user ID* from the real user (running the program) to the program's owner; usually is given to few programs owner by superuser, so that others users can run the program with privileged rights
	```bash
	$ chmod u+s program
	```
+ setgid bit - same as above but for a group; if set for the directory, newly created files in the directory will be given group ownership of the directory, rather than the file's creator; useful for shared directory
	```bash
	$ chmod g+s program
	```
+ sticky bit - on Linux is ignored for files, but if set for the directory, prevents users from deleting or renaming files, unless the user is the owner of directory or file or is a superuser
	```bash
	$ chmod +t program
	```

**Commands**:
+ `id` - return user identity
+ `chmod <mode> <file>` - change file modes;
	+ octal representation:
		+ `r` - 4
		+ `w` - 2
		+ `x` - 1
	+ symbolic notation - involves target, action and permission, example `chomd u+x file` - give execution permission to the owner
		+ target - `u` (user), `g` (group), `o` (others, world), `a` (all)
		+ action - `+` (add permission), `-` (remove), `=` (apply only specified, remove the rest)
		+ permissions - `r`, `w`, `x`
+ `umask <mask>` - controls the default permissions given to a file when it is created; mask takes a form of 4 digits - turned on bits remove those permissions, first octal is for special permissions - setuid, setgid, sticky bit
+ `su <user>` - start new shell as another user (no user argument assumes superuser)
	+ `-l | -` - resulting shell session is a login shell for the specified user (user's environment, working directory); abbreviated form `-` may be used
	+ `-c '<command>'` - execute a single command rather than starting a new interactive command; command should be enclosed in single quotes to avoid shell expansion in the current shell
+ `sudo <command>` - a lot like `su`, but has great configuration capabilities; once a user is added to a privileged group, he enter his own password (in comparison to `su`) to perform the command; also `sudo` performs the command in the current shell (of course in current environment as well)
	+ `-l` - see privileges granted by sudo
+ `chown [<owner>][:[<group>]] <file>` - change the file owner and/or group
	+ arguments:
		+ `bob` - change from current user to bob
		+ `bob:users` - change to bob and group users
		+ `:admins` - change just the group
		+ `bob:` - change to bob and his login group
+ `passwd <user>` - change password; to change your own password no arguments are needed; superuser uses the argument to change particular user's passwd
+ `adduser`
+ `useradd`
+ `groupadd`

[wiki article on malware](http://en.wikipedia.org/wiki/Malware)

[FINISH WITH COMMANDS]


---

[back to contents](#contents)

## Processes

Kernel initializes its own activities as processes and launches program *init*, which in turn runs a series of shell scripts (*init script*) to start system services.

**PID** - process ID (init has PID 1)

**TTY** stand for *Teletype*, which refers to controlling terminal for the process

### Commands

+ `ps` - output process snapshot; default returns the processes associated with the current terminal session
	+ `x` - output all of user's processes regardless of what terminal (if any) they are controlled by; `?` in **TTY** column indicated no controlling terminal; new **STAT** column stands for *state* (process state may be followed by additional characters):

		| State | Meaning																					|
		|:-----:| ----------------------------------------------------------------------------------------- |
		| R		| running or ready to run																	|
		| S		| sleeping that is waiting for an event like keystroke or network packet					|
		| D		| uninverruptible sleep, waiting for I/O such as disk drive									|
		| T		| stopped (has been instructed to stop														|
		| Z		| defunct of *zombie*, a child process that has terminated, but not yet cleaned by parent	|
		| <		| high priority process, *niceness*, giving more time on the CPU							|
		| N		| low priority process																		|

	+ `aux` - outputs additional columns with the following info:

		| Header | Meaning																				|
		|:-----:| --------------------------------------------------------------------------------------|
		| USER	| User ID, owner of the process															|
		| %CPU	| CPU usage in percent																	|
		| %MEM	| Memory usage in percent																|
		| VSZ	| Virtual memory size																	|
		| RSS	| Resident Set Size, amount of physical memory (RAM) the precess is using in kilobytes	|
		| START	| Time when process has started; over 24 the date is used								|

+ `top` - continuously updating (default 3 seconds) display of system processes in order of process activity.
System summary contains the following info:

	| Row	| Field			| Meaning										|
	|:-----:|:-------------:|-----------------------------------------------|
	|1		| top			| name of the program							|
	|		| 12:00:00		| current time									|
	|		| up 1:00		| uptime										|
	|		| 2 users		| # of users logged in							|
	|		| load  average	| refers to the number of processes that are waiting to run (runnable and are sharing CPU); ave for last 60 secs, 5 mins, 15 mins; under 1.0 indicates machine is not busy |
	|2		| Tasks:		| summary of processes and their states			|
	|3		| Cpu(s):		| CPU activity									|
	|		| 0.7%us		| user processes (means it is outside kernel)	|
	|		| 1.0%sy		| system (kernel) processes						|
	|		| 0.0%ni		| *nice* (low priority) processes				|
	|		| 98.3%id		| idle											|
	|		| 0.0%wa		| waiting for I/O								|
	|4		| Mem:			| physical RAM being used						|
	|5		| Swap:			| virtual memory used							|

+ `jobs` - output list of jobs launched from terminal
+ `fg` - return process to foreground
	+ `%<n>` - particular process from jobs' list
+ `bg` - send process to background
	+ `%<n>` - particular process from jobs' list
+ `kill [-signal] PID...` - send signal to process; default signal - **TERM** (Terminate); popular signals; requires superuser privileges to send signal to processes not belonging to executing user

	| Number	| Name	| Meaning																|
	|:---------:|:-----:|-----------------------------------------------------------------------|
	| 1			| HUP	| Hangup; used by many daemon programs to cause reinitialization - when received daemon restarts and re-reads its config file	|
	| 2			| INT	| Interrupt; usually terminates a program								|
	| 9			| KILL	| Kill; is sent to kernel, not a program, thus kernel immediately terminates it (also program has no chance to cleanup)	|
	| 15		| TERM	| Terminate; if program is able to receive signals it will terminate	|
	| 18		| CONT	| Continue; restore process after **STOP**								|
	| 19		| STOP	| Stop; pause process without terminating; is sent to kernel			|

other common signals:

	| Number	| Name	| Meaning											|
	|:---------:|:-----:|---------------------------------------------------|
	| 3			| QUIT	| Quit												|
	| 11		| SEGV	| Segmentation violation; illegal use of memory		|
	| 20		| TSTP	| Terminal Stop; sent to program unlike **STOP**	|
	| 28		| WINCH	| Window Change; sent when window changes size (for example top redraws itself when receives this signal	|

	+ `-l` - complete list of signals
+ `killall [-u user] [-signal] name...` - send signal to multiple processes matching a specified program or username; same as with `kill` requires superuser privileges for processes not belonging to executing user
+ `pstree` - output process list arranged in a tree-like pattern
+ `vmstat` - snapshot of system resources usage of memory, swap and disk I/O
	+ `<n>` - continuous delivery updating every n seconds

Interrupt a process with `Ctrl-C` (**INT**, Interrupt). Pause a process with `Ctrl-Z` (**TSTP**, Terminal stop).

---

[back to contents](#contents)

## Configuration and environment

Shell program reads configuration scripts, *startup files*, which define the default environment shared by all users. The following considers bash. Startup files then followed by more in home directory that define personal environment. Exact sequence depends on the type of shell session - login shell session (when prompted username and password), non-login shell session (terminal session in the GUI)

### Commands

+ `printenv` - display environment variables
	+ `variable` - display particular variable contents
+ `set` - display both shell and environment variables and defined shell functions (sorted)
+ `alias` - display defined aliases
+ `chsh -s /path/to/shell` - set default shell for the current user
+ `export VAR` - make variable available to child processes

### Interesting variables

	| Variable		| Meaning																							|
	|:---------:	|---------------------------------------------------------------------------------------------------|
	| DISPLAY		| name of display, if running in graphical environment (usually `:0`, meaning first generated by X server)		|
	| EDITOR		| program to be used for text editing																|
	| SHELL			| name of shell (if not set when shell starts, bash assigns full pathname of the current user's login shell)	|
	| HOME			| pathname of home direcotry																		|
	| LANG			| defined the character set and collation of your language											|
	| OLD_PWD		| previous working directory																		|
	| PAGER			| program to be used for paging output (often `/usr/bin/less`)										|
	| PATH			| colon-separeted list of directories that are searched when name of executable program is entered	|
	| PS1			| Prompt String 1; defines contents of shell prompt													|
	| PWD			| current working directory																			|
	| TERM			| name of terminal type, sets the protocol to be used with terminal emulator						|
	| TZ			| timezone; often unix-like systems maintain computer's internal clock in *Coordinated Universal Time* (UTC) and apply offset set by this variable	|
	| USER			| username																							|
	| HISTSIZE		| increase the size of command history from the default of 500										|
	| HISTCONTROL	| causes shell's history recording to ignore a command if the same was just recorded				|

### Startup files

General rule - add directories to **PATH** or define additional environment variables in **.bash\_profile** (or equvalent, for example Ubuntu uses **.profile**, everything else to **.bashrc**.
Popular extensions (appended to the end) for backup file - `.back`, `.sav`, `.old`, `.orig`

	| File				| Contents																					|
	|:-----------------:|-------------------------------------------------------------------------------------------|
	| /etc/profile		| global config script that applies to all users											|
	| ~/.bash_profile	| user's personal startup file, can be used to override settings in global config script	|
	| ~/.bash_login		| if previous not found, bash attempts to read this script									|
	| ~/.profile		| if previous two not found, bash attempts to read this script (default in Debian-based distributions)	|
	| /etc/bash.bashrc	| global config script that applies to all users											|
	| ~/.bashrc			| user's personal startup file, can be used to extend or override global settings (almost always read); non-login shells read by default and most of startup files read it as well	|

### More info

INVOCATION section of bash man page

---

[back to contents](#contents)

## Network

### curl

[basic info](https://gist.github.com/subfuzion/08c5d85437d5d4f00e58)

When sending POST request with a **-L** flag (redirection), the follow up request (if needed) will be GET, instead of POST.

#### Flags

+ **--include**, **-i**  - include the HTTP-header in the output
+ **--output**, **-o** *\<file\>* - write output to *\<file\>* instead of stdout. Can specify multiple arguments, order does not matter, just the first **-o** is for the first argument(url), second for the second and so on.
+ **--fail**, **-f** - fail silently, suppress error messages that are sent on failed attempt of server to send a document
+ **--location**, **-L** - if requested page has moved to a different location (and indicated it), curl will redo the request to the new location; if used with option **-i** or **-I**, headers from all requested pages will be shown; curl will pass user+password only to the initial host; to limit number of redirect use **--max-redirs** option
+ **--verbose**, **-v** - verbose output, lines starting with '>' means header data sent by curl, '<' - header data received by curl, '\*' - additional info. More details my be provided by **--trace** flag
+ **--silent**, **-s** - silent or quiet, don't show progress meter or error messages
+ **--show-error**, **-S** - when used with **-s** makes curl show the error message
+ **--user**, **-u** *\<user:password\>* - specify user, password for server authentication. If only passed user, curl will prompt for password
+ **--data**, **-d** \<data\> - sends specified data in a POST request; can send data directly or read from a file - to specify a while add **@** in front, for example -d "@filename.json"; two formats are available:
	- application/x-www-form-urlencoded - param-value pairs separated by **&** sign, "param1:value1&param2:value2"
	- application/json - usual json, '{"key":"value"}'

---

[back to contents](#contents)

## Archive

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

+ `tar -cvf <name.tar> <path_to_directory>` - create an archive of a directory
+ `tar -x` - extract all entries

---

[back to contents](#contents)

## Extra

### ssh

**ssh-keygen**

+ [simple example](https://help.github.com/en/github/authenticating-to-github/generating-a-new-ssh-key-and-adding-it-to-the-ssh-agent)
```shell
ssh-keygen -t rsa -b 4096
```

**ssh-copy-id**

+ [doc](https://www.ssh.com/ssh/copy-id)
```shell
ssh-copy-id -i <path_to_public_key> user@host
```

**scp**

Work as much the same as normal cp command.

+ [example](https://www.simplified.guide/ssh/copy-file)

All directory leading to the last inner one must exist.

```shell
scp myfile.txt remoteuser@remoteserver:/remote/folder/inner_folder
scp remoteuser@remoteserver:/remote/folder/some_file.txt path/on/host
```

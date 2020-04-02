# Unix/Linux tools

http://www.yourownlinux.com/2015/04/sed-command-in-linux-append-and-insert-lines-to-file.html

https://stackoverflow.com/questions/25631989/sed-insert-line-command-osx

[GNU project programs docs](http://www.gnu.org/manual/manual.html)

[Makefile](https://www.gnu.org/software/make/manual/)

man hier - file hierarchy

`Ctrl-Z` - put the current task to background

`fg` - bring the background process back to foreground

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
+ [Package management](#package-management)
+ [Storage](#storage) [STOPPED AT PAGE 185 - CREATING NEW FILESYSTEM]
+ [Network](#network)
	+ [curl](#curl)
+ [Searching for files](#searching-for-files)
+ [Archive](#archive)
	+ [tar](#tar)
+ [Extra](#extra)
	+ [ssh](#ssh)
+ [Resources](#resources)


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
	- `-a | --archive` - copy files and directories and all their attributes; normally cp take the default attributes of the user performing the copy
	- `-i | --interactive` - prompt for confirmation otherwise files could be overwritten silently
	- `-r | --recursive`
	- `-u | --update` - only copy files that do not exist or are newer than the existing ones in the destination directory
+ **mv**
	- `-u | --update`
	- `-i | --interactive` 
+ **ln** \<file\> \<link\>
	- `-s` - create soft link
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
	- `n` - display next node
	- `p` - previous node
	- `u` - parent node
	- `Enter` - follow hyperlink
	- `q` - quit
+ **zless** - special `less` command to view gzip-compressed text files (have .gz extension); some programs have their doc files in `/usr/share/doc` directory
+ **alias** - create custom aliases; exactly as below - no whitespaces around equal sign; `unalias` - to remove alias; alias without argument lists all existing aliases

	alias <name>=<string>

+ **script** \<file\> - start shell session recording

---

### Interesting directories

|	Directory	|	Comments	|
|:--------------|---------------|
| /				| root directory|
| /bin			| contains binaries that must be present for the system to boot and run
| /boot			| contains Linux kernel, initial RAM disk image (for drivers at boot time) and boot loader; <br/> interesting files:<ul><li>/boot/grub/grub.conf or menu.lst - used to configure boot loader</li><li>/boot/vmlinuz - Linux kernel</li></ul>	|
| /dev			| contains *device nodes*; here kernel contains list of all devices it understands
| /etc			| contains system wide configuration files (also shell scripts which start each of the system services at boot time) - everything in this directory should be readable text<br\>interesting files:<ul><li>/etc/crontab - file taht defines automated jobs</li><li>/etc/fstab - table of storage devices and their associated mount points</li><li>/etc/passwd - list of user accounts</li></ul>	|
| /home			| each user is given directory in **/home**
| /lib			| contains shared library files used by the core system programs (similar to dlls in Windows)
| /list+found	| each formatted partition or device using a Linux file system (f.e. ext3) will have this directory, which is used as partial recovory from file system corruption event
| /media		| on modern Linux systems contains the mount points for removable media such as USB drives, CD-ROMs, etc that are mounted automatically at insertion
| /mnt			| on older Linux systems contains mount points for removable devices that have been mounted manually
| /opt			| used to install "optional" software; mainly used to hold commercial software products
| /proc			| virtual file system maintained by Linux kernel; the "files" it contains are peepholes into kernel itself; files are readable and will give a picture of how kernel sees your computer
| /root			| home directory for root account
| /sbin			| contains "system" binaries; programs that perfomr vital system tasks that are generally reserved for superuser
| /tmp			| storage for temporary, transient files created by various programs; some configs cause it to be emptied each time system is rebooted
| /usr			| contains all programs and support files used by regular users
| /usr/bin		| contains executable programs installed by Linux distrib
| /usr/lib		| shared libraries for programs in **/usr/bin**
| /usr/local	| contains programs that are not included with distrib, but are intended for system-wide use; programs compiled from source code are normally installed in **/usr/local/bin**
| /usr/sbin		| contains more system administration programs
| /usr/share	| contains all shared data used by programs in **/usr/bin** - deafult config files, icons, screen backgrounds, sound file, etc
| /usr/share/doc| contains documentations files for most packages on the system
| /var			| data that is likely to change - databases, spool file, user mail, etc
| /var/log		| contains *log files*, records of various system activity

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

+ **uniq** - takes data from stdin or single fill and removes the duplicates
	- `-d` - returns the list of duplicates
+ **wc** - word count, returns number of lines, words and bytes
	- `-l` - only reports lines
+ **head**, **tail** - output first, last 10 lines respectively
	- `-n <number>` - change the default 10 line behavior
	- `-f` - output file content in real-time (works with `tail`)
+ **tee** [\<file\> ...] - acts as an intermediate fitting - sends incoming input both to a file and further to stdout of a pipeline; example below:
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

**sed [flags] [file ...]**

+ `-i <extenstion>` - edit file in-place, saving backup with the specified extension; if zero-length extension is given, no backup will be saved

Stream editor

#### Examples

var=smth

`sed -i '' "<number>s/.*/$var/" <target_file>` - substitute given line (number) with a variable, second argument is a regex, current example is substitute whole line; if variable contains '/' use different separator - '|'; same with **-a** means append, **-i** means insert

`sed -i '' '/pattern to match/d' <target_file>` - delete lines matching patterns

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

## Package management

Packaging systems:

1. Debian style (.deb) - Debian, Ubuntu, Xandros, Linspire
2. Red Hat style (.rpm) - Fedora, CentOS, Red Hat Enterprise Linux, OpenSUSE, Mandriva, PCLinuxOS

*Package file* - basic unit of software in a packaging system; may contain meta data and pre-, post-installation scripts to perform configuration. *Package maintainer* gets the software in source code from *upstream provider* (author), compiles it and creates the package metadata and any necessary installation scripts; also could apply modifications to soruce code in improve integration.

Package tools:

| Distribution			| Low-level tools	| High-level tools	|
|-----------------------|:-----------------:|:-----------------:|
| Debian-style			| dpkg				| apt-get, aptitude	|
| Fedora, RHEL, CentOS	| rpm				| yum				|

+ **Search package:**
	* Debian-style
		```bash
		apt-get update
		apt-cache search *seach_string*
		```
	* Fedora, RHEL, CentOS
		```bash
		yum search *search_string*
		```
+ **Install from repository:**
	* Debian-style
		```bash
		apt-get update
		apt-get install *package_name*
		```
	* Fedora, RHEL, CentOS
		```bash
		yum install *package_name*
		```
+ **Install from package file** (no dependancy resolution is performed)**:**
	* Debian-style
		```bash
		dpkg -install *package_file*
		```
	* Fedora, RHEL, CentOS
		```bash
		rmp -i *package_name*
		```
+ **Remove a package:**
	* Debian-style
		```bash
		apt-get remove *package_name*
		```
	* Fedora, RHEL, CentOS
		```bash
		yum erase *package_name*
		```
+ **Update a package from a repository:**
	* Debian-style
		```bash
		apt-get update
		apt-get upgrade
		```
	* Fedora, RHEL, CentOS
		```bash
		yum update
		```
+ **Update a package from a package file:**
	* Debian-style
		```bash
		dpkg --install *package_file*
		```
	* Fedora, RHEL, CentOS
		```bash
		rmp -U *package_file*
		```
+ **List installed packages:**
	* Debian-style
		```bash
		dpkg --list
		```
	* Fedora, RHEL, CentOS
		```bash
		rpm -qa
		```
+ **Determine if a package is installed:**
	* Debian-style
		```bash
		dpkg --status *package_name*
		```
	* Fedora, RHEL, CentOS
		```bash
		rmp -q *package_name*
		```
+ **Package info:**
	* Debian-style
		```bash
		apt-cache show *package_name*
		```
	* Fedora, RHEL, CentOS
		```bash
		yum info *package_name*
		```
+ **Finding which package installed a file:**
	* Debian-style
		```bash
		dpkg --search *file_name*
		```
	* Fedora, RHEL, CentOS
		```bash
		rmp -qf *file_name*
		```
	
### More info

+ [package management on Debian systems](http://www.debian.org/doc/FAQ/ch-pkgtools.en.html)
+ [RMP home page](http://www.rpm.org)
+ [YUM home page at Duke](http://linux.duke.edu/projects/yum/)
+ [metadata wiki article](http://en.wikipedia.org/wiki/Metadata)


---

[back to contents](#contents)

## Storage

`/etc/fstab` - lists the devices (typically hard disk partitions) that are to be mounted at boot time

`/etc/fstab` fields:

| Field | Contents			| Description	|
|-------|:-----------------:|---------------|
| 1		| Device			| traditionally name of device file associated with physical device (such as `/dev/hda1`); modern distributions associate device with text label, which is read by OS when device is attached	|
| 2		| Mount Point		| directory where the device is attached to the file system tree									|
| 3		| File System Type	| Linux allows many system types to be mounted; most native - `ext3`, others - `FAT16(msdos)`, `FAT32(vfat)`, `NTFS(ntfs)`, `CD-ROM(iso9600)`	|
| 4		| Options			| various options like read-only; or prevent any programs from being executed from them				|
| 5		| Frequency			| single number that specifies if and when a file system is to be backed up with the `dump` command	|
| 6		| Order				| single number that specifies in what order file systems should be checked with the `fsck` command

Mount point is simply a directory on the file system tree. If that directory has contents, they wont be available until device is unmounted. If any user or process is using the device (lets say user is exploring device's file system tree), device can not me unmounted.

OSs use buffers to speed up the reading and writing to "slow" data devices (f.e. hard disk). That is why it is important to unmount a device, which gives OS the chance to actually write data to the device, if there is anything left in buffers.

**Linux storage device names:**

| Pattern	| Device										|
|-----------|-----------------------------------------------|
| /dev/fd\*	| floppy disk drives							|
| /dev/hd\*	| IDE (PATA) disks on older systems; typical motherboards contain 2 IDE connectors or *channels*, each with a cable to 2 attachments points for drives: <ul><li>*master* device</li><li>*slave* device</li></ul>Device names are ordered such that **/dev/hda** refers to master device on first channge, **/dev/hdb** - slave device on first channel, **/dev/hdc** - master on second, and so on; Trailing digit indicates the partition number on the device - **/dev/hda1** refers to first partition, while **/dev/hda** refers to the entire drive	|
| /dev/lp\*	| printers
| /dev/sd\*	| SCSI disks; recent Linux systems kernel treats all disk-like devices (included PATA?SATa hard disjs, flash drives, and USB mass storage devices (f.e portable music players, digital cameras) as SCSI disks; the rest if teh naming is similar to the older **/dev/hd** naming scheme	|
| /dev/sr\*	| optical drives (CD/DVD readers and burners)	|

If system does not automatically mount removable devices, check the name when device is attached in **/var/log/messages** or **/var/log/syslog**. Devie name remains the same as long as device remains physically attached and computer is not rebooted.

### Commands

+ **mount** *device* *mount_point* - mount a file system; command without arguments displays currently mounted file systems with the following format - *device* on *mounting_point* type *file_system_type* (*options*)
	+ `-t` *type* - specify file system type
+ **umount** *device* - unmount a file system
+ **fsck** - check and repair a file system
+ **fdisk** - partition table manipulator
+ **mkfs** - create a file system
+ **fdformat** - format a floppy disk
+ **dd** - write block oriented data directly to a device
+ **genisoimage (mkiofs)** - create an ISO 9660 image file
+ **wodim (cdrecord)** - write data to optical storage media
+ **md5sum** - calculate an MD5 checksum

---

[back to contents](#contents)

## Network

### Examination and monitoring

#### Commands

+ **ping** - sends a special network packet (called IMCP ECHO\_REQUEST) to a speciafied host, most network devices will reply to it (may be configured to ignore IMCP traffic or this packets in particular)
	+ `-c` *count* - limit number of sent and received ECHO\_RESPONSE packets
+ **traceroute** - lists all the "hops" network traffic takes to get from the local system to a specified host; some systems use similar **tracepath** program;
	+ `-I` - use ICMP ECHO instead of UDP datagrams (synonym for "-P icmp")
+ **ip** - multi-purpose network configuration tool available in modern Linux kernels (relpaces deprecated **ifconfig**)
+ **netstat** - examine various network settings and statistics
	- `-ie` (doesn't work on mac) - shows network interfaces
	- `-r` - kernel's network routing table

### Tranporting files over network

ftp program (took name from File Transfer Protocol) is used to download files over the Internet. It communicates with ftp servers, whcich contain files that can be up- and downloaded. Web browser also support this protocol (**ftp://**). Original form is not safe as account names and passwords are sent in cleartext (thus almost all FTP over Internet is done by *anonymous* FTP servers - anyone can log in by "anonymous" name and meaningless password).

There are many other client version among which the **lftp** is more popular. It acts just like traditional plus supports other protocols (http), can run background tasks, has tab completion and other features.

#### ftp

```shell
ftp file_server
```

Invoke the ftp program and connect it to the *file_server*; login name would usually be *anonymous* and while some servers will accept empty password, others might be expecting email, thus try using *user@example.com*

+ **cd** *path/to* - normal **cd** operation; most files for public downloading are usually located under the *pub* directory
+ **ls**
+ **lcd** *path/to* - change working directory on the local machine
+ **get** *file_name* - transfer specified file
+ **bye** - log off
+ **help** - display available commands

#### wget

Another popular command-line program for downloading. Can download signle or multiple files, both from web and ftp servers, can also download sites. Among other features are background, recursive, partially completed downloading.

#### curl

[basic info](https://gist.github.com/subfuzion/08c5d85437d5d4f00e58)

When sending POST request with a `-L` flag (redirection), the follow up request (if needed) will be GET, instead of POST.

+ `include, -i`  - include the HTTP-header in the output
+ `output, -o <file>` - write output to *\<file\>* instead of stdout. Can specify multiple arguments, order does not matter, just the first **-o** is for the first argument(url), second for the second and so on.
+ `fail, -f` - fail silently, suppress error messages that are sent on failed attempt of server to send a document
+ `location, -L` - if requested page has moved to a different location (and indicated it), curl will redo the request to the new location; if used with option `-i` or `-I`, headers from all requested pages will be shown; curl will pass user+password only to the initial host; to limit number of redirect use **--max-redirs** option
+ `verbose, -v` - verbose output, lines starting with '>' means header data sent by curl, '<' - header data received by curl, '\*' - additional info. More details my be provided by `--trace` flag
+ `silent, -s` - silent or quiet, don't show progress meter or error messages
+ `show-error, -S` - when used with **-s** makes curl show the error message
+ `user, -u <user:password>` - specify user, password for server authentication. If only passed user, curl will prompt for password
+ `data, -d <data>` - sends specified data in a POST request; can send data directly or read from a file - to specify a while add **@** in front, for example -d "@filename.json"; two formats are available:
	- application/x-www-form-urlencoded - param-value pairs separated by **&** sign, "param1:value1&param2:value2"
	- application/json - usual json, '{"key":"value"}'

#### ssh

```shell
ssh [user@]remote_system [command]
```
Connect to remote host; optinally log in as specified *user*; optionally run a single *command* in remote shell

+ `-p` *port* - specify custom port

Secure protocol to communicate with remote machine. Authenticates remote server (confirms it is who it says it is, thus, preventing man-in-the-middle attack) and encrypts all communications. Authentication can fail because of 2 reasons: man-in-the-middle attack, ssh-server or remote OS was reinstalled. Error message will point to the offedning key, removing it (usually from *known_hosts* file) will let ssh client to accept the connection and add new key.

Consists of 2 parts:

+ ssh server on the remote machine, listening for incoming connections on default port 22
+ ssh client on local machine

Many Linux distros are shipped with popular **OpenSSH** from OpenBSD project, some include both client and server part (like Red Hat), while others (like Ubuntu) only supply the client part. For latter **openssh-server** package needs to be installed for server part.

SSH provides sort of encrypted tunneling for commands to safely be transmitted to remote system, but this also allows other networks traffic to be sent through creating sort of VPN. Best use is to transmit GUI from remote system to local one. `-X` or `-Y` login on remote transmits launched program's GUI to the local machine:

Most popular programs that lets run ssh (or telnet) session on Windows is [PuTTY](https://www.chiark.greenend.org.uk/~sgtatham/putty/)

```shell
ssh -X remote_system
xload
```

Run command on remote and get output to the local file (command to run on remote system needs to be enclosed in single quotes to prevent shell expansion in local shell):
```shell
ssh remote_system 'ls *' > local_file
```

#### ssh-keygen

```shell
ssh-keygen -t rsa -b 4096
```

+ [example](https://help.github.com/en/github/authenticating-to-github/generating-a-new-ssh-key-and-adding-it-to-the-ssh-agent)

#### ssh-copy-id

```shell
ssh-copy-id -i <path_to_public_key> user@host
```

+ [doc](https://www.ssh.com/ssh/copy-id)

#### scp

```shell
scp myfile.txt remote_user@remote_server:/remote/folder/inner_folder
scp remote_user@remote_server:/remote/folder/some_file.txt path/on/host
```

Works as much the same as normal cp command. All directories leading to the last inner one must exist.

+ [example](https://www.simplified.guide/ssh/copy-file)

#### sftp

```shell
sftp remote_system
```

Works pretty much like original **ftp**, but uses ssh ecnrypted tunnel. Does not require ftp server to be running on remote system, only ssh server. Many graphical file managers support sftp protocol (like GNOME or KDE), thus simply putting `sftp://` into locaton bar lets you operate on file on remote system.

---

### Extra resources

+ [Linux network administration quide](http://tldp.org/LDP/nag2/index.html)
+ Wikipedia
	- [internet protocol address](http://en.wikipedia.org/wiki/Internet_protocol_address)
	- [host name](http://en.wikipedia.org/wiki/Host_name)
	- [uniform resource identifier](http://en.wikipedia.org/wiki/Uniform_Resource_Identifier)

---

[back to contents](#contents)

## Searching for files

#### locate

**locate** *substring*

Performs rapid database search of pathnames, and the returns every name that matches the given substring. In order to work the database should already exist. Can be updating weekly or daily. **updagedb** command can be used to manually update this database, which is possible run as a cron job as well.

Two common versions on Linux distribs are **slocate** and **mlocate**, which are usually accessed by symbolic link **locate**.

---

### find

In the simplest form takes one or more arguments (directories) to search. With the use of *options*, *tests* and *actions* the default output can be specified and narrowed down.

#### Tests

`-type T` limits search to specified type; example below will limit search to directories:

```shell
find ~ -type d
```

More available options:

+ `b` - block special device file
+ `c` - cahracter special device file
+ `d` - directory
+ `f` - file
+ `l` - symbolic link

`-name PATTERN` - limits the output matching given wildcard pattern

`-size [SIGN]{NUMBER}{UNITS}` - limits outputs based on size condition; plus sign indicates *greater that*, while minus - *smaller than*, and no sign - *to match exactly*; units may be one of the following:

+ `b` - 512-byte block, default
+ `c` - bytes
+ `w` - 2-byte words
+ `k` - kilobytes (1024 bytes)
+ `M` - megabytes
+ `G` - gigabytes

Example below outputs image files with size greater than 1 megabyte:

```shell
find ~ -type f -name "*.JPG" -size +1M
```

More available options(not a complete list):

| Test				| Description	|
|-------------------|---------------|
| -cmin *n*			| match files and directories whose content or attributes were last modified exactly *n* minutes ago (to specify more than use *+n*, less than - *-n*	|
| -cnewer *file*	| match files or directories whose contents or attributes were last modified recently than those of *file*	|
| -ctime *n*		| match files or directories whose contents or attributes were last modifed *n*\*24 hours ago	|
| -empty			| match empty files and directories
| -group *name*		| match file or directories belonging to *group* (may be expressed either as a group name or as a numeric group ID	|
| -iname *pattern*	| case-insensitive **-name** test	|
| -inum *n*			| match files with inode number *n* (helpful for finding all the hard links to a particular inode	|
| -mmin *n*			| files or directories whose contents were last modified *n* minutes ago	|
| -mtime *n*		| files or directories whose contents were last modified *n*\*24 hours ago	|
| -name *pattern*	| files and directories with the specified wildcard *pattern*	|
| -newer *file*		| files or directories whose contents were modified more recently than the specified *file* (useful for writing shell scripts to perform file backups)	|
| -nouser			| files or directories that do not belong to a valid user; belonging to deleted accounts or detect activity by attackers	|
| -nogroup			| files or directories that do not belong to a valid group
| -perm *mode*		| files or directories that have permissions set to the specified *mode* (either octal or symbolic notation)
| -samefile *name*	| similar to *-inum*; match files that shre the same inode number as file *name*	|
| -size *n*			| files if size *n*	|
| -type *c*			| of type *c*	|
| -user *name*		| files or directories beloning to user *name* (may be expressed by a username or by a numeric user ID	|

Operators provide a way to combine **tests** into more complex logical relationship. Available operators:

| Operator	| Description	|
|-----------|---------------|
| -and, -a	| matches if tests on both sides are true; no operator present implies **and**	|
| -or, -a	| if one of tests are true		|
| -not, !	| if following test is false	|
| ( )		| groups tests and operators together to form larger expressions; by defaults **find** evaluates from left to right; since parenthesis have special meaning to shell, they must be quoted - usually escaped with backslash	|

Example below outputs files and directories with bad permissions:

```shell
$> find ~ \( -type f -not -perm 0600 \) -or \( -type d -not -perm 0700 \)
```

---

#### Actions

Predifined and user-defined actions provide a way to act on the itmes on the list.

| Action	| Description	|
|-----------|---------------|
| -delete	| delete the currently matching file	|
| -ls		| perform the equivalent of `ls -dils` on the matching file	|
| -print	| output the full relative pathname of the matching file; default	|
| -quit		| quit once a match has been made	|

Actions parameter acts just like any test, participating in logical expression evaluation, thus should be put at the end not to affect the precedence - in the two examples below the first one would print only if two first tests are true, while the later would print all files:

```shell
$> find ~ type -f -and -name '*.back' and -print
$> find ~ -print and type -f -and -name '*.back'
```

| User-defined actions	| Description	|
|-----------------------|---------------|
| -exec *command* {} ;	| invoke arbitrary commands; braces represent the current pathname, semicolon is a required delimiter to indicate the end of the command	|
| -ok *command* {} ;	| same as previous, but prompts user before each execution	|

When **-exec** action is used, it launches a new instance of the specified command for each argument. To combine all results into one change semicolon to plus sign.

External command **xargs** may also be used to achieve the same effect. It accepts input from standard input and converts it into an argument list for a specified comand. Number of arguments that can be placed in command line isn't unlimited. When a command line exceed the maximum, xargs executes the specified command with the maximim number of arguments possible and then repeats the process untill standard input is exhausted. To see the maximum size of the cl - `xargs --show-limits`

Examples:

+ sames as using `-delete` action(since braces and semicolon have special meanings for shell, must be either quoted or escaped):

	```shell
	$> find ~ -type f -name '*.bak' -exec rm '{}' ';'
	```
+ promts for confirmation for each execution:

	```shell
	$> find ~ -type f -name 'foo*' -ok ls pl '{} ':'
	```

+ launches ls once for all pathnames:

	```shell
	$> find ~ -type f -name 'foo*' -exec ls -l '{}' +
	```
+ same as previous, but using external **xargs** command

	```shell
	$> find ~ -type f -name 'foo*' -print | xargs ls -l
	```

Whitespaces embedded in filenames will cause problems for **xargs** and **find** comands, as they serve as delimiters for arguments. Thus **find** has option to set *null* character as delimiter in the output, `print0` action, and `--null` or `-0` for **xargs** will accept this null terminated output.

---

#### Options


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


---

[back to contents](#contents)

## Resources

+ [linuxcommand.org](http://linuxcommand.org/index.php)

### Random

+ [slashdot](https://slashdot.org/)

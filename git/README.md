### Supporting materials
* [list of supported languages for markdown code blocks](https://github.com/github/linguist/blob/master/lib/linguist/languages.yml)
* [git book](https://git-scm.com/book/en/v2)

# Contents

----------

# Git get started

Popular Distributed Version Control Systems (DVCS) - Git, Mercurial, Bazaar, Darcs.

In comparison to other VCSs, which use delta-based version control (have a base file and each version is a difference made to that file), git makes a snapshot of all files on every version. If file has not changed, stores a reference a link to previous identical file it has already stored.

Git uses SHA-1 hash mechanism for checksumming for all kind of operations as file change detection, storing, etc.

Three main states that a file can have:
- modified - file modified but not commited
- staged - marked modified file to go into next commit snapshot
- committed - data is stored in local database

[Installing git](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git)

## Setup

Configuration variables that control all aspects of Git can be stored in three different places:
- `/etc/gitconfig` file - values are applied to every user on the system and all their repositories; pass `[--system]` option to **git config** to operate on values from this file specifically
- `~/.gitconfig` or `~/.config/git/config` file - values specific to user; pass `[--global]` option to **git config** to operate on this file, affects all repositories of the user
- `.git/config` (file in the repository) - specific to single repository; `[--local]` option is the default, should be located in corresponding repository

**git config --list --show-origin** - view all settings and there they are coming from

### Identity

**git config --global user.name** "name"

**git config --global user.email** email

This information is added to every commit.

----------

### Editor

Git uses specified editor when user to type in a message. If not configured, system's default is used.

**git config --global core.editor vim**

----------

### Checking settings

**git config --list** - vew all settings

**git config <setting>** - check specific setting

**git config --show-origin <setting>** - show setting as well as where Git took it from

----------

## Help

Commands to get man pages:
- **git help \<verb\>**
- **git <verb> --help**
- **man git-\<verb\>**
- **git <verb> -h** - consice "help" output, refresher

----------

# Git basics

## Getting a git repository

**git init** - creates subdirectory (.git), repository skeleton

**git clone \<url\> [directory]** - copy existing repository to local machine, optional directory argument, where repository will be created

## Recording changes to the repository

**git status** - determine status of all files in the current repository, can be tracked (unmodified | modified | stages) or untracked

- `[--short], [-s]` - short version; 1 column is staged area, 2 column - working tree; corresponding signs are:
	+ **?** - new files
	+ **A** - added to staged area
	+ **M** - modified files

**git add <file>** - adding files to the next commit (new | modified | resolve conflicts)

`.gitignore` - add files or patterns to ignore, [examples](https://github.com/github/gitignore), it is possible to have multiple gitignores in subdirectories, which will only apply to files under that directory
- start pattern with slash (/) to avoid recursivity
	+ `/TODO` - only ignore TODO file in the current directory, not subdir/TODO
- end pattern with slash (/) to specify a directory
	+ `build/` - ignore all files in any directory named build
- negate pattern by starting it with an exclamation point (!)
	+ `*.a`
	+ `!lib.a` - do track lib.a, even though you ignore \*.a files
- use 2 asterics (\*\*) to match nested directories
	+ `doc/**/*.pdf` - ignore all pdf files in the doc/ directory and all its subdirectories

**git diff** - shows what lines that were changed but not staged yet (that is changes with staged area and local directory)

+ `[--staged], [--cached]` - shows lines that were changed and staged(difference between last commit and staged changes)

**git difftool** - allows for graphical view in configured editor, run **git difftool --tool-help** to get more details

**git commit** - commits staged changes, with no arguments launches the chosen (--global core.editor) editor - lines starting with **#** will be ignored

+ `[-v]` - in addition to default call adds the output of **diff** command
+ `[-m]` \<message\> - alternative, commit message
+ `[-a]` - skip the staging area - automatically add all files that are already tracked (does not include new (untracked) files)

**git rm** \<file\> - remove file from working directory and stage the removal

+ `[--cached] \<file\>` - remove file from stagin area, but not from working directory (for example forgot to add to gitignore)
	- `log/\*.log` - can pass files, directories and file-glob patterns, backslash before start in necessary, as git applies its own filename expansion in addition to shell's expansion

**git mv** \<file_from\> \<file_to\> - convenience command, applies **mv**, removal of old name and staging of new new (will appear as **renamed** in **git status**)

----------

## Viewing the commit history

**git log** - with no arguments lists all commits in chronological order (from most recent to later ones); pretty option with oneline or format specifiers work particularly useful with graph option

+ `[-patch], [-p]` - show difference introduced in each commit
+ `[--stat]` - summarizing option - what is change, how much, etc
+ `[--shortstat]` - same as above, but obviously short
+ `[--pretty]=[oneline | short | full | fuller | format]` - outline info in different format than the default; all except *format* are builtins, while format lets you create your own (see below)
	- `git log --pretty=format:"%h - %an, %ar : %s"` - example
	- %H - commit hash
	- %h - abbreviated form
	- %T - tree hash
	- %t - abbreviated form
	- %P - parent hashes
	- %p - abbreviated form
	- %an - author name
	- %ae - ... email
	- %ad - ... date
	- %ar - ... date, relative
	- %cn - commiter name
	- %ce - ... email
	- %cd - ... date
	- %cr - ... date, relative
	- %s - subject
+ `[--graph]` - nice little ASCII graph
+ `[--name-only]` - show the names of file modified after each commit info
+ `[--name-status]` - same as above with added/modified/deleted info as well
+ `[--abbrev-commit]` - abbreviated form for checksum
+ `[--relative-date]` - display date in relative format
+ `[--oneline]` - shorthand for --pretty=oneline --abbrev-commit together

----------

## Limiting log output

- `-<n>` - where n is an integer show last `n` commits
- `[--since]` or `[--until]` - specify the date; can accept different formats:
	+ "2.weeks"
	+ "2009-01-15"
	+ "2 years 1 day 3 minutes ago"
- `[--author]` - show commits from particular author
- `[--grep]` - search for keywords in the commit messages
- `[-S] <string>` - Git's "pickaxe" option, takes a string and show only those commits that changed number of occurences
- `[--] /path/to/file/or/directory` - output commits that introduced a change to those files
- `[--no-merges]` - not merge commits

----------

# Stuff to check out
- **bisect** command which is used to find where "the feature" was broken fisrt - can pass the script to check it

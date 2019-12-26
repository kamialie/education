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
- /etc/gitconfig file - values are applied to every user on the system and all their repositories; pass [--system] option to **git config** to operate on values from this file specifically
- ~/.gitconfig or ~/.config/git/config file - values specific to user; pass [--global] option to **git config** to operate on this file, affects all repositories of the user
- .git/config (file in the repository) - specific to single repository; [--local] option is the default, should be located in corresponding repository

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
- **git help <verb>**
- **git <verb> --help**
- **man git-<verb>**
- **git <verb> -h** - consice "help" output, refresher

# Git basics

## Getting a git repository

**git init** - creates subdirectory (.git), repository skeleton
**git clone <url> [directory]** - copy existing repository to local machine, optional directory argument, where repository will be created

## Recording changes to the repository

- **git status** - determine status of all files in the current repository, can be tracked (unmodified | modified | stages) or untracked
	+ [--short], [-s] - short version; ? - new files, A - added to staged area, M - modified files; 1 column - staged area, 2 column - working tree
- **git add <file>** - adding files to the next commit (new | modified | resolve conflicts)

.gitirnore - add files or patterns to ignore, [examples](https://github.com/github/gitignore), it is possible to have multiple gitignores in subdirectories, which will only apply to files under that directory
- start pattern with slash (/) to avoid recursivity
	+ /TODO - only ignore TODO file in the current directory, not subdir/TODO
- end pattern with slash (/) to specify a directory
	+ build/ - ignore all files in any directory named build
- negate pattern by starting it with an exclamation point (!)
	+ \*.a
	+ !lib.a - do track lib.a, even though you ignore \*.a files
- use 2 asterics (\*\*) to match nested directories
	+ doc/\*\*/\*.pdf - ignore all pdf files in the doc/ directory and all its subdirectories

**git diff** - shows what lines that were changed but not staged yet (that is changes with staged area and local directory)
	+ [--staged], [--cached] - shows lines that were changed and staged(difference between last commit and staged changes)

**git difftool** - allows for graphical view in configured editor, run **git difftool --tool-help** to get more details

----------

# Stuff to check out
- **bisect** command which is used to find where "the feature" was broken fisrt - can pass the script to check it

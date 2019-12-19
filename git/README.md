### Supporting materials
* [list of supported languages for markdown code blocks](https://github.com/github/linguist/blob/master/lib/linguist/languages.yml)
* [git book](https://git-scm.com/book/en/v2)

# Contents

----------

# Git basics

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

### Editor

Git uses specified editor when user to type in a message. If not configured, system's default is used.

**git config --global core.editor vim**

### Checking settings

**git config --list** - vew all settings
**git config <setting>** - check specific setting
**git config --show-origin <setting>** - show setting as well as where Git took it from

## Help

Commands to get man pages:
- **git help <verb>**
- **git <verb> --help**
- **man git-<verb>**
- **git <verb> -h** - consice "help" output, refresher

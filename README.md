### Supporting materials
* [list of supported languages for markdown code blocks](https://github.com/github/linguist/blob/master/lib/linguist/languages.yml)
* [git book](https://git-scm.com/book/en/v2)

# Contents

+ [Get started](#git-get-started)
	- [Setup](#setup)
	- [Help](#help)
+ [Git basics](#git-basics)
	- [Getting a git repository](#getting-a-git-repository)
	- [Recording changes to the repository](#Recording-changes-to-the-repository)
	- [Viewing the commit history](#viewing-the-commit-history)
	- [Limiting log output](#limiting-log-output)
	- [Undoing things](#undoing-things)
	- [Working with remotes](#working-with-remotes)
	- [Tagging](#tagging)
	- [Git Aliases](#git-Aliases)
+ [Branching](#branching)
	- [Branches in a nutshell](#branches-in-a-nutshell)
	- [Basic branching and merging](#basic-branching-and-merging)
	- [Branch management](#branch-management)
	- [Remote branches](#remote-branches)
	- [Rebasing](#rebasing)
+ [Git tools](#git-tools)
	- [Revision selection](#revision-selection)
	- [Interactive staging](#interactive-staging)
	- [Stashing and cleaning](#stashing-and-cleaning)
	- [Searching](#searching)
	- [Rewriting history](#rewriting-history)

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

Set identity(this information is added to every commit):
+ **git config --global user.name** "name"
+ **git config --global user.email** email

Git uses specified editor when user needs to type in a message. If not configured, system's default is used:
+ **git config --global core.editor vim**

Checking settings:
+ **git config --list** - view all settings
+ **git config <setting>** - check specific setting
+ **git config --show-origin <setting>** - show setting as well as where Git took it from
+ **git config --list --show-origin** - view all settings and there they are coming from

Turn on autocompletion in bash:
+ move `git-completion.bash` file to home directory
+ source it to bashrc with the following line - `. ~/git-completion.bash`

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

+ `[--origin], [-o] <remote>` - give other name to refer to remote, other than the default name `origin`

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

## Undoing things

**git commit**

+ `[--amend]` - redo last commit (in case of forgeting to add some files, changing commit message, making additional changes), in other words take the staging area and uses it for the commit (actually takes last commit away, creates new one and pushes it over); example:
	- git commit -m "initial commit"
	- git add forgotten_file
	- git commit --amend

**git reset \<reference\> \<path/to/file/or/directory\>** - reset the specified file, thus its becomes unstaged

+ git reset HEAD some_file - example

**git checkout -- \<path/to/file/or/directory\>** - unmodify a file, revert it to last commit

----------

## Working with remotes

Remote repositories are version of the project that are hosted elsewhere (can be remote server, network, internet, even local machine).

Origin - default name Git gives to the server you clone from

**git remote** - list remote servers you have configured

+ `[-v]` - show URLs as well
+ `add <shortname> <url>` - add a remote
+ `show <remote>` - see more info about particular remote
+ `rename <old_name> <new_name>` - rename remotes short name (changes references as well - <old_name>/master to <new_name>/master
+ `remove <remote>` or `rm <remote>` - remove remote, all remote-tracking branches and configs are deleted as well

Git fetch pulls down all data from the remote project that you dont have locally. After it you have all references to all branches locally and cna merge or inspect at any time. Does not merge data automatically. If current branch is set up to track a remote branch, then **git pull** automatically does **git fetch** and then tries to merge the changes.

**git fetch \<remote\>** - get the data from the remote server

**git push \<remote\> \<branch\>** - example, push changes to remote master branch

**git push push \<remote> \<local_name\>:\<remote_name>** - above is the shortcut to this; use this one to name the remote branch differenly than the local

----------

## Tagging

Tags are used to explicitly mark certain points in history.

To create a lightweight tag dont pass any additional options - `git tag v1.4-lw`

Types of tags:

+ lightweight - just like branch that doesn't change, in other words pointed to specific commit
+ annotated - stored as full object in Git database (checksummed as well), since it contains additional information - tagger name, email, and date. (can also be signed and verified by GNU Privacy Guard (GPG))

**git tag \<pattern\>** - list existing tags, optional pattern (enclosed in double quotes) can be passed as well

+ `[--list], [-l]` - act like default for entire list of tags; if pattern is supplied flag is mandatory for desiered behavior
+ `[-a] <tag_name> <commit_checksum>` - create annotated tag; optional \<commit_checksum\> parameter to tag past commits (short or full version of checksum)
+ `[-m] <message>` - specify taggin mesage
+ `[-d] <tag_name>` - remove a tag locally

**git show \<tag_name\>** - show the tag data alongside the commit info

**git push \<remote_name\>**

+  `<tag_name>` - push tag to remote server
+ `--tags` - push all tags at once (pushes both types of tags)
+ `--delete <tag_name>` - remove tag from remote server
+ `:ref/tags/<tag_name>` - way to interpret - read the value before colon as null, thus pushing null == deleting tag

**git checkout \<tag_name\>** - checkout to the commit by passing a tag that points to it

----------

## Git Aliases

Simple as any other aliasing - just replace "long" command with you alias!

**git config --global alias.\<alias\> \<command\>** - if command consists of multiple arguments, its best to enclose them in single quotes

Good examples:

+ `git config --global alias.co checkout`
+ `git config --global alias.br branch`
+ `git config --global alias.ci commit`
+ `git config --global alias.st status`
+ `git config --global alias.last 'log -1 HEAD'`

----------

# Git Branching

## Branches in a nutshell

+ Blob - represents the metadata for individual file
+ Tree - lists the contents of the directory and specifies wich file name are stored as which blobs
+ Commit - contains the pointer to the tree and all commit metadata; if its not a root commit, it also contains pointers to parent(s)

Thus when git stages files, it computes checksums for each file and stores that version of the file in the Git repository (blob). When git performs commit command, it checksums each subdirectory of the project and stores them as a tree object. Commit objects contains commit metadata and pointers to the trees

**git branch** - list all local branches

**git branch \<name\>** - create a new branch; actually creates a new pointer to the commit you are currently on

To keep track where you are currently on Git uses special pointer `HEAD` - it shows where you are on in the local repository.

**git checkout <tag | commit | branch>** - switch to the provided commit, branch (moves `HEAD`)

**git checkout -b \<name\>** - create and switch to a branch at the same time

Summary: creatig a branch in Git is just writing small metadata file, which acts like a pointer to commit.

----------

## Basic branching and merging

The below examples contain following setup: one branch checkouts from master to do additional work. Some work was committed to this additional branch and now source branch is what you want to merge(additional branch) and target branch (master) is where you want to merge it

To perform merge checkout out to target branch and type in `git merge <source_branch`.

Performing merge can lead to 2 cases :

+ source branch can be reach by following target branch's commit history, that is source branch was directly ahead of target branch - this case lead to fast-forward "merge", git just moves the pointer ahead
+ source branch have diverged from target branch and there are no conflicts, so git creates new snapshot (merge commit), which results from two tips of branches and their common ancestor
+ source branch have diverged from target branch and there are some conflits, so git pauses and until you resolve conflicts (then you can add-commit the work); list unmerged files by running `git status`

**git mergetool** - open graphical tool to resolve conflicts, append tool name to use other than default
**git mergetool --tool-help** - get help (for example tool is not configured)

**git branch -d <name>** - delete branch

## Branch management

**git branch**

+ `-v` - list branches and their last commits as well
+ `--merged` - list branches that are already merged into the branch you are currently on (good candidates to be deleted, since the current branch contains their work)
+ `--no-merged` - list branches that contain work that have not been merged into branch you are currently on (trying to delete this branch will result in warning)
+ `-D <branch>` - force deleting

`--merged` and `--no-merged` are relative to current branch if no argument is passed

## Remote branches

**git ls-remote \<remote\>** or **git remote show \<remote\>** - get full list of remote references or  remote branches with more info respectively

Remote branch's name takes the form `<remote>/<branch>`, for example `origin/master`

You have cloned a repository (thus your master and origin/master refer to the same commit), but later on someone has push to server and you have commited further locally. Run `git fetch <remote>` to get the latest version of origin to you local repository.

**git checkout -b \<branch\> \<remote\>/\<branch\>** - merge remote branch into current working branch, thus local branch track the remote one

**git checkout --track \<remote\>/\<branch\>** - create and check out to "tracking branch" - local branch that has direct relationship with remote branch (git know where to push, pull, etc)

**git checkout \<branch\>** - shortcut for the above, if branch does not exist locally  (and only exists on one remote) Git creates "tracking-branch" automatically

**git branch**

+ `[--set-upstream], [-u]` - set the upstream branch to track for local branch you are on
+ `[-vv]` - also output tracking branches (if local branch is tracking remote one), also if current branch is ahead or behind; does the comparison based on local references, thus do `git fetch --all` to get up date information

**git pull** - is essentially `git fetch` followed by `git merge`

**git push <remote> --delete <branch>** - delete remote branch

## Rebasing

Setup: source branch checks out from target and does some work, while otehr work was done on target as well. Then source branch tries to perform rebase.

To perform rebase checkout to source branch and type `git rebase <target_branch`.

**git rebase \target_branch\>** - rebase current branch into target branch

+ `[--onto] <parent_a> <parent_b> <source_branch>` - in the situation when parent_b diverged from parent_a and source_branch further diverged from parent_b use the following to rebase source_branch to the parent_a
+ `[--continue]` - restart rebase after revosling conflicts
+ `[--about]` - abort the rebase operation

Operation: going to common ancestor, getting the diff introduced by each commit of the branch you are on, saving the diffs to temporary files, resetting the current branch to the same commit as the target branch, and applying each change.

Thus rebasing just replays the changes that took place in one branch in another branch, while classic merge takes endpoint and merges them together. After rebase is done the target branch can be simply fastforwarded by `git merge <source_branch>`

**Do not rebase commits that exist outside your repository and that people may have based work on** - golden rule; that is when you rebase you introduce new commits, while abandoning other, while other commits may have been used by others when they merged their work. Try to run `git pull --rebase` or `git fetch ; git rebase <remote>/<branch>` to let git try to rebase your work over rebase other people have performed earlier (and you work was done on top of the old commits that were abandoned because of that rebase). If new rebased commit is nearly iddentical to the one that was on separate branch, git might be able to successfuly apply rebase of your branch as well.

Good rule of thumb is to rebase work you havent pushed anywhere.

----------

# Git tools

## Revision selection

Git provides number of ways to refer to single commit, set of commits, or range of commits

+ Single revision
	- 40-character full SHA-1
	- short SHA-1 - can pass short version as long as its not ambiguous (no other commit has it); pass `--abbrev-commit` to `git log` to output shorter versions (but still unique), defaults to 7, but gets longer to keep them unique
	- branch reference - the commit at the tip of the branch; `rev-parse` is a low level operation that name resolves too - `git rev-parse <branch_name>` will output its SHA-1

Git keeps logs of where your HEAD and branch references have been in the last few months; this information is strictly local and represents only local changes/info; use the command below:

**git reflog** 

**git show <reference>** - can refer to reflog data as well, examples below (substitute \<reference\>:

+ `HEAD@{5}` - 5th entry from `git reflog`
+ `master@{yesterday}` - using syntax to access specific dates

**git log -g** - see reflog info in `git log` output

Ancestry reference uses `^` (caret) symbol. If added at the end of a reference, it is resolved to the parent of that commit - `git show HEAD^` resolved to the commit before current one. Can also specify number after caret to specify which parent (useful for merge commits) - `HEAD^2` - first parent is the branch you were on when performing the merge, while second is the one that is merged.

Another ancestry reference is `~` (tilde). Always goes to the first parent, thus `HEAD~2` means *first parent of the first parent*. Two syntaxes can be also combined `HEAD~3^2`

Double-dot syntax - shows range of commits that are reachiable from on commit, but aren't from another, example

	A->B->E->F->master
	   |->C->D->experiment

`git log master..experiment` - will output D and C, while vice versa will return F and E

`git origin/master..HEAD` - shows what commits are going to be pushed to the server (can ommit HEAD, git will substitute it)

`^` or `--not` before any reference means do not show reachable commits from that reference; can specify more than two reference (unlike 2 dot)

`git log refA..refB` is the same as `git log ^refA refB` and same as `git log refB --not refA`

Triple-dot show commits that are reachable by eiether of two references, but not by both; `--left-rght` options helps to determined which commits are reachable by which of the two references:

`git log master...experiment` returns F E D C

----------

## Interactive staging

**git add [-i | --interactive]** - interactive shell mode

Patch option lets you stage separate hunks of file. Patch option `[-p | --patch]` also works with `add`, `reset`, `checkout` and `stash save` commands

----------

## Stashing and cleaning

**git stash** or **git stash push** - stash changed files (dirty state) onto separate stack

+ `[list]` - show the list of stashed files on the stack; following commands, if not specified the index, assume that operation is to be done on the top most item
+ `[apply] [--index] stash@{n}` - apply the file in the current state (branch and directory); does not drop the file from stash stack; `--index` flag tell git to apply it to the staging area; can pass `n` to specify with file to apply
+ `[drop] stash@{n}` - drop item from the stack
+ `[pop] stash@{n}` - apply the stash and drop from stack
+ `[--include-untracked | -u]` - include untracked files
+ `[--all | -a]` - include all files, including ignored ones
+ `[--patch]` - interactive stashing
+ `branch <new branchname>` - create a new branch from a stashed work; creates new branch -> checks out the commit you were on -> applies stashed items -> drops the stash if applied successfully

**git clean** - removes untracked files irreversibly; safer option is to run `git stash --all`

+ `[-f]` - force, "really do this" - required flag by Git, unless configuration variable `clean.requiredForce` is not set to false; second `-f` might be needed if you try to removed submodules
+ `[-d]` - remove directories that become empty as a result of clean command
+ `[--dry-run | -n]` - show what would have been removed; that is put `-n` instead of `-f` to inspect first
+ `[-x]` - removed ignored files as well
+ `[-i]` - interactive mode

----------

## Searching

**git grep** - search withtin working directory or anywhere in the git history

+ `[--line-number | -n]` - prints out line numbers where matches were found as well
+ `[--count | -c]` - show summary with filenames containing the match and number of matches in each
+ `[--break]` - break each file (insert new line after each file) output
+ `[--heading]` - instead of starting each entry with the filename containing it, output filename on the separate line, then lines with the match
+ `[add more]`

**git log**

+ `[-S] <string>` - output commits that changes the number of occurences of provided string
+ `[-G]`
+ `[-L] :<function>:<file_name>` - line log search, git will try to figure out the bounds of the provided function and output the changes made to it throughout whole history; if git cant figure out the bounds, pass the regular expression, for the provided example it would be `git log -L '/unsigned long function/',/^}/:file_name`; can also pass range of lines or single line

----------

## Rewriting history

All the following is preformed locally. Once the work is pushed to the server, it is completely different story.

**git commit --ammend** - modify last commit message; opens configured editor and loads it once editor is closed; can also be used to change the contents - simply make changes, stage them and then perform the command (changes the hash of commit)

+ `[--no-edit]` - avoid editor session (for example adding files or changes, and there is no need to change the message)

To change multiple commits use `git rebase` command with `-i`, for interactive, option; do not specify the commits already on the server, cuase the will be rewritten; the following example changes last 3 commits (could also specify `HEAD~2^`
```bash
git rebase -i HEAD~3
```
The command returns the list of commits (but in reverse order than `git log`):
```bash
pick f7f3f6d changed my name a bit
pick 310154e updated README formatting and added blame
pick a5f4a0d added cat-file

# Rebase 710f0f8..a5f4a0d onto 710f0f8
#
# Commands:
# p, pick <commit> = use commit
# r, reword <commit> = use commit, but edit the commit message
# e, edit <commit> = use commit, but stop for amending
# s, squash <commit> = use commit, but meld into previous commit
# f, fixup <commit> = like "squash", but discard this commit's log message
# x, exec <command> = run command (the rest of the line) using shell
# b, break = stop here (continue rebase later with 'git rebase --continue')
# d, drop <commit> = remove commit
# l, label <label> = label current HEAD with a name
# t, reset <label> = reset HEAD to a label
# m, merge [-C <commit> | -c <commit>] <label> [# <oneline>]
# .       create a merge commit using the original merge commit's
# .       message (or the oneline, if no original merge commit was
# .       specified). Use -c <commit> to reword the commit message.
#
# These lines can be re-ordered; they are executed from top to bottom.
#
# If you remove a line here THAT COMMIT WILL BE LOST.
#
# However, if you remove everything, the rebase will be aborted.
#
# Note that empty commits are commented out
```
git will perform the specified commands from top to bottom; git it will perform the script (when you close the editor);

- change commit message - change `pick` to `edit`, git will stop there, perform usual `git commit --amend`, continue with `git rebase --continue`
- reorder commits - simply reorder / delete commit entries
- squash commits - change `pick` to `squash` on commit that you want to squash to previous one, in the following example last two commits are squashed into first; git then puts you into editor to merge commit messages:

		pick <hash> commit message one
		squash <hash> commit message two
		squash <hash> commit message three

- split commit - change `pick` to `edit`; git will perform parent commits and the one indicated as `edit` also stop right after, then you can `git reset HEAD^` and redo commit(s) finishing with `git rebase --continue` when done

**git filter-branch** -  rewrite large portions of history, though python sctipt `git-filter-repo` is recommended and can be found [here](https://github.com/newren/git-filter-repo)
	+ `[--tree-filter] '<shell_command>' HEAD` - runs specified shell command on each checkout, and recommits the result
	+ `[--all]` - run in all branches
	+ `[--subdirectory-filter] <directory> HEAD` - makes the specified directory a root directory for every commit; git also automatically removes all commits that did not affect the subdirecoty [NEEDS_CHECK]
	+ `[--commit-filter]` - change the email in all commits, example below (changes hashes of all commits, not just where the change applied)
```bash
git filter-branch --commit-filter '
        if [ "$GIT_AUTHOR_EMAIL" = "schacon@localhost" ];
        then
                GIT_AUTHOR_NAME="Scott Chacon";
                GIT_AUTHOR_EMAIL="schacon@example.com";
                git commit-tree "$@";
        else
                git commit-tree "$@";
        fi' HEAD
```

----------

## Reset Demystified

3 trees (workspaces) that git system manages and manipulates:

1. HEAD - pointer to the last commit on the branch(current branch reference)
2. Index - proposed next commit(staging area)
3. Working directory (working tree) - while previous two trees store contents in `.git`, this one unpackes them so you can easily work with files; refer to this tree as sandbox, where you can try and do all the changes

Workflow goes as follows:
|<--------(checkout the project)--|------------------HEAD
|---------------------------------Index---(commit)-->|
working directory--(stage files)->|------------------|

You make changes in the current working directory, then run `git add`, which moves the modifications to staging area (Index), then `git commit` creates a snapshot, which finally makes all trees to agree.

**git reset**

+ `[--soft] <reference>` - moves the HEAD (also branch it is poiting to); `git reset --soft HEAD~` (go to parent of HEAD) moves the branch to the previous commit (undo), Index and Working directory do not change
+ `[--midex] <reference>` - the default behaviour (does the same without any flags), that is moves the HEAD and updates Index as well, all changes are also unstaged
+ `[--hard] <reference>` - moves the HEAD and updates both Index and Working directory; if commit you moved from was perfomed, it can be recovered from Git DB, but otherwise all data is overwritten

----------

# Stuff to check out
- **bisect** command which is used to find where "the feature" was broken fisrt - can pass the script to check it
- **git stash push path/to/file** - stash individual file
- **git stash -p** - ask before every stash

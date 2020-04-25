# bash, shell... 📟

[Bash home page](https://www.gnu.org/software/bash/)

[Brief documentaton](https://www.gnu.org/savannah-checkouts/gnu/bash/manual/bash.html)

[Bash FAQ](http://mywiki.wooledge.org/BashFAQ)

Bash metacharacters(must be quoted or escaped if not intendent to be used): space, tab, newline, '|', '&', ';', '(', ')', '<', '>'.

# Contents

* [Shell syntax](#shell-syntax)
	- [Quoting](#quoting)
	- [ ] [Variables](#variables)
* [Shell commands](#shell-commands)
	- [ ] [Pipelines](#pipelines)
	- [ ] [Lists of Commands](#lists-of-commands)
	- [ ] [Compound commands](#compound-commands)
		+ [x] [Looping constucts](#looping-constucts)
		+ [ ] [Conditional constructs](#conditional-constructs)
			- [x] [if](#if)
			- [x] [case](#case)
			- [x] [select](#select)
			- [x] [((...))](#((...)))
			- [ ] [[[...]]](#[[...]])
		+ [x] [Grouping commands](#grouping-commands)
	- [ ] [Coprocesses](#coprocesses)
	- [GNU Parallel](#gnu-parallel)
* [ ] [Shell functions](#shell-functions)
* [ ] [Shell parameters](#shell-parameters)
* [Shell expansions](#shell-expansions)
	- [x] [Brace](#brace)
	- [x] [Tilde](#tilde)
	- [ ] [Parameter](#parameter)
	- [x] [Command](#command-substitution)
	- [x] [Arithmetic](#arithmetic)
	- [ ] [Word splitting](#word-splitting)
	- [x] [Filename](#filename)
	- [x] [Pattern matching](#pattern-matching)
	- [x] [Quote removal](#quote-removal)
* [Redirections](#redirections)
* [Fun stuff](#fun-stuff)
* [Bash prompt](#bash-prompt)
* [Keyboard tricks](#keyboard-tricks)
* [Resources](#resources)

----------

### Quoting

'\' preserves the meaning of the next character except newline - it is treated as a line continuation. Single quotes preserve the literal meaning of each character within the quotes, cant escape single quotes with single quotes. Double quotes do the same except for '$', '`', '\', '!'(when history expansion is enabled). Backslashes that are followed without special meaning are left unmodified. Parameters '*', '@' have special meaning in double quoates.

ANSI-C quoting take the form *\$\'string\'*, for example \t, \n, \octal_number_for_unicode_symbol, etc

Interactive comments are on by default.

----------

### Variables

To access variable use '**$**' in front:
* **0** - name of the schell or shell script
* **$** - ID of the shell
* **!** - ID of the last executed background process(job, pipeline)
* **?** - exit status of the last executed foreground pipeline
* **_** - expands to the last argument of last command executed

----------

[back to contents](#contents)

## Shell commands

Simple shell commands consists of the commands itself followd by arguments, separeted by spaces.

----------

[back to contents](#contents)

### Pipelines

The format for a pipeline:
```bash
[time [-p]] [!] command1 [ | or |& command2] ...
```

A pipeline is a sequence of one or more commands separeted by one of the control operators '**|**' or '**|&**'. The output of the each command is connected via a pipe to the input of the next command (connection is performed before any redirections specified by the command).

If '**|&**' is used, command's standard error, in addition to its standard output, is connected to next command input (shorthand for 2>&1 |).

Reserved work *time* causes timing statistics to be printed for the pipeline once it finishes, the **-p** option changes the output to POSIX format.

Each command in a pipeline is executed in its own subshell, which is a separete process. The exit status of a pipeline is the exit status of the last command in the pipeline. If the reserved '**!**' precedes the pipeline, the exit status is the logical negation of the exit status.

----------

[back to contents](#contents)

### Lists of commands

A list is a sequence of one or more pipelines separeted by one of the operators (optionally terminated by '**;**', '**&**', or a newline):

		;, &, &&, ||

Of the list operators, '**&&**', '**||**' have equal precedence, followed by '**;**', '**&**', which have equal precedence.

If a command is terminated by a control operator '**&**', the shell executes the command asynchronously in a subshell (in the background). The shell doesnt wait for the command to finish and the return status is 0. Command separated by a '**;**' are executed sequentially.

AND example:
```bash
command1 && command2
```
Second command is executed of first one returns an exit status of zero (success).

OR example:
```bash
command1 || command2
```
Second command is executed if first one returns a non-zero exit status.

----------

### Compound commands

...

----------

[back to contents](#contents)

### Looping constructs

'**;**' can be replaced by one or more newlines.

#### until
```bash
until test-commands; do consequent-commands; done
```
Execute consequent commands as long as test command has an exit status which is not zero. Return status is the return status of the last consequent command, or zero if none was executed.

#### while
```bash
while test-commands; do consequent-commands; done
```
Same as above.

Counting example(`-le` stands for `less than or equal to`):

```shell
n=1
while [ $n -le 5 ]; do
	echo "Iteration $n"
	((n+=1))
done
```

### for
```bash
for name [ [in [words ...] ] ; ] do commands; done

for (( expr1 ; expr2 ; expr 3 )) ; do commands ; done
```

Expands *words* and execute *commands* once for each member in the resulting list.

Names separated by space create a list that for loop can iterate over:

```shell
for count in one two three; do echo "$count"; done
```

#### Examples

rename files:

```shell
for file in *.HTM; do
	name = $(basename "$file" .HTM)
	mv "$file" "$name.html"
done
```

----------

[back to contents](#contents)

### Conditional constructs

#### if
```shell
if test-command; then
    consequent-commands;
[elif more-test-commands; then
    more-consequent;]
[else alternate-consequents;]
fi
```

If clause evaluated when test-commands returns 0 (success status), otherwise elif, else clauses execute in turn.

`Test` evaluates the condition recieved and exits with zero if its true and with one if its false. Square bracket is an alias to `test` command, but it then also should be closed with a closing square bracket (spaces after opening and before closing brackets are important!).

Checking if string is empty can be done in two ways:

```shell
if test -n "$PATH"; then echo 'PATH is not empty'; fi
if [ -n "$PATH"; then echo "PATH is not empty"; fi
```

#### case
```bash
case word in
    [ [(] pattern [| pattern]...) command-list ;;]...
esac
```
Case will selectively execute command-list to the first pattern that matches *word*. '**|**' is used to separate patterns, '**)**' - to terminate a pattern list.

Each clause (list of patterns and associated command-list) must be terminated with '**;;**', '**;&**', or '**;;&**'. Default case can be denoted with '\*' (common idiom).
* **;;** - no subsequent matches are attempted after first pattern match
* **;&** - causes execution to continue
* **;;&** - shell tests the next clause and executes it on succcessful match

#### select
```bash
select name [in words ...]; do commands; done
```
The select constructs allow the easy generation of menus.

The set if expanded words is printed on the stderror, preceded with a number (if *in words* is ommitted, the positional parameters are printed). Then PS3 prompt is printed and a line is read from stdin. If the line is number corresponding to the given options, name is set to that value. Answer is saved is **REPLY** variable. Commands are executed until a break command is encountered.
```bash
select fname in *;
do
	echo you picked $fname \($REPLY\)
	break;
done
```

#### ((...))
```bash
((expression))
```

The arithmetic *expression* is evaluated according to the [Shell Arithmetic](#shell-arithmetic). If the value is non-zero, the return status is 0; otherwise 1. Same as [let build in](#bash-builtins)

#### [[...]]


----------

[back to contents](#contents)

### Grouping commands
```bash
( list )
{ list; }
```
Bash provides two ways to group commands to be executed as a unit. Redirection may be applied to the entire command [list](#lists-of-commands). Parentheses cause a subshell environment to be created for the command list (variable assignment do not remain), curly braces execute command list in the current shell (semicolon is required for curly braces).

Historical note: braces are reserved words and must be separated from the list by blanks or other meracharacters, parentheses are operators and are always recognized.

----------

[back to contents](#contents)

### Coprocesses
```bash
coproc [NAME] command [redirections]
```
Coproc is reserved word and forces the shell command to be executed in a subshell (as if '**&**' control operator was specified). Two-way pipe is established between executing shell and the coprocess.

Creates a corpocess named *NAME* (default *COPROC*).

[example explained](https://unix.stackexchange.com/questions/86270/how-do-you-use-the-command-coproc-in-various-shells)

----------

[back to contents](#contents)

### GNU Parallel

[to be continued]

----------

[back to contents](#contents)

## Shell functions

[to be continued]

----------

[back to contents](#contents)

## Shell parameters

[to be continued]

----------

[back to contents](#contents)

## Shell expansions

Expansion is performed on the command line after it has been split into tokens. The order is the following:
1. brace
2. tilde, parameter and variable, arithmetic, command substitution (left-to-right fashion); on some systems process substitution is also available
3. word splitting
4. filename

Quote removal is performed as the last stage.

[Tilde expansion not perfomed inside double quotes](https://unix.stackexchange.com/questions/151850/why-doesnt-the-tilde-expand-inside-double-quotes). Also read about expansions above - almost all symbols reserve their literal meaning except several ones.

All shell special characters are treated as ordinary ones inside double quotes, except `$`, `\`, `\``:
+ word-splitting, file (pathname) expanstion, tilde expansion and brace expansion are suppressed
+ parameter expansion, arithmetic expansion and command substitutions are still possible

To supress all expansions use single quotes!

----------

[back to contents](#contents)

### Brace

Spaces must be escaped if intendent between braces!

```bash
$ echo a{d,c,b}e
ade ace abe
```

Comma separeted value will be inserted(not sorted)

	{x..y[..incr]}

```bash
$ echo a{1..4..1}e
a1e a2e a3e a4e
```

Expanded inclusive from x to y. When either x or y is preceded by 0, shell attempts to force all items to have the same length(feeding zeros).
* x, y - either integers or single characters
* incr - optional, an integer

Some examples:
```bash
$ echo {001..15}
001 002 003 004 005 006 007 008 009 010 011 012 013 014 015
$ echo Front-{A,B,C}-Back
Front-A-Back Front-B-Back Front-C-Back
```

----------

[back to contents](#contents)

### Tilde

If a word begins with unquoted '~', all characters up to the first unquoted slash(or all characters if no slashes) are considered a *tilde-prefix*. If none of the characters in the *tilde-prefix* are quoted, prefix is treated as a *login name*. If it is null, tilde is replaced with HOME shell variable, if unset, the home directory of the user executing the command, otherwise associated login name home directory.

If characters are in the tilde-prefix is a number (N), optionally prefixed by a '+' or a '-', tilde-prefix is replaced with corresponding element from the directory stack, as it would be displayed with [dirs](the-directory-stack) build-in

Each variable assignment is checked for unquoted tilde-prefixes following a ':' or the first '=' - in these cases tilde expanstion is also preformed.

Examples:
* ~ - value of $HOME
* ~/forr - $HOME/foo
* ~fred/foo - subdirectory of the home directory of the user fred
* ~+/foo - $PWD/foo
* ~-/foor - $OLDPWD/foo
* ~N or ~+N - string that would be displayed by 'dirs +N'
* ~-N - string that would be displayed by 'dirs -N'

----------

[back to contents](#contents)

### Parameter

[ADD INFO FROM GNU reference]

See list of available varialbes:
```bash
$ printenv
$ env
```

Mistyped variable name will result to an empty string

----------

[back to contents](#contents)

### Command substitution
```bash
$ echo $(command)

or

$ echo `command`
```

The output of the command replaces the command itself. Command is executed in subshell, trailing new lines are deleted (embedded newlines are not deleted, but may be removed during word splitting).

Old-style backquote form retains literal meaning of backslashes (except when followed by '$', '\`', or '\\'). In paranthesis form all characters make up the command, none is treated specially.

Command substututions may be nested (with old-style escape inner backquotes with backslashes).

Some commands might need to be embedded by double quotes to preserve the way output looks, example below:
```bash
$ echo $(cal)
February 2008 Su Mo Tu We Th Fr Sa 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29
$ echo "$(call)"
	February 2008
Su Mo Tu We Th Fr Sa 
				1  2
 3  4  5  6  7  8  9
10 11 12 13 14 15 16
17 18 19 20 21 22 23
24 25 26 27 28 29
```
----------

[back to contents](#contents)

### Arithmetic
```bash
$(( expression ))
```

Evaluates the expression and substitutes the result. Expression is treated as if it is within double quotes, all tokens undergo parameter and variable expanstion, command substitution, and quote removal.

Only supports integers, operators include - `+, -, *, /, %, **`

----------

[back to contents](#contents)

### Process

[to be continued]

----------

[back to contents](#contents)

### Word splitting

[to be continued]

----------

[back to contents](#contents)

### Filename

After word splitting (unless `-f` option has been set), Bash looks for `*`, `?`, and `[` characters; if one of them appears, word is regarded as a *pattern* and is replaced with an alphabetically sorted list of filenames matching the pattern (pattern matching is described below); `.` character must be mactched explicitly, since those entries are ignored by default

Depending on `nullglob`, `failglob` options being set or not, word can be left unchanged, error message could be printed or word can be removed completely

----------

[back to contents](#contents)

### Pattern matching

Any character, other than pattern character, matches itself; backslash escapes the following character, escaping backslash is discarded; special pattern characters must be quoted to match literally

+ `*` - matches any string, including null character; two adjacent `*`s used as a single pattern will match all files and zero or more directories and subdirectories, if followed by `/`, two adjacent `*` will match only directories and subdirectories; check *globstart* option
+ `?` - matches any single character
+ [...] - matches any of enclosed characters, pair of characters separated by a hyphen denote a range (check C locale to be sure what is included in range, default `[a-dx-z]` results int `[abcdxyz]`); `!` or `^` right after `[` reverses the logic; `-` can be mtaches by including first or last character, `]` can be matched only if specified first; *character classes* can be specified within `[]`:
	- alnum, aplha, ascii, blank, cntrl, digit, graph, lower, print, punct, space, upper, word, xdigit - word class matches letter, digits and `_`

[A BIT MORE INFO IS LEFT AT GNU REFERENCE]

----------

[back to contents](#contents)

### Quote removal

After all preceding expansions, all unquoted occurences of ' \\ ', ' ' ', ' " ' that did not match one of the above expansions are removed

[to be continued]
----------

### Fun stuff

* \<esc\> + . - insert last argument of previous command (obviously doesnt work in vim mode)

----------

[back to contents](#contents)

### Links

* https://unix.stackexchange.com/questions/4782/how-to-pass-the-output-of-one-command-as-the-command-line-argument-to-another
* https://superuser.com/questions/110054/custom-vim-highlighting
* https://brettterpstra.com/2015/07/09/shell-tricks-inputrc-binding-fun/
* https://brettterpstra.com/2015/07/09/shell-tricks-inputrc-binding-fun/
* https://github.com/ycm-core/YouCompleteMe#mac-os-x-super-quick-installation
* https://realpython.com/vim-and-python-a-match-made-in-heaven/
* https://github.com/junegunn/vim-plug
* https://stackoverflow.com/questions/33177595/what-does-cc-path-to-directory-configure-do - where does bash store this variables

----------

[back to contents](#contents)

## Bash prompt

Prompt is defined by `PS1` (prompt string one) environment variable.

Unix-like systems have two complex subsustems to control terminal - `termcap` and `terminfo`.

Escape codes used in shell prompts:

| Sequence	| Value displayed												|
|:----------|---------------------------------------------------------------|
| \a		| ASCII bell													|
| \d		| Current date in day, month, date format; f.e. "Mon May 25"	|
| \h		| Hostname of the local machine minus trailing domain			|
| \H		| Full hostname													|
| \j		| number of jobs running in current shell session				|
| \l		| name of the current terminal device							|
| \n		| new line														|
| \r		| carriage return												|
| \s		| name of shell													|
| \t		| current time 24 hour h : m : s format								|
| \T		| time in 12 h format											|
| \A		| time in 12 h AM/PM format										|
| \A		| 24 h h:m format												|
| \u		| username														|
| \v		| version of the shell											|
| \V		| version and release of the shell								|
| \w		| current working directory										|
| \W		| last part of the current working directory					|
| \\!		| history number of the current command							|
| \\#		| number of commands entered during this shell session			|
| \\$		| displays $ or # instead if you have superuser privileges		|
| \\[		| signals start of one or more non-printing characters - f.e. moving cursor or changing colors	|
| \\]		| end of non-printing characters								|

Text colors(use bold attribute (1) to get new (light) colors):

| Sequence		|  Color		|
|:-------------:|---------------|
| \033[0;30m	| black			|
| \033[0;31m	| red			|
| \033[0;32m	| green			|
| \033[0;33m	| brown			|
| \033[0;34m	| blue			|
| \033[0;35m	| purple		|
| \033[0;36m	| cyan			|
| \033[0;37m	| light grey	|
| \033[1;30m	| dark grey		|
| \033[1;31m	| light red		|
| \033[1;32m	| light green	|
| \033[1;33m	| yellow		|
| \033[1;34m	| light blue	|
| \033[1;35m	| light purple	|
| \033[1;36m	| light cyan	|
| \033[1;37m	| white			|

Background colors (no not support bold):

| Sequence		|  Color	|
|:-------------:|-----------|
| \033[0;40m	| black		|
| \033[0;41m	| red		|
| \033[0;42m	| green		|
| \033[0;43m	| brown		|
| \033[0;44m	| blue		|
| \033[0;45m	| purple	|
| \033[0;46m	| cyan		|
| \033[0;47m	| light grey|

Color attributes: normal (0), bold(1), underscore (4), blinking (5), inverse (7). Many emulators ignore blinking attribute

Moving cursor:

| Sequence		|  Action					|
|:-------------:|---------------------------|
| \033[*l*;*c*H	| move the cursor to line *l* and column *c*	|
| \033[*n*A		| move up *n* lines			|
| \033[*n*B		| move down *n* lines		|
| \033[*n*C		| move forward *n* lines	|
| \033[*n*D		| move backward *n* lines	|
| \033[2J		| clear the screen and move the cursor to the upper left corner (line 0, column 0)	|
| \033[K		| clear from the cursor position to the end of current line	|
| \033[s		| store current location	|
| \033[u		| recall the stored location|

### Examples

writing current time as the bar at the top:
```bash
PS1="\[\033[s\033[0;0H\033[0;41m\033[K\033[1;33m\t\033[0m\033[u\] <\u@\h \W>\$ "
```

### More info

+ [Bash prompt HOWTO](http://tldp.org/HOWTO/Bash-Prompt-HOWTO/)
+ [wiki ASNI escape codes](https://en.wikipedia.org/wiki/ANSI_escape_code)

----------

[back to contents](#contents)

## Keyboard tricks

Since wonderful bash is GNU project, learn damn emacs keystrokes

### Movement

+ `Ctrl-a` - move to the beginning of the line
+ `Ctrl-e` - end of line
+ `Ctrl-f` - forward one character
+ `Ctrl-b` - backward one character
+ `Alt-f` - forward one work
+ `Alt-b` - backward one word
+ `Ctrl-l` - clear the screen (same as command `clear`)

### Modify text

+ `Ctrl-d` - delete character
+ `Ctrl-t` - traspose (exchange) characters at the cursor location with the one preceding
+ `Alt-t` - traspose the word with the one preceding
+ `Alt-l` - convert lowercase from the location to the end of word
+ `Alt-u` - conver to uppercase

### Cutting and pasting text

+ `Ctrl-k` - kill text from location to the end of line
+ `Ctrl-u` - to the beginning of the line
+ `Alt-d` - to the end of the current word
+ `Alt-backspace` - to he beginning of the current word (if at the beginning, kills previous word)
+ `Alt-y` - yank text from the kill-ring

### Completion

+ `Alt-?` - display all possible completions (same as double tab)
+ `Alt-*` - insert all possible completions

### History

Bash keeps the history of the last 500 commands in `.bash_history`, history command retrieves just that
```bash
$ history | less
$ history | grep /usr/bin
```

+ `!<line_number>` - expands to command under that line
+ `Ctrl-p` - move to the previos history entry
+ `Ctrl-n` - next entry
+ `Alt-<` - move to the top of history list
+ `Alt->` - end of history list
+ `Ctrl-r` - reverse incremental search
+ `Alt-p` - reverse non-incremental search (type int the search string and press enter before the search is performed)
+ `Alt-n` - forward search non-incremental
+ `Ctrl-o` - execute the item in the history and advace to the next one (handy if you are going to re-execute sequence of commands)

### More info

* [bash rc not source automatically](https://apple.stackexchange.com/questions/12993/why-doesnt-bashrc-run-automatically)
* [basic customization](https://www.ostechnix.com/hide-modify-usernamelocalhost-part-terminal/)
* [more on customization](https://www.cyberciti.biz/tips/howto-linux-unix-bash-shell-setup-prompt.html)

# Later organized

Command line arguments are accessed with `$` sign and a number, f.e. first cl-argument is `$1`.

# Resources

+ [bash scripting tutorial](https://ryanstutorials.net/bash-scripting-tutorial/) - also other tutorials
+ [tutorial for beginners](https://linuxconfig.org/bash-scripting-tutorial-for-beginners)

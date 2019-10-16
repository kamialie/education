# bash, shell... 📟

[Bash home page](https://www.gnu.org/software/bash/)

[Brief documentaton](https://www.gnu.org/savannah-checkouts/gnu/bash/manual/bash.html)

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
* [Fun stuff](#fun-stuff)

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

### for
```bash
for name [ [in [words ...] ] ; ] do commands; done

for (( expr1 ; expr2 ; expr 3 )) ; do commands ; done
```

Expands *words* and execute *commands* once for each member in the resultant list.

----------

[back to contents](#contents)

### Conditional constructs

#### if
```bash
if test-command; then
    consequent-commands;
[elif more-test-commands; then
    more-consequent;]
[else alternate-consequents;]
fi
```
If clause evaluated when test-commands returns 0 (success status), otherwise elif, else clauses execute in turn.

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

----------

[back to contents](#contents)

### Brace

Spaces must be escaped if intendent between braces!

```bash
bash$ echo a{d,c,b}e
ade ace abe
```

Comma separeted value will be inserted(not sorted)

```bash
{x..y[..incr]}
bash$ echo a{1..4..1}e
a1e a2e a3e a4e
```

Expanded inclusive from x to y. When either x or y is preceded by 0, shell attempts to force all items to have the same length(feeding zeros).
* x, y - either integers or single characters
* incr - optional, an integer

----------

[back to contents](#contents)

### Tilde



----------

### Fun stuff

* \<esc\> + . - insert last argument of previous command

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

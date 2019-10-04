# bash, shell...

[Bash home page](https://www.gnu.org/software/bash/)
[Brief documentaton](https://www.gnu.org/savannah-checkouts/gnu/bash/manual/bash.html)

Bash metacharacters(must be quoted or escaped if not intendent to be used): space, tab, newline, '|', '&', ';', '(', ')', '<', '>'.

# Contents

* [Quoting](#quoting)
* [ ] [Variables](#variables)
* [Fun stuff](#fun-stuff)

### Quoting

'\' preserves the meaning of the next character except newline - it is treated as a line continuation. Single quotes preserve the literal meaning of each character within the quotes, cant escape single quotes with single quotes. Double quotes do the same except for '$', '`', '\', '!'(when history expansion is enabled). Backslashes that are followed without special meaning are left unmodified. Parameters '*', '@' have special meaning in double quoates.

ANSI-C quoting take the form *\$\'string\'*, for example \t, \n, \octal_number_for_unicode_symbol, etc

Interactive comments are on by default

### Variables

To access variable use '$' in front:
* 0 - name of the schell or shell script
* $ - ID of the shell
* ! - ID of the last executed background process(job, pipeline)
* ? - exit status of the last executed foreground pipeline
* _ - expands to the last argument of last command executed

### Fun stuff

* <esc> + . - insert last argument of previous command

### Links

* https://unix.stackexchange.com/questions/4782/how-to-pass-the-output-of-one-command-as-the-command-line-argument-to-another
* https://superuser.com/questions/110054/custom-vim-highlighting
* https://brettterpstra.com/2015/07/09/shell-tricks-inputrc-binding-fun/
* https://brettterpstra.com/2015/07/09/shell-tricks-inputrc-binding-fun/
* https://github.com/ycm-core/YouCompleteMe#mac-os-x-super-quick-installation
* https://realpython.com/vim-and-python-a-match-made-in-heaven/
* https://github.com/junegunn/vim-plug

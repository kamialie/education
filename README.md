# Lovely Vim

# Contents

+ [Hot keys](#hot-keys)
	- [File/buffer manipulaton:](#File/buffer-manipulaton:)
	- [Insert mode enter](#insert-mode-enter)
	- [Motion](#motion)
	- [Editing](#editing)
	- [Search and substitution](#search-and-substitution)
	- [Back to the future](#back-to-the-future)
	- [Labels and folders](#labels-and-folders)
	- [Macros](#macros)
	- [Fun stuff](#fun-stuff)
+ [Debugging](#debugging)
+ [Plugins](#plugins)

## Hot keys

+ <> tags refer to user input:
	- <number> - insert number consisting [0-9]
	- <symbol> - alphanumber symbol
+ [argument | argument] - or, choose one of the options enclosed in square brackets

### File/buffer manipulaton:

+ `[:w]` - save file
+ `[:q!]` - quit without saving changes
+ `[:w] <file_name>` - save current buffer as file_name
+ `[:e]` - edit specified buffer
+ `[:bn]` - go to next buffer
+ `[:bp]` - go to previous buffer
+ `[:vs]` - open new buffer vertically
+ `[:sp]` - open new buffer horizontally
+ `[Ctrl][w] [arrows | [hjkl] | w]` - move to another buffer
+ `[Ctrl][w] [Shift] [hjkl]` - move current buffer elsewhere
+ `[Ctrl][w] <number>  [+ | -]` - increase/decrease the size of the current buffer
+ `[:tabe] <file>` - open file in another tab (to open multiple files in tabs add -p flag)
	* `[gt]` - go to the next tab
	* `[gT]` - go to previous tab
	* `<i>[gt]` - go to ith tab
+ `[:tabm] <number>` - move current tab elsewhere
+ `[:tabs]` - list all files in tabs
+ `[:tabo]` - close all tabs except current
+ `[:tabc]` - close current tab
	* `<i>` - close ith tab
+ `[qa]` - close all buffers/tabs
+ `[:mks] <session_name>` - save a session with tabs for next login
	* vim -S session_name - open session
	* vim -p file1 file2 file3 - open 3 files at ones in tabs
+ `[:mks!] <session_name>` - save changes to session (if you added/deleted tabs)
+ `[:ls]` - list current buffers
+ `[:b]` - go to the next buffer
	* `<substring>` - unique filename substring will make it jump to that buffer


### Insert mode enter

Press while in normal mode:

+ `[i]` - under cursor
+ `[I]` - at the beginning of line
+ `[a]` - after cursor
+ `[A]` - at the end of line
+ `[o]` - new line under cursor
+ `[O]` - new line above cursor
+ `[s]` - delete character and insert
+ `[S]` - delete line and insert

### Motion

Press while in normal mode:

+ `[w]` - go to the beginning of next word
+ `[b]` - go to the beginning of previous word
+ `[e]` - go to the end of current(next) word
+ `[gg]` - go to start of file
+ `[G]` - go to end of file
+ `[Ctrl][e]` - scroll down
+ `[Ctrl][y]` -scroll up
+ `[Ctrl][d]` - jump down half screen
+ `[Ctrl][u]` - jump up half screen
+ `[Ctrl][f]` - jump down full screen
+ `[Ctrl][b]` - jump up full screen
+ `<number> [[gg] | [G]]` - go to the specified line
+ `[-]` - go the previous line (beginning)
+ `[+]` - go to the next line (beginning)
+ `[0]` - go to beginning of line
+ `[$]` - go to end of line
+ `[^]` - go to the first non blank character on the line
+ `[%]` - go to matching bracket, quotes
+ `[f]<symbol>` - go to specified symbol in the current line; then use `[;]` or `[,]` to more the next or previous one respectively
+ `[F]<symbol>` - same as f, but backwards
+ `[t]<symbol>` - same as f, but go to one symbol before specified
+ `[T]<symbol>` - same as t, but backwards
+ `[H]` - go to the top of the screen
+ `[M]` - go to the middle of file
+ `[L]` - go to the last line
+ `[z]`
	* `[z]` - center current line
	* `[t]` - make current line the top most
	* `[b]` - same as above but bottom most
+ `[Ctrl][o]` - go to the previous jump (be careful with what is considered to be a jump in vim - hjkl      are not jumps)
+ `[Ctrl][i]` - go to the next jump
+ `[K]` - find a reference to the function under cursor

### Editing

+ `[r] <letter>` - replace letter under cursor
+ `[R]` - enter replace mode, every typed character deletes existing one
+ `[v]` - enter visual mode
+ `[Ctrl][v]` - enter visual block mode
+ `[C]` - erase from cursor till end of line and enter insert mode
+ `[cc]` - erase current line and enter insert mode
+ `[ce]` - delete current word and go to insert mode
+ `[d]` - delete operator
	* `[w]` - until next word, excluding first character
	* `[e]` - until next word, including first character
	* `[i]` - inside; for example `di“` - delete everything inside quotes; works the same with `[c]`
		- `[p]` - paragraph
		- `[t]` - tags
		- `["]` - quotes

### Copy/paste/cut

+ `[[yy] | [Y]]` - copy current line
+ `[[dd] | [D]]` - cut current line
+ `[[y] | [d]]` - copy or cut
	* `[[j] | [arrow down]]` - current and line below
	* `[[k] | [arrow up]]` - current and line above
+ `[:]<start>[,]<end>[[y] | [d]]` - copy or cut line between lines start and end (included); you can also copy/cut till label - just create a label somewhere, then use `[y][`]<label>`
+ `[p]` - paste line under cursor
+ `[P]` - paste line before cursor
+ `[x]` - cut character under cursor
+ `[“]<letter>[command]` - specify the buffer for the command
	* `[\_]` - underscore, empty buffer
+ `<number>[operation]` - do operation number of times
+ `[.]` - repeat last operation in a whole
+ `[:read] <filename>` - yank and paste everything from specified file into current buffer
+ While in insert mode:
	* `[Ctrl][r]<register>` - insert text from specified register (register is any lowercase letter)
	* `[Ctrl][r][a]` - insert text from dot register (dot register holds last modification you made in insert mode
+ `[J]` - append line below to the current (physically move it)


### Search and substitution

+ `[*]` -  go to next occurence of the word under cursor; also highlights all occureneces \*
+ `[/]<text>` - got to the first character of first occurence of text in file starting from cursor location
	* `[\c]` - turn on ignore case search for just one search
	* `[n]` - go to next occurence
	* `[N]` - go to previous occurrence
+ `[?]<text>` - same as previous, but backwards
+ `[:][s][/]<old>[/]<new>` - substitute first occurrence of  ‘old’ by ‘new’ (type `[%]` instead of `[:]` upfront for entire file)
	* `[/]` - additional options, concatinate to command parent command (above); options below can be used separetely or together
		- `[g]` - all in current line
		- `[c]` - confirm before doing
		- `:s/i/o/gc` - substitute all occurences of `i` by `o` in the current line and ask confirmation before doing each substitution
+ `[:]<number>[,]<number>` - apply next operation in between lines specified
+ `[:noh]` - turn off highlighting


### Back to the future

+ `[u]` - undo last command
+ `[U]` - return current line to its original form
+ `[Ctrl][r]` - undo undos


### Labels and folders

Highlight region in vim, then `[[:][fold] | [zf]]` - fold lines

`fo` - is short for `fold`

+ `<number>[,]<number>[fo]` - same as previous, folder in between lines specified (toggle is same)
+ `[,][+]<number>[fo]` - fold current line and lines passed relative to it (below it)
+ [mb][%][zf’b] - fold lines !dont try this, needs checking!
+ `[za]` - open/close toggle
+ `[zo]` - open folding
+ `[zc]` - close folding
+ `[zd]` - delete folding
+ `[m][any_lowercase_character]` - create local label
+ `[m][any_uppercase_letter]` - create global label
+ ``[`][label]`` - go to the label created before
	* ``[`][`]`` - go to last jump


### Macros:

+ `[q]<letter>[command sequence][q]` - record macros
	* `[@]<letter>` - apply to the current cursor location
	* `[@@]` - rerun last macros
+ `[[Ctrl][N] | [Ctrl][P]]` - autocomplete
+ `[Ctrl][X] [Ctrl][N]` - autocomplete only in current file
+ `[Ctrl][X] [Ctrl][F]` - autocomplete for filenames


### Fun stuff:

+ `[[Operation]<count>[motion]]` - for example, [d2w] - deletes next two words
+ `[:!][shell_command]` - run shell command within vim
+ `[:make]` - trigger make within your vim!
+ `[Ctrl][R][=][expression]` - calculator in vim!!! Use it in insert mode - result will be printed in file

----------

## Debugging

Steps to get started

1. `:packadd termdebug` - load the plugin
2. `:Termdebug` - start debugging
3. `(gdb) file a.out` - load the program (alternatively pass a.out to the command above)

`(gdb) quit` - quit debugger

### Simple commads

+ `(gdb) next`, `:Over` - execute current line and stop at the next line
+ `(gdb) step`, `:Step` - execute current line and stop at the next statement, entering functions
+ `(gdb) finish`, `:Finish` - execute until leaving the current function
+ `(gdb) where`, `:Break` - show the stack
+ `(gdb) frame <N>` - go to Nth stack frame
+ `(gdb) continue`, `Continue` - continue execution
+ `CTRL-C`, `:Stop` - interrupt the program
+ `(gdb) print`, `:Evaluate` `:Ev` - evaluate expression under cursor

### Other commands

+ `:Gdb` - jump to gdb window
+ `:Program` - jump to program window
+ `:Source` - jump to source code window or create one
+ `:Break` - set a breakpoint at the current line
+ `:Clear` - delete a breakpoint at the current line

Create your own shortcuts that will work in any window using `TermDebugSendCommand()` function, which takes gdb command as argument:
```vim
map ,w :call TermDebugSendCommand('where')<CR>
```

[gdb reference](https://ftp.gnu.org/old-gnu/Manuals/gdb/html_chapter/gdb_3.html)

----------

## Plugins

## Interesting links
- https://medium.freecodecamp.org/learn-linux-vim-basic-features-19134461ab85
- http://vim.wikia.com/wiki/Mapping_keys_in_Vim\_-\_Tutorial\_(Part_2)
- http://learnvimscriptthehardway.stevelosh.com/chapters/07.html
- https://hashrocket.com/blog/posts/8-great-vim-mappings
- https://www.shortcutfoo.com/blog/top-50-vim-configuration-options/
- https://dougblack.io/words/a-good-vimrc.html
- https://stackoverflow.com/questions/736701/class-function-names-highlighting-in-vim
- https://stackoverflow.com/questions/29192124/how-to-color-function-call-in-vim-syntax-highlighting
- https://stackoverflow.com/questions/736701/class-function-names-highlighting-in-vim
- https://stackoverflow.com/questions/37777417/how-to-use-vim-key-bindings-with-visual-studio-code-vim-extension - exit key mapping in vscode for vim extension:
- http://vimdoc.sourceforge.net/htmldoc/options.html#'statusline', http://vimdoc.sourceforge.net/htmldoc/windows.html#status-line - status line


## Installation on systems

generete core files:
```shell
$> ulimit -c unlimited
```

then pass it to gdb:
```shell
$> gdb -c core program
```

gdb `backtrace` command show the call sequence back from the crashing (or current) point back to the top main call.

gdb `up` command - move up on the calling stack

gdb `list` command - show lines around specified

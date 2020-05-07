# Puppet

[language resources](https://puppet.com/docs/puppet/latest/lang_resources.html)

[design phylosophy](http://radar.oreilly.com/2015/04/the-puppet-design-philosophy.html)

# Contents

# Basics

Variables are preceded by `$`.

`fact` variable is a hash (dictionary) with information about the system.

Most resources are idempotent, meaning running them twice wont have any side effects.
However, `exec` resources is not, since it runs commands.
But you can add conditionals (like `onlyif` to prevent unnecessary execuion (check examples)

## Examples

```puppet
if $facts['is_virtual'] {
	package { 'smartmontools':
		ensure => purged,
	}
} else {
	package { 'smartmontools':
		ensure => installed,
	}
}
```

```puppet
exec { 'move example file':
	command => 'mv /home/user/text.txt /home/user/Desktop',
	onlyif => 'test -e /home/user/text.txt',
}
```

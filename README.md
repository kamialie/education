# Puppet

[language resources](https://puppet.com/docs/puppet/latest/lang_resources.html)

[design phylosophy](http://radar.oreilly.com/2015/04/the-puppet-design-philosophy.html)

[style guide](https://puppet.com/docs/puppet/latest/style_guide.html)

[puppet server installation](https://puppet.com/docs/puppetserver/latest/install_from_packages.html)

[check puppet manifest fits style-guide](http://puppet-lint.com/)

Catalog - list of rules that are generated for one specific computer once the server has evaluated all variables, conditionals, and functions.

Configuration file is called `manifest` and has `.pp` extension.

Resource types are written in lowercase letter, while in future reference they are capitalized.

# Contents

# Basics

[testing first module](https://rspec-puppet.com/tutorial/)

Variables are preceded by `$`.

`fact` variable is a hash (dictionary) with information about the system.

Most resources are idempotent, meaning running them twice wont have any side effects.
However, `exec` resources is not, since it runs commands.
But you can add conditionals (like `onlyif` to prevent unnecessary execuion (check examples)

`--noop` parameter in cl simulates the actual run and outputs what would have been applied.

## Examples

Ensure package exists on client:
```puppet
package { 'htop':
	ensure => present,
}
```

Simple conditional:
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

Run shell command with conditional:
```puppet
exec { 'move example file':
	command => 'mv /home/user/text.txt /home/user/Desktop',
	onlyif => 'test -e /home/user/text.txt',
}
```

Define and include a class in one file (normally class definitions are placed in separate file):
```puppet
class ntp {
	package { 'ntp':
		ensure => present,
	}
	file { '/etc/ntp.conf':
		source => '/home/user/ntp.conf',
		replace => true,
		require => Package['ntp'],
		notify => Service['ntp'],
	}
	service { 'ntp':
		enable => true,
		ensure => running,
		require => File['/etc/ntp.conf'],
	}
}

include ntp
```

# Modules

Module - collections of manifests and associated data. It usually contains `files` (files that wont change), `templates` (files that change depending on run of manifest) and `manifests` directories.

`init.pp` must be present in the module `manifests`, because its the first file read.

Include global module (use double colons):
```puppet
include ::apache
```

# Nodes

[puppet ssl explained](http://www.masterzen.fr/2010/11/14/puppet-ssl-explained/)

Node definitions are stored in manifest file `site.pp`, which is stored at the root of the nodes environment.

## Examples

Configuration for default nodes:
```puppet
node default {
	class { 'sudo': }
	class ntp { 'ntp':
		servers => ['server1.com', 'server2.com']
	}
}
```

Autosign requests from clients on server:
```shell
$> puppet config config --serction master set autosign true
```

Test run from client:
```shell
$> puppet agent -v --test
```

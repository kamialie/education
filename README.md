# Ansible

# Contents
	+ [Ansible ad-hoc command](#ansible-ad-hoc)
	+ [Ansible playbook](#ansible-playbok)
	+ [Ansible inventory](#ansible-inventory)
	+ [Help](#help)

# Ansible ad-hoc

Some modules support `--diff` and `--check` flags. `--diff` flags also shows the differences being made in `git diff` format. `--check` runs the command in dry mode, thus showing what will happen, if you actually run the command. Two flags together would output complete info on changes that will apply.

# Ansible playbook

[docs](https://docs.ansible.com/ansible/latest/user_guide/playbooks_intro.html)

run the playbook:
```shell
$> ansible-playbook playbook.yml
```

add more verbose output with `--verbose, -v` flag, repeat for more info.

## check later

`gather_facts: true/false` for play in playbook

# Ansible inventory


## Useful commands

+ list declared hosts:
	```shell
	$> ansible-inventory --list
	```
+ output in graph mode with groups and connections:
	```shell
	$> ansible-inventory --graph
	```
+ include variables as well:
	```shell
	$> ansible-inventory --graph --vars
	```


# Help

List contents of the available option of the plugin:

```shell
$> ansible-doc --type <plugin> --list
$> ansible-doc -t connections -l
```

[ansible modules](https://docs.ansible.com/ansible/latest/modules/modules_by_category.html)

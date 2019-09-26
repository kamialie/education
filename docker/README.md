**Docker**

https://github.com/docker/dockercraft
https://github.com/veggiemonk/awesome-docker

# docker-machine

...
Tool for provisioning and managing Dockerized hosts (hosts with Docker Engine on them), typically installed on local system, has its own command line client (docker-machine) and the Docker Engine client (docker)
Docker Engine is a client-server app made if the Docker daemon, REST API, CLI client that talks to the daemon (through REST API wrapper)

Can ssh into machine using "docker-machine ssh \<machine_name\>". To work as root either add sudo upfront or change to root either by "sudo -i" or "sudo su"

### Commands

Not specifying the name of machine defaults to the name 'default'

docker-machine
* ls - list available machines
* create \<name\> - create a docker machine
    + [--driver], [-d] \<name\> - set a driver; full list of avaliable drivers at https://docs.docker.com/machine/drivers/
    + [--driver] \<name\> --help - list available options to create a machine
* ip \<name\> - get machine's ip address
* env \<name\> - display commands to set up environment for docker (to run docker commands against particular machine)
    + [-u] - display commands to unset environemnt variabes
* start \<name\> - start machine
* stop \<name\> - stop machine
* restart \<name\> - restart machine
* upgrade \<name\> - upgrade machine to latest version of Docker(depends on underlying distribution on host); for example if uses Ubuntu, command is similar to sudo apt-get upgrade docker-engine
* ssh \<machine_name\> \<command\>- login or run a command on a machine using SSH, if no command specified logs in like regular ssh
    + [--native-ssh] - use Go ssh implementation instead of local one; docker by default tries to find local ssh and uses that it does find it

### Links


# docker

...

### Commands

docker [command] --help - brings options list

docker
* login - login to dockerhub to push and pull images
* tag \<image_name\> \<username\/repository:tag\> - give a new tag to image(prepare to push to remository), new tag is created and referenced the parent image
* ps - list containers (default to currently running containers)
    + [--all], [-a] - show all
    + [--quiet], [-q] - display only numeric IDs
    + [--size], [-s] - display total file sizes
* images
* run - run a command in a new container
    + [--rm] - automatically remove container when it exits
    + [--tty], [-t] - allocate pseudo-tty
    + [--interactive], [-i] - keep stdin open even if not attached
    + [--detach], [-d] - run container in a background
    + [--publish], [-p] \<host_port\>:\<container_port\> - publish ports from host to container
    + [--volume], [-v] - https://docs.docker.com/storage/bind-mounts/
    + [--mount] - https://docs.docker.com/storage/bind-mounts/
    + https://stackoverflow.com/questions/35550694/what-is-the-purpose-of-the-i-and-t-options-for-the-docker-exec-command/35551071#35551071 - on TTY
* exec
* attach
* start
* pull - pull(download) an image from a registry, can be used to update an image as well
* build .
    + [--tag], [-t] \<name\>:\<tag\> - tag resulting image

### Links

------------

# docker swarm and docker node

...
A swarm is a group of machines that are running Docker and joined into a cluster. Docker commands are now executed on a whole cluster by a swarm manager.
Machines can be physical or virtual, referred as nodes in swarm.
Always run docker swarm init and join with port 2377 or leave it blank for default(swarm manager port)

### Commands

docker swarm
* init - initialize swarm, targeted docker engine becomes a manager; generates two random tokens - manager and worker
    + [--advertise-addr] - addvertise address
* leave - leave a swarm, works without warning for workers
    [--force] - use it on manager, when swarm wont be used anymore; proper way is to demote manager to worker, then leave the swarm without warnings

docker node
* ls - list all the nodes that the Docker Swarm manager knows about(used on manager node)
* ps \<node_name\> - list tasks running on one or more nodes, default to current node
    + $(docker ls -q) - list all tasks in a swarm(better use docker stack ps)

### Links

https://docs.docker.com/engine/reference/commandline/swarm_init/
https://www.youtube.com/watch?v=bU2NNFJ-UXA

------------

# docker stack

...
Single container running in a service is called a _task_.

### Commands

docker stack
* deploy \<stack_name\> - deploy a new stack or update an existing one(no need to shut down, just apply changes and run command again)
    + [--compose-file] [-c] \<path_to_docker-compose.yml\> - path to docker-compose.yml that is docker swarm is used
* ps \<stack_name\> - view all tasks of a stack
* ls - lists the stacks
* rm \<stack_name\> - removes the stack from the swarm(has to be run targeting a manager node) that is services, netwerks and secret associations

### Links


------------

# docker service

...
Service can be viewed as a separate part of the app, in docker sense - separate container(piece of software). Service can be scaled through replicas - number of containers running that(same) piece of software.
Load-balancing is done through round-robin fashion(after last one comes first)

### Commands

docker service
* ls - lists services running in the swarm(has to be run targeting a manager node)
* ps \<service\> - list the tasks of one or more services(has to be run targeting a manager node)

### Links


------------

# docker-compose

### Commands

docker-compose
* config - validate and view the Compose file
* up - build, recreate, start and attach to container
    + [--detach], [-d] - set a driver; full list of avaliable drivers at https://docs.docker.com/machine/drivers/
    + [--build] - build images before starting(force to even if they exist)
* down - stops and removes containers, networks, volumes and images createad by up, external networks and volumes are not removed
* exec - equivalent of docker exec, allows to run commands in services (by default allocates TTY, example, docker-compose exec web sh)
* kill - forces conainers to stop by sending SIGKILL, optinally other signals can be sent
* logs - display logs output from services
    + [--folow], [-f] - follow output
* pause - pauses running containers of the service, can be unpaused by docker-compose unpause
* ps - list containers
* restart - restart all stopped and running services, for changes in Compose file use restart_policy
* rm - remove stopped service containers
* scale - sets the number of containers to run for a service; alternatively in Compose file 3. can specify replicas under deploy key, deploy key only works woth docker stack deploy command
* top - list runnning processes

### Links


------------

# Dockerfiles

For running option look for [docker build] command above. Valid Dockerfile must start with a FROM instuction.

When Docker builds an image, each instruction represents a read-only layer, where each one is a delta of the changes from previos one. When you run an image, new writable later(container layer) is added on top of underlying layers(all changes are written to this container layer - creating, deleting, modifiying files), more on that https://docs.docker.com/storage/storagedriver/

Build context is the directory with all recursive contents where _docker build_ was issued, it is sent to Docker daemon as a whole and affect build time and final size of the image.

Exlude not relevant files with .dockerignore (works similar to .gitignore, more at https://docs.docker.com/engine/reference/builder/#dockerignore-file)

Docker offers a reserved, miminal image, _scratch_, as starting point for building images. It signals to the build process that the next command ti be the first filesystem layer in the final image. Can copy executable and just run it(https://docs.docker.com/develop/develop-images/baseimages/):

	FROM scratch
	ADD hello /
	CMD ["/hello"]

[not finished]Minimize image size:
- Only **RUN**, **COPY**, **ADD** create layers, rest create temporary intermediate images.
- sort multi-line arguments alphanumerically, add space before backslash

		RUN apt-get update && apt-get install -y \
		  bzr \
		  cvs \
		  git \
		  mercurial \
		  subversion	
- decouple applications - create separate images for database, web server, etc

Building ruby on rails docker image https://medium.com/@lemuelbarango/ruby-on-rails-smaller-docker-images-bff240931332, https://ncona.com/2017/09/getting-rails-to-run-in-an-alpine-container/

Building gitlab image manually https://docs.gitlab.com/ee/install/installation.html, https://about.gitlab.com/install/#debian, https://packages.gitlab.com/gitlab/gitlab-ce/install
Problems:
    - https://gitlab.com/gitlab-org/omnibus-gitlab/issues/4257 - configuration freezes on ruby-block
    - https://gitlab.com/gitlab-org/omnibus-gitlab/issues/430
    - https://serveradmin.ru/ustanovka-gitlab-v-lxc-konteyner/
    - https://docs.gitlab.com/omnibus/common_installation_problems/
Further configuration - https://docs.gitlab.com/omnibus/README.html#installation-and-configuration-using-omnibus-package

### Instructions

#### FROM
- **FROM** _\<image\>_ [**AS** _\<name\>_]
- **FROM** _\<image\>:\<tag\>_ [**AS** _\<name\>_]

Initializes new build stage and sets the base image; **FROM** supports variables that are declared by any **ARG** instruction that occur before first **FROM**, however everything before **FROM** is outside build and wont be accessible, use following trick to handle it(use **ARG** without a value inside of a build stage):

        ARG VERSION=latest
        FROM busybox:$VERSION
        ARG VERSION
        RUN echo $VERSION > image_version

Optionally a name can be given to a new build stage by adding **AS** _name_, then it can be used in subsequent **FROM** **COPY** _--from=\<name\>|index\>_ 

Alpine is the recommended base image - currently under 5MB

#### RUN
- **RUN** _\<command\>_ (shell form) - defaults to _/bin/sh -c_ on Linux; can use \ to break line
- **RUN** [_"exec", "param1", "param2"_](exec form) - parsed as JSON array, that is must use double-quotes; doesn't invoke command shell, so **RUN** ["echo", "$HOME"] wont be substituted, run with shell or use shell form

Execute commands in a new layer on top of of the current image; default shell can be changed using **SHELL** or use exec form; cache for **RUN** can be used during next build - can be invalidated using _--no-cache_ flag on build

Avoid using apt-get upgrade and dist-upgrade, as many essential packages from parent images can not upgrade inside an uprivileged container, rather update and install packages you need (keep it in one command to avoid caching issues, if command changes in the future):

	RUN apt-get update && apt-get install -y \
	    package-one \
	    package-two 


#### CMD
- **CMD** [_"exec", "param1", "param2"_] (exec form)
- **CMD** [_"param1", "param2"_] (as default params to **ENTRYPOINT**)
- **CMD** command param1 param2 (shell form) - will be executed in /bin/sh -c

Provide defaults for an executing container, can include an executable or can omit exec, but specify **ENTRYPOINT** as well; if **CMD** is used to provide defaults to **ENTRYPOINT**, both should be with the JSON array format; can be only one **CMD** in Dockerfile; doesnt execute anything at build time; if arguments are specified on docker run, then they will override the default in **CMD**

Should almost always be used in exec form (like **CMD** ["apache2", "-DFOREGROUND"]), in most other cases should be given an interactive shell, such as bash, python and perl, so that docker run -it python would will get user dropped into usable shell. In rare cases should be used as params to **ENTRYPOINT**

#### ENTRYPOINT
- **ENTRYPOINT** [_"exec", "param1", "param2"_](exec form, preferred)
- **ENTRYPOINT** command param1 param2 (shell form)

Configure container that will run as exec; both **CMD** and **ENTRYPOINT** define what commands executed when running a container; command line arguments to docker run \<image\> will be appended after all elements in an exec form **ENTRYPOINT** and override all **CMD** elements; only last **ENTRYPOINT** will have an effect

Can be used in combination with a helper script, allowing it to work just like in combination with **CMD** - having default and particular behaviour. Script uses the _exec_ Bash command so that the final running application becomes the container's PID 1, allowing to receive any Unix signals

	#!/bin/bash
	set -e
	if [ "$1" = 'postgres' ]; then
	    chown -R postgres "$PGDATA"
	    if [ -z "$(ls -A "$PGDATA")" ]; then
		gosu postgres initdb
	    fi
	    exec gosu postgres "$@"
	fi
	exec "$@"

#### EXPOSE
- **EXPOSE** _\<port\>_ [_<port>/protocol>..._] 

Informs Docker that the container listens on the specified network port at runtime(doesn't actually publish them, use -p flag of docker run to publish or -P to publish all exposed port randomly), defaults to TCP(can specify UDP)

#### ADD
- **ADD** _\<src>... \<dest\>_
- **ADD** ["_\<src>_",... "_\<dest\>_"]

Copies new files, directories or remote files from \<src\> and adds them to the filesystem of the image at the path \<dest\>;

* _\<dest\>_ can be absolute or relative to **WORKDIR**
* doesn't support authentication for URL, will need to use **RUN** _wget_, **RUN** _curl_ instead
* _\<src\>_ must be inside context of build 
* if _\<src\>_ is URL and _\<dest\>_ doesn't end with slash - file in downloaded and copied to _\<dest\>_, otherwise dowloaded to _\<dest\>/\<filename\>_(URL must have a nontrivial path)
* if _\<src\>_ is is local tar, then it's unpacked as a directory, resources from remote URLs are not decomplressed
* if multiple _\<src\>_ are specified, then _\<dest\>_ must be a directory and it must end with a slash /
* if _\<dest\>_ doesn't end with a slash, it will be considered a regular file and contents of _\<src\>_ wil be written at _\<dest\>_
* if _\<dest\>_ doesn't exist, it is created along with all missing directories

For best image size results prefer curl or wget over **ADD** (can later delete unnecessary files and avoid adding extra layer)

#### COPY
- **COPY** _\<src>... \<dest\>_
- **COPY** ["_\<src>_",... "_\<dest\>_"]

Copies new files or directories from \<src\> and adds them to the filesystem at the path \<dest\>, same rules as **ADD** apply to **COPY**

**COPY** is generally preferred over **ADD** as it is more transparent and provides basic functionality. **ADD** best usage is for local tar extraction. In case of multiple Dockerfile steps, **COPY** them individually - results in fewer cache invalidations, as each **COPY** is on seperate layer.

#### ENV
- **ENV** _\<key\>_ _\<value\>_ - sets a single variable to a value, entire string after the first space(after key) will be treated as the \<value\>
- **ENV** _\<key\>_=_\<value\>_ ... - allows for multiple variable to be set(quotes and backslashes can be used to include white-spaces)

Sets the environment variable \<key\> to the value \<value\>, this value will be in the environment for all subsequent instructions in the build stage; variables set using **ENV** will persist when container is run from resulting image(can view them using docker inspect)

To really unset **ENV** variable set, use and unset them on the same layer:

	FROM alpine
	RUN export ADMIN_USER="mark" \
	    && echo $ADMIN_USER > ./mark \
	    && unset ADMIN_USER
	CMD sh

**ENV** and **ARG** usage - https://vsupalov.com/docker-arg-env-variable-guide/#arg-and-env-availability

#### ARG
- **ARG** _\<name\>[=\<default\>]_ 

Defines a variable that users can pass at build-time to the builder using _docker build --build-arg \<varname\>=\<value\>_(can be viewed by any user with docker hitory command - therefore not recommended for passwords, etc); can optionally include a default value; comes into effect from the line on which it was defined and ends at the end of the build stage - out of scope results in empty string; **ENV** always overried **ARG**; predefined ARGS (excluded from _docker history_ and is not cached) - HTTP_PROXY HTTP_proxy HTTPS_PROXY http_proxy FTP_PROXY ftp_proxy NO_PROXY np_proxy; "cache miss" occured upon first usage, not definition(if value has changed)

#### WORKDIR
- **WORKDIR** /path/to/workdir

Sets the working directory for any **RUN**, **CMD**, **ENTRYPOINT**, **COPY** and **ADD** instructions, if doesnt exist it is created; can resolve previously set **ENV** variables

#### USER
- **USER** \<user\>[\<group\>
- **USER** \<UID\>[\<GID\>]

Sets the user name (or UID) to use when running the image and any **RUN**, **CMD**, **ENTRYPOINT** instructions that follows. Avoid installing or using sudo as it has unpredictable TTY and signal-forwarding behavior. For similar to sudo behavior consider "gosu" (https://github.com/tianon/gosu).

### Links

- https://docs.docker.com/engine/reference/builder/
- https://kapeli.com/cheat_sheets/Dockerfile.docset/Contents/Resources/Documents/index

------------


# docker-compose.yml file

docker-compose [-f] \<path\> - specify path to docker-compose.yml file; can specify two, the later is applied over and in addition to previuos files; if nothing is specified docker is looking for docker-compose.yml and docker-compose.overried.yml - must supply at least the first; followed by '-' instructs to read from stdin

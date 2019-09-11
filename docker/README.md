**Docker**


# docker-machine

...


### Commands

Not specifying the name of machine defaults to the name 'default'

docker-machine
* ls - list available machines
* create \<name\> - create a docker machine
    + [--driver], [-d] \<name\> - set a driver; full list of avaliable drivers at https://docs.docker.com/machine/drivers/
    + [--driver] \<name\> --help - list available options to create a machine
* ip \<name\> - get machine's ip address
* env \<name\> - display commands to set up environment for docker (to run docker commands against particular machine)
* start \<name\> - start machine
* stop \<name\> - stop machine
* restart \<name\> - restart machine
* upgrade \<name\> - upgrade machine to latest version of Docker(depends on underlying distribution on host); for example if uses Ubuntu, command is similar to sudo apt-get upgrade docker-engine

### Links


# docker

...

### Commands

docker [command] --help - brings options list

docker
* ps - list containers (default to currently running containers)
    + [--all], [-a] - show all
    + [--quiet], [-q] - display only numeric IDs
    + [--size], [-s] - display total file sizes
* images
* run
* exec
* attach
* start

### Links



# docker swarm

...

### Commands


### Links


------------

# Dockerfiles

For running option look for [docker build] command above. Valid Dockerfile must start with a FROM instuction.

### Instructions

- **FROM** _\<image\>_ or **FROM** _\<image\>:\<tag\>_ - initializes new build stage and sets the base image; **FROM** supports variables that are declared by any **ARG** instruction that occur before first **FROM**, however everything before **FROM** is outside build and wont be accessible, use following trick to handle it(use **ARG** without a value inside of a build stage):

        ARG VERSION=latest
        FROM busybox:$VERSION
        ARG VERSION
        RUN echo $VERSION > image_version

- **RUN** - execute commands in a new layer on top of of the current image; default shell can be changed using **SHELL** or use exec form; cache for **RUN** can be used during next build - can be invalidated using _--no-cache_ flag on build
    + **RUN** _\<command\>_ (shell form) - defaults to _/bin/sh -c_ on Linux; can use \ to break line
    + **RUN** _["exec", "param1", "param2"]_(exec form) - parsed as JSON array, that is must use double-quotes; doesn't invoke command shell, so **RUN** ["echo", "$HOME"] wont be substituted, run with shell or use shell form
- **CMD** - provide defaults for an executing container, can include an executable or can omit exec, but specify **ENTRYPOINT** as well; if **CMD** is used to provide defaults to **ENTRYPOINT**, both should be with the JSON array format; can be only one **CMD** in Dockerfile; doesnt execute anything at build time; if arguments are specified on docker run, then they will override the default in **CMD**
    + **CMD** _["exec", "param1", "param2"]_ (exec form)
    + **CMD** _["param1", "param2"]_ (as defaults params to **ENTRYPOINT**)
    + **CMD** command param1 param2 (shell form) - will be executed in /bin/sh -c
- **ENTRYPOINT** - configure container that will run as exec; both **CMD** and **ENTRYPOINT** define what commands executed when running a container; command line arguments to docker run \<image\> will be appended after all elements in an exec form **ENTRYPOINT** and override all **CMD** elements; only last **ENTRYPOINT** will have an effect
    + **ENTRYPOINT** _["exec", "param1", "param2"]_(exec form, preferred)
    + **ENTRYPOINT** command param1 param2 (shell form)
- **EXPOSE** _\<port\> [<port>/protocol>...]_ - informs Docker that the container listens on the specified network port at runtime(doesn't actually publish them, use -p flag of docker run to publish or -P to publish all exposed port randomly), defaults to TCP(can specify UDP)
- **ADD** - copies new files, directories or remote files from \<src\> and adds them to the filesystem of the image at the path \<dest\>;
    + **ADD** _\<src>... \<dest\>_
    + **ADD** ["_\<src>_"... "_\<dest\>_"]
    + rules:
	    * _\<dest\>_ can be absolute or relative to **WORKDIR**
	    * doesn't support authentication for URL, will need to use **RUN** _wget_, **RUN** _curl_ instead
	    * _\<src\>_ must be inside context of build 
	    * if _\<src\>_ is URL and _\<dest\>_ doesn't end with slash - file in downloaded and copied to _\<dest\>_, otherwise dowloaded to _\<dest\>/\<filename\>_(URL must have a nontrivial path)
	    * if _\<src\>_ is is local tar, then it's unpacked as a directory, resources from remote URLs are not decomplressed
	    * if multiple _\<src\>_ are specified, then _\<dest\>_ must be a directory and it must end with a slash /
	    * if _\<dest\>_ doesn't end with a slash, it will be considered a regular file and contents of _\<src\>_ wil be written at _\<dest\>_
	    * if _\<dest\>_ doesn't exist, it is created along with all missing directories

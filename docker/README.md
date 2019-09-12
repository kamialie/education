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

When Docker builds an image, each instruction represents a read-only layer, where each one is a delta of the changes from previos one. When you run an image, new writable later(container layer) is added on top of underlying layers(all changes are written to this container layer - creating, deleting, modifiying files), more on that https://docs.docker.com/storage/storagedriver/

Build context is the directory with all recursive contents where _docker build_ was issued, it is sent to Docker daemon as a whole and affect build time and final size of the image.

Exlude not relevant files with .dockerignore (works similar to .gitignore, more at https://docs.docker.com/engine/reference/builder/#dockerignore-file)

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


### Instructions

##### FROM
- **FROM** _\<image\>_ [**AS** _\<name\>_]
- **FROM** _\<image\>:\<tag\>_ [**AS** _\<name\>_]

Initializes new build stage and sets the base image; **FROM** supports variables that are declared by any **ARG** instruction that occur before first **FROM**, however everything before **FROM** is outside build and wont be accessible, use following trick to handle it(use **ARG** without a value inside of a build stage):

        ARG VERSION=latest
        FROM busybox:$VERSION
        ARG VERSION
        RUN echo $VERSION > image_version

Optionally a name can be given to a new build stage by adding **AS** _name_, then it can be used in subsequent **FROM** **COPY** _--from=\<name\>|index\>_ 

Alpine is the recommended base image - currently under 5MB

##### RUN
- **RUN** _\<command\>_ (shell form) - defaults to _/bin/sh -c_ on Linux; can use \ to break line
- **RUN** [_"exec", "param1", "param2"_](exec form) - parsed as JSON array, that is must use double-quotes; doesn't invoke command shell, so **RUN** ["echo", "$HOME"] wont be substituted, run with shell or use shell form

Execute commands in a new layer on top of of the current image; default shell can be changed using **SHELL** or use exec form; cache for **RUN** can be used during next build - can be invalidated using _--no-cache_ flag on build



##### CMD
- **CMD** [_"exec", "param1", "param2"_] (exec form)
- **CMD** [_"param1", "param2"_] (as default params to **ENTRYPOINT**)
- **CMD** command param1 param2 (shell form) - will be executed in /bin/sh -c

Provide defaults for an executing container, can include an executable or can omit exec, but specify **ENTRYPOINT** as well; if **CMD** is used to provide defaults to **ENTRYPOINT**, both should be with the JSON array format; can be only one **CMD** in Dockerfile; doesnt execute anything at build time; if arguments are specified on docker run, then they will override the default in **CMD**

##### ENTRYPOINT
- **ENTRYPOINT** [_"exec", "param1", "param2"_](exec form, preferred)
- **ENTRYPOINT** command param1 param2 (shell form)

Configure container that will run as exec; both **CMD** and **ENTRYPOINT** define what commands executed when running a container; command line arguments to docker run \<image\> will be appended after all elements in an exec form **ENTRYPOINT** and override all **CMD** elements; only last **ENTRYPOINT** will have an effect

##### EXPOSE
- **EXPOSE** _\<port\>_ [_<port>/protocol>..._] 

Informs Docker that the container listens on the specified network port at runtime(doesn't actually publish them, use -p flag of docker run to publish or -P to publish all exposed port randomly), defaults to TCP(can specify UDP)

##### ADD
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

##### COPY
- **COPY** _\<src>... \<dest\>_
- **COPY** ["_\<src>_",... "_\<dest\>_"]

Copies new files or directories from \<src\> and adds them to the filesystem at the path \<dest\>, same rules as **ADD** apply to **COPY**

##### ENV
- **ENV** _\<key\>_ _\<value\>_ - sets a single variable to a value, entire string after the first space(after key) will be treated as the \<value\>
- **ENV** _\<key\>_=_\<value\>_ ... - allows for multiple variable to be set(quotes and backslashes can be used to include white-spaces)

Sets the environment variable \<key\> to the value \<value\>, this value will be in the environment for all subsequent instructions in the build stage; variables set using **ENV** will persist when container is run from resulting image(can view them using docker inspect)

##### ARG
- **ARG** _\<name\>[=\<default\>]_ 

Defines a variable that users can pass at build-time to the builder using _docker build --build-arg \<varname\>=\<value\>_(can be viewed by any user with docker hitory command - therefore not recommended for passwords, etc); can optionally include a default value; comes into effect from the line on which it was defined and ends at the end of the build stage - out of scope results in empty string; **ENV** always overried **ARG**; predefined ARGS (excluded from _docker history_ and is not cached) - HTTP_PROXY HTTP_proxy HTTPS_PROXY http_proxy FTP_PROXY ftp_proxy NO_PROXY np_proxy; "cache miss" occured upon first usage, not definition(if value has changed)

##### WORKDIR
- **WORKDIR** /path/to/workdir

Sets the working directory for any **RUN**, **CMD**, **ENTRYPOINT**, **COPY** and **ADD** instructions, if doesnt exist it is created; can resolve previously set **ENV** variables

##### USER
- **USER** \<user\>[\<group\>
- **USER** \<UID\>[\<GID\>]

Sets the user name (or UID) to use when running the image and any **RUN**, **CMD**, **ENTRYPOINT** instructions that follows

### Links

- https://docs.docker.com/engine/reference/builder/
- https://kapeli.com/cheat_sheets/Dockerfile.docset/Contents/Resources/Documents/index

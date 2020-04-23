# Docker :whale: :package:

Go further:
* [follow up on learning on Docker official website](https://docs.docker.com/get-started/part6) (links at the ends)
* [Awesome docker](https://github.com/veggiemonk/awesome-docker)
* [Docker craft](https://runnable.com/docker/basic-docker-networking)
* [Docker github documentation](https://github.com/docker/docker.github.io)
* [Kubernetes](https://www.docker.com/products/kubernetes)

# Contents

* [Networking in docker](#networking-in-docker)
    - [x] [bridge](#bridge)
    - [ ] [overlay](#overlay)
    - [x] [host](#host)
    - [ ] [macvlan](#macvlan)
    - [ ] [docker network command](#docker-network)
* [ ] [Storage in docker](#storage-in-docker)
	- Storage types
		+ [Volumes](#volumes)
		+ [Bind mounts](#bind-mounts)
		+ [tmpfs](#tmpfs)
	- [docker volume command](#docker-volume)
* [docker-machine](#docker-machine)
* [docker cli](#docker-cli)
    - [ ] [docker stack](#docker-stack)
    - [x] [docker service](#docker-service)
    - [ ] [docker swarm, docker node](#docker-swarm-and-docker-node)
* [Dockerfiles](#dockerfiles)
    - [x] [FROM](#from)
    - [x] [RUN](#run)
    - [x] [CMD](#cmd)
    - [x] [ENTRYPOINT](#entrypoint)
    - [x] [USER](#user)
    - [x] [ADD](#add)
    - [x] [COPY](#copy)
    - [x] [ENV](#env)
    - [x] [ARG](#arg)
    - [x] [WORKDIR](#workdir)
    - [x] [USER](#user)
    - [ ] [VOLUME](#volume)
    - [ ] [ONBUILD](#onbuild)
    - [ ] [STOPSIGNAL](#stopsignal)
    - [ ] [HEALTHCHECK](#healthcheck)
    - [ ] [SHELL](#shell)
    - [ ] [LABEL](#label)
* [Docker compose](#docker-compose)
    - [ ] [docker-compose tool](#docker-compose-tool)
    - [ ] [volumes](#volumes)

# Docker?

Docker containers are very much like virtual machines, but the main difference is that docker only isolates applications, meaning all of the containers on the server run on the same server's operation system. That, in result, requires fewer resource that virtual machines (low-level operating system tasks). In an example of virtual machine application scaling these tasks would be duplicated. On macOS and Windows Docker requires Linux virtual machine to run containers, since docker directly communicates with Linux kernel.

Docker images are templates that are used to create containers. One image can be build on top of another. Repository images can be tagged to specify a version (separete the name from a tag using a colon, e.g. alpine:3.4, default is latest).

Installing docker on Linux:
* prepare package manager and prerequisite packages:
```bash
apt-get -y install apt-transport-https ca-certificates curl
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | apt-key add -
add-apt-repository \
    "deb [arch=amd64] https://download.docker.com/linux/ubuntu \ $(lsb_release -cs) stable"
sudo apt-get update
```
* install docker:
```bash
apt-get -y install docker-ce
```
* add new user to avoid root access:
```bash
sudo groupadd docker
sudo usermode -aG docker $USER
```

## Networking in docker

[to be continued]
[official docs](#https://docs.docker.com/network/)
https://runnable.com/docker/basic-docker-networking
[multi-host networking](#https://docs.docker.com/network/overlay-standalone.swarm/)
[docker swarm reference architecture](#https://success.docker.com/article/networking)

[History note] Docker Inc. acquired SocketPlane from Docker 1.12, which completely rearchitectured their networking model.

Three pillars:

1. Container network model (CNM) - design
2. Libnetwork - docker's implementation (logic, API, UX...)
3. drivers - actual implementations of networks; each network type has its own driver; docker has a notion of *local* (native, build-in) and *remote* (3rd party) drivers; *local* include bridge, overlay, MACVLAN, IPVLAN, host, none.

[reference on CNM](https://github.com/moby/libnetwork/blob/master/docs/design.md)

Competing CNI (Container Network Interface) is used by Kubernetes (CoreOS Inc.)

Major parts of CNM:

+ Sandbox (aka namespace in Linux world) - isolated area of OS, contains full network stack (IP confgigs, DNS, routing table, etc)
+ Endpoint (aka network interface, eth0)
+ Network - from docker's perspective, multiple endpoint talking to each other

`docker info` - is a basic command that gathers all info about current docker state; here, in plugins section, you can find available networks drivers

Docker's networking subsystem is pluggable, using drivers.

Good example of creating couple user-defined networks for an application: couple containers running the application, one running the database. All containers are connected to the backend network, while application containers are also connected to front-end, where load-balancer will also be connected (its single port, which is exposed to the host will map requests to one of the application containers)

### bridge

Docker bridge driver is automatically installed on host machine(can provide isolation from containers on other networks). Bridge networks only apply to containers running on the same Docker daemon host. Default bridge networks is created upon Docker startup, newly created containers automatically connect to it unless otherwise specified (user-defined networks are superior to the default one)

Default bridge network is called *bridge*(docker0 on host), used to connect containers on a single host. *Host* and *none* arent fully-fledged and only used to start a container connected directly to the Docker daemon host's networking stack or to start a container with no network devices (thus completely isolate container).

Bridge network is creatied via `802.1d bridge` device; it is also called sirtual switch or vswitch. Actually uses Linux's kernel, which makes very fast and durable.

Differences between default and user-defined:
* containers on user-defined network automatically expose all ports to each other, and no ports to outside world; on default network user needs to manually open ports for two containers to communicate, thus close unnecessary ports for outside world by other means
* user-defined bridges provide automatic DNS resolution between containers; default requires IP, legacy --link option or manipulations in each containers /etc/hosts
* can easaly configure new network; default requires restart or Docker
* sharing environment variables can be done using docker-compose or docker swarm service(using secrets and configs)

Container connect to the bridge network (switch) via eth0 adapter, which is shown in `brctl show` command as interfaces. Container can connect to more than one bridge network.

For extra inspection use `bridge-utils` package (on Ubuntu) to get even more info on bridge network; command `brctl show` will output all bridge networks on host.

### host

[to be continued](#https://docs.docker.com/network/host/)
The container shares the host's networking namespace (directly reachable from host IP), and doesn't get its own IP. Can be used for optimization purposes, when there is a need to handle large range of ports (no NAT translation). Only works on Linux hosts.
```bash
docker run --rm -d --network host --name my_nginx nginx
```
### overlay

[to be continued]
The overlay network creates a distributed network among multiple Docker daemon hosts.

When swarm is initialized or join Docker hosts to an existing swarm, two networks are created on the host:
1. overlay network called *ingres*, which handles control and data traffic related to swarm services; swarm service connects to it by default if not user-defined is specified
2. bridge network called *docker_gwbridge*, which connects the individual Docker daemon to the other daemons participating in the swarm; it is a virtual bridge that connects the overlay networks(including ingress) to an individual Docker daemon's physical network, so that traffic can flow to and from swarm managers and workers; exists in the kernel of a Docker host.

When creating a overlay network for services on manager, docker automatically adds them on worker node when creating a service and removes them when removing a service (that is it initially gets created on manager and is scoped to swarm, but actually appears on workers upon need).

User-defined overlay network also provides DNS translation in contrast to default ingress network.

Standalone containers can also join overlay network, if it was created with *attachable* flag:
```bash
docker network create -d overlay --attachable test-net
docker run -dti --network test-net alpine
```

#### behind the scenes

Each node (host) has its own sandbox with a bridge network. For overlay network, each of these sandboxes get a VXLAN tunnel endpoints that are plumed into bridge. Then, the communication is done through this tunnel (effectively the overlay network; also is a single layer2 broadcast domain; layer2 adjencency), so router doesn't participate in making decisions, rather VXLAN encapsulation takes care of all that.

Underlay network (through router) must be connected.

Ports that should be open: 4789(UDP), 7946(TCP/UPD), 2377(TCP).

### macvlan

Linux specific driver. Uses Linux subinterfaces and naming conventions that come with that. For example, with a parent eth0 interface, after creating new macvlan connecting to vlan 100, it creates a eth0.100 subinterface. **Require Promiscuous mode**.

Suitable for the case, when you want to connect containers to the existing VLAN with vms and apps, since bridge and overlay are more contaner-focused. Contanerizing parts of business application includes starting and connecting docker hosts to the existing network. Containers inside these hosts connect to the app's network through their host, but overall picture enables us to avoid hosts and see containers directly connected to the app's network.

When creating macvlan network it needs the subnet information, default gateway and a name, thus knowing where to connect containers on the outside network, that is which vlan. It also needs to know a range of addresses that are not already in use on vlan to avoid duplication (as it is words right now).

For strict isolation Linux kernel filters and drops all packets between container and the host it is running on.

### ipvlan

Almost same as `macvlan`, but more cloud friendly (doesn't require special mode) and does not assign each participant its own MAC address (actually assignes parents MAC address). All other rules apply here as well.

### docker network command

* **create**
    + [--attachable] - enable manual container attachment
    + [--driver], [-d] [*bridge*, *overlay*] - driver to manage the network, default are bridge or overlay, can specify third party pluggins
    + [--ingress] - create swarm routing-mesh network
    + [--subnet] - subnet in CIDR format that represents a network segment (bridge network can only have one)
    + [--gateway] - specify gateway address, if omitted Engine selects it automatically
    + [--ip-range] - allocate container ip from a sub-range
    + [--aux-address]
    + [--opt], [-o] *\<arguments\>* - set driver specific options
	encrypted=true - option for overlay driver
* **ls** - list networks in the docker host
* **connect** - connect container to a software-defined network
* **disconnect** - disconnect container to a software-defined network
* **inspect** *\<network_name\>* - display detailed info about one or more networks
    + `--format`, `-f` - format output
    + `--verbose`, `-v` - verbose output

## Storage in docker

[to be continued]
https://docs.docker.com/storage/
https://www.ionos.com/community/server-cloud-infrastructure/docker/understanding-and-managing-docker-container-volumes/
https://stackoverflow.com/questions/30040708/how-to-mount-local-volumes-in-docker-machine


Files required to run the application and the data files that applications generates are separeted in Docker world. To make data persist no matter what you do with containers - use docker volumes.

By default all files created inside a container are stored on the writable layer:
* data doesn't persist when container is deleted
* writable layer is tightly coupled to the host
* writing to the writable layer requires a storage driver to manage the filesystem (union filesystem, using Linux kernel - extra abstraction that reduces performance)

Check [docker run with volume option](#docker-cli) to see how to connect container to existing volume.

### Storage types

#### Volumes

Stored in a part of the host filesystem which is managed by Docker (/var/lib/docker/volumes on Linux). The directory created at this path is what is mounted into the conrainer. Multiple containers can share one volume.

Easy back up data by accessing it on /var/lib/docker/volumes/volume_name.

If empty volume is mounted into directory in the container in which files or directories exist, they are copied over. If container is started with non-existing volume, empty volume is created.

#### Bind mounts

May be stored anywhere on the hot system. The file or directory is referenced by its full path on the host machine. Does not need to exists, will be created on demand. Can affect host's filesytem, as container has full access to it.
Good for sharing configuration files, source code or artufacts from the host to containers (by default Docker bind mounts /etc/resolv.conf).

#### tmpfs

Stored in the host system's memory only, and are never written to the host filesystem. Isnt persistent on disk, either on the Docker host or within container. Used by container during its lifetime to store non-persistent state or sensitive info (swarm uses tmpfs mounts to mount secrets into service's containers).
Best usage when you dont want data to persist either on the host or on container, or when writing large amount of non-persistent data

### docker volume

docker create *\<volume_name\>* and docker create --name *\<volume_name\>* do the same thing!

* **create** 
	- [--name] *\<volume_name\>* - specify volume name (if not specified, docker generates a random name)
	- [--driver], [-d] - specify volume driver name, defaults to *local*
	- [--opt], [-o] - set driver specific options
* **ls**
	- [--quiet], [-q] - only display volume names
* **inspect** *\<volume_name\>* - display detailed information on one or more volumes, can be useful to inspect if an image uses volumes
* **rm** *\<volume_name\>* - remove one or more volumes
	- [--force], [-f] - force removal

## docker-machine

...
Tool for provisioning and managing Dockerized hosts (hosts with Docker Engine on them), typically installed on local system, has its own command line client (docker-machine) and the Docker Engine client (docker)
Docker Engine is a client-server app made if the Docker daemon, REST API, CLI client that talks to the daemon (through REST API wrapper)

Can ssh into machine using "docker-machine ssh *\<machine_name\>*". To work as root either add sudo upfront or change to root either by "sudo -i" or "sudo su"; login and password for ssh connection - docker - tcuser

Docker autosend bug reports in case of docker-machine create or upgrade faulires, to opt out create .docker/machine/no-error-report file (doesnt have to have any contents)

### Commands

Not specifying the name of machine defaults to the name *default*

**docker-machine**
* **ls** - list available machines
* **create** *\<name\>* - create a docker machine
    + [--driver], [-d] *\<name\>* - set a driver; full list of avaliable drivers at https://docs.docker.com/machine/drivers/
    + [--driver] *\<name\>* --help - list available options to create a machine
* **ip** *\<name\>* - get machine's ip address
* **env** *\<name\>* - display commands to set up environment for docker (to run docker commands against particular machine)
    + [-u] - display commands to unset environment variabes
* **start** *\<name\>* - start machine
* **stop** *\<name\>* - stop machine
* **restart** *\<name\>* - restart machine
* **upgrade** *\<name\>* - upgrade machine to latest version of Docker(depends on underlying distribution on host); for example if uses Ubuntu, command is similar to sudo apt-get upgrade docker-engine
* **ssh** *\<machine_name\>* *\<command\>-* login or run a command on a machine using SSH, if no command specified logs in like regular ssh
    + [--native-ssh] - use Go ssh implementation instead of local one; docker by default tries to find local ssh and uses that it does find it
* **mount** *\<machinename:/path/to/dir\>* - 
    + [-u] *\<machinename:/path/to/dir\>* - unmount
* **inspect** *\<machine_name\>* - inspect info about a machine

### Links


## docker cli

Detach from container - Ctrl + p Ctrl + q

When stopping containers using **docker stop** docker waits 10 seconds by default(after sending SIGTERM) before it sends a SIGKILL.

### Commands

docker [command] --help - brings options list

**docker**
* **ps** - list containers (default to currently running containers)
    + [--all], [-a] - show all
    + [--quiet], [-q] - display only numeric IDs
    + [--size], [-s] - display total file sizes
* **run** - run a command in a new container(shorthand for **pull** (if needed), **create**, and **start** commands.
    + [--rm] - automatically remove container when it exits
    + [--tty], [-t] - allocate pseudo-tty
    + [--interactive], [-i] - keep stdin open even if not attached
    + [--detach], [-d] - run container in a background
    + [--publish], [-p] *\<host_port\>:\<container_port\>* - publish ports from host to container
    + [--volume], [-v] [*\<absolute_directory_path_on_host\>* or *\<volume_name\>*]:*\<absolute_path_in_container\>* - bind mount a volume, attach a filesystem mount to the container, tells docker to store data in the left argument (either directory on the host or volume). Good use includes binding configuration files, that you want to persist on the host. Use of mount option is preferable. [more info](https://docs.docker.com/storage/bind-mounts/)
    + [--mount] - https://docs.docker.com/storage/bind-mounts/
	+ [--network] *\<network_name\>* - connect container to the network (can also specify ip with --ip option).
	+ [--ip] *\<ip\>* - specify ip for container
    + https://stackoverflow.com/questions/35550694/what-is-the-purpose-of-the-i-and-t-options-for-the-docker-exec-command/35551071#35551071 - on TTY
* **logs** *\<container_name\>* - fetch the logs of a container
	+ [--details] - show extra details
	+ [--follow], [-f] - follow log output
	+ [--timestamps], [-t] - show timestamps
* **start**
* **attach**
* **exec**
* **cp** *\<container_name\>*:*\<src_path\> \<dest_path\>* (or vice versa) - copy files/folders between a container and the local filesystem, container can be stopped or running (details are below)
    + [--follow-link], [-L] - always follow symbol link in SRC_PATH (otherwise link itself is copied)
* **diff** - inspect changes to files or directories on a container's filesystem (based on image it was built from):
    + **A** - file or directory was added
    + **D** - file or directory was deleted
    + **C** - file or directory was changed (for folders that is file was added or removed from it)
* **images** - list images
    + [--quiet], [-q] - only show numeric IDs
* **pull** - pull(download) an image from a registry, can be used to update an image as well
* **build** .
    + [--tag], [-t] *\<name\>:\<tag\>* - tag resulting image
    + [--file], [-f] - name of the Dockerfile, default PATH/Dockerfile
* **commit** *\<container_name\>* *[\<repository:tag\>]* - create a new image from a container's changes
* **tag** *\<image_name\>* *\<username\/repository:tag\>* - give a new tag to image(prepare to push to remository), new tag is created and referenced the parent image
* **login** - login to dockerhub to push and pull images
    + [--username], [-u] *\<user_name\>*
    + [--password], [-p] *\<password\>*
    + [--password-sdtin] - accept password from stdin (thus not appear in shell history or logs), e.g. cat ~/my_password.txt | docker login -u user --password-stdin
* **logout** [*\<server\>*] - logout from registry
* **push** *\<image_name\>* - push an image to repository or registry

Copy rules:
* *\<src_path\>* is a file:
    + *\<dest_path\>* doesnt exist - file is saved to a created *\<dest_path\>*
    + *\<dest_path\>* doesnt exist and ends with '/' - error
    + *\<dest_path\>* exists and is a file - contents are overwritten
    + *\<dest_path\>* exists and is a directory - file is copied to the directory useing source name
* *\<src_path\>* is a directory:
    + *\<dest_path\>* doesnt exist - *\<dest_path\>* is created and the source contents of the source are copied over
    + *\<dest_path\>* exists and is a file - error
    + *\<dest_path\>* exists and is a directory:
	- *\<src_path\>* doesnt end with '/.' - source directory is copied into dest
	- *\<src_path\>* ends with '/.' - contents are copied over

### Links

------------

## docker stack

[Overview](#https://docs.docker.com/engine/reference/commandline/stack/)

Kubernetes is a possible host alongside docker swarm, example and supporting flags [to be continued]

Docker stack commands is required to run in a swarm mode. The context for compose file is taken from [context.go](#https://github.com/docker/swarmkit/blob/master/template/context.go)

Works only with compose file version 3!

```yml
version: "3"
services:
web:
    image: artnova/pyweb
    environment:
        NODENAME: "{{.Node.Hostname}}"
```
Single container running in a service is called a _task_. Collection of services that make ap an application is called a _stack_.

### Commands

**docker stack**
* **deploy** *\<stack_name\>* - deploy a new stack or update an existing one(no need to shut down, just apply changes and run command again); supports compose file v3.0 and higher
    + [--compose-file], [-c] *\<path_to_docker-compose.yml\>* - path to docker-compose.yml, can provide multiple files by multiple flags (**docker service ls** to confirm correct creation)
* **ps** *\<stack_name\>* - view all tasks of a stack, [formatting output](#https://docs.docker.com/engine/reference/commandline/stack_ps/#formatting)
    + [--quiet], [-q] - only show IDs of the tasks, can be useful to be used with **docker inspect**

		    docker inspect $(docker stack ps -q voting)

* **ls** - lists the stacks
* **services** - list the services on the stack
* **rm** *\<stack_name\>* - removes the stack from the swarm(has to be run targeting a manager node) that is services, networks and secret associations

### Links

[Difference with docker-compose tool](https://vsupalov.com/difference-docker-compose-and-docker-stack/)

------------

## docker service

Service can be viewed as a separate part of the app, in docker sense - separate container(piece of software). Service can be scaled through replicas - number of containers running that(same) piece of software.

Load-balancing is done through round-robin fashion(after last one comes first) (or ingress load balancing?)

### Commands

**docker service**
* **ls** - lists services running in the swarm(has to be run targeting a manager node)
* **ps** *\<service\>* - list the tasks of one or more services(has to be run targeting a manager node)
* **logs** - show logs of a service or task
    + [--follow], [-f] - follow log output
* **create** - create a service in a swarm (must be run on a manager node)
	+ [--mode] [replicated | global] - defaults to **replicated**, which means service runs as many containers as needed, **global** sets one container to each active node in the swarm
    + [--name] *\<service_name\>* - set service name
    + [--network] *\<network_name\>* - attach a service to an existing network
    + [--replicas] *\<number\>* - set number of containers for the service
    + [--publish], [-p] *\<host:service\>* - publish service ports externally to the swarm
* **scale** *\<service_name=number\>* - scale one or more services
* **update** *\<service_name\>* - changes the configuration of a service after it has been created
	+ [--publish-add] *\<outside:inside\>* *\<service_name\>* **or** published=*\<outside\>*, target=*\<inside\>* - add or update a port mapping, arguments are numbers
	+ [--publish-rm] *\<outside:inside\>* - remove a port mapping, arguments are numbers
* **rm** *\<service_names\>* - remove one or move services

### Links

------------

## docker swarm and docker node

...
A swarm is a group of machines that are running Docker and joined into a cluster. Each server is know as a node, and can be either a manager of worker (slave). Each Docker commands are now executed on a whole cluster by a swarm manager.
Machines can be physical or virtual, referred as nodes in swarm.

Always run docker swarm init and join with port 2377 or leave it blank for default(swarm manager port)

Removing nodes from the swarm includes both leaving the swarm with **docker swarm leave**, then removing it from the list on a manager with **docker node rm** *\<node_name\>*

When deploying containers for a project (application), database container doesn't fit to swarm model - any container can be easily replicated by starting ne w containers and be easily moved accross the nodes. Thus, you may want to create database service with a constraint flag to be created on a specific node (also with volume flag, etc). Use constraints to control which nodes will be assigned containers (by hostname, role, label, etc).

Docker service command run against swarm is reflected on all nodes - for example post being exposed from service to outside world is done on all nodes, meaning any request recieved to specified port will be recieved by Docker and sent to one of the containers in the service. (ingress load balancing)

[alternative is Kubernetes](https://kubernetes.io)

### Commands

**docker swarm**
* **init** - initialize swarm, targeted docker engine becomes a manager; generates two random tokens - manager and worker
    + [--advertise-addr] - addvertise address
* **join** *\<host:port\>* - join a swarm
    + [--token] *\<token\>* - token for entry (different for worker and manager); acquire token with **join-token** command below
* **leave** - leave a swarm, works without warning for workers
    + [--force] - use it on manager, when swarm wont be used anymore; proper way is to demote manager to worker, then leave the swarm without warnings
* **join-token** [*manager*, *worker*] - print token to join the existing swarm as manager or a worker (run on manager node)

**docker node**
* **ls** - list all the nodes that the Docker Swarm manager knows about(used on manager node)
* **ps** *\<node_name\>* - list tasks running on one or more nodes, default to current node
    + $(docker ls -q) - list all tasks in a swarm(better use docker stack ps)
* **rm** *\<node_name\>* - remove one or more node s from the swarm
    + [--force], [-f] - force remove
* **update** *\<node\>* - update metadata about a node (availabilty, labels, roles...)
    + [--availability] [activea | pause | drain] *\<node_name\>* - let the manager know about changes; is done before taking out node for "maintenance", manager is now able to rebuild, rebalance the services; **pause** tells manager not to assign new containers, **drain** stops existsing containers and doesnt assign new ones
	+ [--force] - try out the example at page 140 in Essential docker
	+ [--image] - update images for containers, is done by stopping each container in turn (to speed up the process run **docker service --update-parallelism** command)
	+ [--label-add] *\<label\>* - add a label
    + [--label-rm] *\<label\>* - remove a label
    + [--role] - worker or manager
* **demote** *\<manager_node_name\>* - demote manager to a worker
* **promote** *\<node_name\>* - promote node to a manager (can be run only on a manager)

### Links

https://docs.docker.com/engine/reference/commandline/swarm_init/
https://www.youtube.com/watch?v=bU2NNFJ-UXA

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

teamspeak server - https://www.hostinger.com/tutorials/how-to-make-a-teamspeak-3-server/

Building ruby on rails docker image https://medium.com/@lemuelbarango/ruby-on-rails-smaller-docker-images-bff240931332, https://ncona.com/2017/09/getting-rails-to-run-in-an-alpine-container/

Building gitlab image manually https://docs.gitlab.com/ee/install/installation.html, https://about.gitlab.com/install/#debian, https://packages.gitlab.com/gitlab/gitlab-ce/install
Problems:
    - https://gitlab.com/gitlab-org/omnibus-gitlab/issues/4257 - configuration freezes on ruby-block
    - https://gitlab.com/gitlab-org/omnibus-gitlab/issues/430
    - https://serveradmin.ru/ustanovka-gitlab-v-lxc-konteyner/
    - https://docs.gitlab.com/omnibus/common_installation_problems/
Further configuration - https://docs.gitlab.com/omnibus/README.html#installation-and-configuration-using-omnibus-package

### Instructions

----------

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

----------

#### RUN
- **RUN** _\<command\>_ (shell form) - defaults to _/bin/sh -c_ on Linux; can use \ to break line
- **RUN** [_"exec", "param1", "param2"_](exec form) - parsed as JSON array, that is must use double-quotes; doesn't invoke command shell, so **RUN** ["echo", "$HOME"] wont be substituted, run with shell or use shell form

Execute commands in a new layer on top of of the current image; default shell can be changed using **SHELL** or use exec form; cache for **RUN** can be used during next build - can be invalidated using _--no-cache_ flag on build

Avoid using apt-get upgrade and dist-upgrade, as many essential packages from parent images can not upgrade inside an uprivileged container, rather update and install packages you need (keep it in one command to avoid caching issues, if command changes in the future):

	RUN apt-get update && apt-get install -y \
	    package-one \
	    package-two 

----------

#### CMD
- **CMD** [_"exec", "param1", "param2"_] (exec form)
- **CMD** [_"param1", "param2"_] (as default params to **ENTRYPOINT**)
- **CMD** command param1 param2 (shell form) - will be executed in /bin/sh -c

Provide defaults for an executing container, can include an executable or can omit exec, but specify **ENTRYPOINT** as well; if **CMD** is used to provide defaults to **ENTRYPOINT**, both should be with the JSON array format; can be only one **CMD** in Dockerfile; doesnt execute anything at build time; if arguments are specified on docker run, then they will override the default in **CMD**

Should almost always be used in exec form (like **CMD** ["apache2", "-DFOREGROUND"]), in most other cases should be given an interactive shell, such as bash, python and perl, so that docker run -it python would will get user dropped into usable shell. In rare cases should be used as params to **ENTRYPOINT**

----------

#### LABEL

[to be continued]

----------

#### ENTRYPOINT
- **ENTRYPOINT** [_"exec", "param1", "param2"_](exec form, preferred)
- **ENTRYPOINT** command param1 param2 (shell form)

Configure container that will run as exec; both **CMD** and **ENTRYPOINT** define what commands executed when running a container; command line arguments to docker run \<image\> will be appended after all elements in an exec form **ENTRYPOINT** and override all **CMD** elements; only last **ENTRYPOINT** will have an effect

Can be used in combination with a helper script, allowing it to work just like in combination with **CMD** - having default and particular behaviour. Script uses the _exec_ Bash command so that the final running application becomes the container's PID 1, allowing to receive any Unix signals

```bash
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
```

----------

#### EXPOSE
- **EXPOSE** _\<port\>_ [_<port>/protocol>..._] 

Informs Docker that the container listens on the specified network port at runtime(doesn't actually publish them, use -p flag of docker run to publish or -P to publish all exposed port randomly), defaults to TCP(can specify UDP)

Default is tcp, can also specify udp:

	EXPOSE 80/tcp
	EXPOSE 4242/udp

----------

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

----------

#### COPY
- **COPY** _\<src>... \<dest\>_
- **COPY** ["_\<src>_",... "_\<dest\>_"]

Copies new files or directories from \<src\> and adds them to the filesystem at the path \<dest\>, same rules as **ADD** apply to **COPY**

**COPY** is generally preferred over **ADD** as it is more transparent and provides basic functionality. **ADD** best usage is for local tar extraction. In case of multiple Dockerfile steps, **COPY** them individually - results in fewer cache invalidations, as each **COPY** is on seperate layer.

----------

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

----------

#### ARG
- **ARG** _\<name\>[=\<default\>]_ 

Defines a variable that users can pass at build-time to the builder using _docker build --build-arg \<varname\>=\<value\>_(can be viewed by any user with docker hitory command - therefore not recommended for passwords, etc); can optionally include a default value; comes into effect from the line on which it was defined and ends at the end of the build stage - out of scope results in empty string; **ENV** always overried **ARG**; predefined ARGS (excluded from _docker history_ and is not cached) - HTTP_PROXY HTTP_proxy HTTPS_PROXY http_proxy FTP_PROXY ftp_proxy NO_PROXY np_proxy; "cache miss" occured upon first usage, not definition(if value has changed)

----------

#### VOLUME

[to be continued]

----------

#### WORKDIR
- **WORKDIR** /path/to/workdir

Sets the working directory for any **RUN**, **CMD**, **ENTRYPOINT**, **COPY** and **ADD** instructions, if doesnt exist it is created; can resolve previously set **ENV** variables

----------

#### USER
- **USER** \<user\>[\<group\>
- **USER** \<UID\>[\<GID\>]

Sets the user name (or UID) to use when running the image and any **RUN**, **CMD**, **ENTRYPOINT** instructions that follows. Avoid installing or using sudo as it has unpredictable TTY and signal-forwarding behavior. For similar to sudo behavior consider "gosu" (https://github.com/tianon/gosu).

----------

#### ONBUILD
- **ONBUILD** **[INSTRUCTION]**

Adds a trigger instruction to be executed at a later time, when current image is used as a base image, as if it was inserted right after **FROM** instruction in the calling Dockerfile.
All instructions under **ONBUILD** are saved under OnBuild key and can be inspected with **docker inspect** command. Triggers are not inherited by children(not saved under Onbuild key in a resulting image).
Can not trigger **FROM**

----------

#### STOPSIGNAL

[to be continued]

----------

#### HEALTHCHECK

[to be continued]

----------

#### SHELL

[to be continued]

----------

### Links

- https://docs.docker.com/engine/reference/builder/
- https://kapeli.com/cheat_sheets/Dockerfile.docset/Contents/Resources/Documents/index
- [COPY with --from flag](#https://medium.com/@tonistiigi/advanced-multi-stage-build-patterns-6f741b852fae)

------------


# Docker compose

Not supported for docker stack deploy:
* build
* cgroup_parent
* container_name
* devices
* tmpfs
* external_links
* links
* netword_mode
* restart
* security_opt
* userns_mode

Multiple arguments can be specified either in dictionary style:

	instruction:
	    var1: value1
	    var2: value2

or in list style:

	instruction:
	    - var1=value1
	    - var2=value2

Specifying durations - string format, supports _us_, _ms_, _s_, _m_ and _h_

	2.5s
	10s
	1m30s
	2h32m56s

Specifying byte values - string format, supports _b_, _k_, _m_ and _g_, and alternatives _kb_, _mb_, _gb_

	2b
	1024kb
	2048k
	1gb

## docker-compose tool

[Overview](https://docs.docker.com/compose/reference/overview/)

Docker compose is a separete tool (used to be separate Python project) that allows you to create all parts of a single application automatically: containers, volumes, networks, etc. Under hood it uses docker engine APT to perform necessary steps. It accepts a docker-compose.yml file, where one describes the components of "infrastructure". [Read more about difference of docker-compose tool and built-in docker stack](https://vsupalov.com/difference-docker-compose-and-docker-stack/)

docker-compose [-f] \<path\> - specify path to docker-compose.yml file; can specify two, the later is applied over and in addition to previuos files; if nothing is specified docker is looking for docker-compose.yml and docker-compose.overried.yml - must supply at least the first; followed by '-' instructs to read from stdin

Network, volume and service definitions are applied to each container respectively (analogy to docker network create, docker volume create). All parts are prefixed with the directory name containing configuration file (docker-compose.yml) to ensure different compose files can create the same name networks, volumes, containers, etc (to change this behaviour check out -p flag of the docker-compose command).

Use **up** to set up and start services the first time, **run** for "one-off" tasks, **start** for restarting previously created containers.

### Commands

**docker-compose**
* [--file], [-f] *\<file_name\>* - specify alternate compose file (defaults to *docker-compose.yml*)
* [--project-name], [-p] *\<name\>* - specify alternate project name (defaults to directory name where compose file resides)
* **config** - validate and view the Compose file
    + [--services] - print services, one per line
    + [--volumes] - print volumes one per line
    + [--quiet], [-q] - only validate, dont print anything
* **build** - create images required for the services it contains
    + [--parallel] - build images in parallel
    + [--force-rm] - always remove intermediate containers
    + [--no-cache] - do not cache when build the image
* **up** [*\<service_name, ...\>*] - build, recreate, start and attach to container. Can optionally pass service name(s) to start services selectively.
    + [--detach], [-d] - set a driver; [full list of avaliable drivers](https://docs.docker.com/machine/drivers/)
    + [--build] - build images before starting(force to even if they exist)
* **down** - stops and removes containers, networks, volumes and images createad by up, external networks and volumes are not removed
    + [--rmi] *\<type\>* - remore images, type is either *'all'* (all images used by any service) or *'local'* (only images that dont have custom tag set by the 'image' field
    + [--volume], [-v] - remove named volumes declared in 'volumes' section and anonymous volumes attached to containers
* **exec** - equivalent of docker exec, allows to run commands in services (by default allocates TTY, example, docker-compose exec web sh)
    + [--detach], [-d] - run command in a background
    + [--privileged] - give extended privileges to the process
    + [--index]*\<index\>* - index of the container if there are multiple instances of a service (default is 1)
    + [--workdir], [-w]*\<dir\>* - path to workdor directory for this command
* **kill** - forces conainers to stop by sending SIGKILL, optinally other signals can be sent
    + [-s]*\<SIGNAL\>* - SIGNAL to send to the container
* **logs** - display logs output from services
    + [--folow], [-f] - follow output
* **pause** - pauses running containers of the service, can be unpaused by **docker-compose unpause**
* **ps** - list containers
    + [-quiet], [-q] - only display IDs
    + [--services] - display services
    + [--all], [-a] - show all stopped containers
* **restart** - restart all stopped and running services, for changes in Compose file use restart_policy
* **rm** - remove stopped service containers
* **scale** *\<service_name\>*=*\<number\>*- sets the number of containers to run for a service; alternatively in Compose file 3. can specify replicas under deploy key, deploy key only works woth docker stack deploy command
* **top** - list running processes
* **events** - stream container events for every container in the project
    + [--json] - json object is printed one per line in the following format
		```json
		{
		    "time": "2015-11-20T18:01:03.615550",
		    "type": "container",
		    "action": "create",
		    "id": "213cf7...5fc39a",
		    "service": "web",
		    "attributes": {
			"name": "application_web_1",
			"image": "alpine:edge"
		    }
		}
		```

### Links


------------


### Instructions

**version** - specify version of compose file to use (f.e. 3, 2, 3.7...)

**services** - denotes a section where services (distinct containers) are described.

#### build

Can be specified as a string containing a path to build context or as an object with the path specified under context and optionally Dockerfile and args. If **image** is also specified, then Compose names the build images with \<name\>:\<tag\> and optinal tag

* **context** - path to build context or url to git repository; if relative, then relative to compose file
* **dockerfile** - dockerfile to use
* **args** - environment variables accessible only during the build process (first should specify in Dockerfile using **ARG** instructiona); if value is ommitted then environment variables where the compose file is run is used
* **cache_from** (v3.2) - a list of images that the engine uses for cache resolution

		build:
		    context: .
		    cache_from:
			- alpine:latest
			- corp/web_app:3.14

* **labels** (v3.3) - add metadata to the resulting image using Docker labels(https://docs.docker.com/config/labels-custom-metadata/); recommended to use reverse-DNS notation to avoid conflicts
* **shm_size** (v3.5) - set the size of /dev/shm partition for this build's containers, specify as the number of bytes are a sting representing a byte value(https://docs.docker.com/compose/compose-file/#specifying-byte-values)
* **target** (v3.4) - build the stage as defined inside Dockerfile(https://docs.docker.com/engine/userguide/eng-image/multistage-build/)
* **cap_add** - add or drop container capabilities (ignored when deploying a stack)
* **cgroup_parent** - optinal parent group
* **command** - override default command, can be either in shell or list form
* **configs** - grant access to configs [to be continued]
* **container_name** - specify custom name, cant be scaled(cause ever container name must be unique), ignored when deploying stack
* **credential_spec** - [to be continued]
* **depends_on** - express dependancies between services, docker-compose up starts redis and db before web(waits only until they have started, not until the are "ready", to control deploying stage - https://docs.docker.com/compose/startup-order/), docker-compose stop stops web before redis and db; ignored when deploying stack

		version: "3.7"
		services:
		    web:
			build: .
			depends_on:
			    - db
			    - redis
		    redis:
			image: redis
		    db:
			image: postgres

#### deploy

Specify configuration related to deployment and running of services. Only takes effect when deploying with a docker stack deploy and ignored by docker-compose up and docker-compose run

* **endpoint_mode** - specify a service discovery method for external clients connecting to a swarm
    + vip - docker assignes the service a virtual IP (VIP) that acts as the front end for clients to reach the service on a network. Docker routes requests between the client and the available worker nodes for the service without client knowledge of how many nodes are participating in the service or their IP addresses or ports (it is default)
    + dnsrr - DNS round-robin (DNSRR), does not use a single virtual IP; Docker sets up DNS entries for the services such that a DNS query for the service name returns a list of IP addresses; useful for using own load balancer for example
* **labels** - specify labels for the service; only set on service, not containers, to set on containers use labels outside deploy
* **mode**
    + [global] - exactly one container per swarm node
    + [replicated] - specified number of containers (default)
* **placement** - specify placement of constraints and preferences(more info at https://docs.docker.com/engine/reference/commandline/service_create/#specify-service-placement-preferences-placement-pref)
    + node.role == manager

			deploy:
			    placement:
				constaints: [node.role == manager]

			deploy:
			    placement:
				constaints:
				    - node.role == manager

* **replicas** - specify number of containers
* **resources** - configure resource constraints, analogoes to docker service create counter part [to be continued]
* **restart_policy** - configures if and how to restart containers when they exit
    * condition: [on-failure or any] - any is default
    * delay: \<number\>s - specify how long to wait (default is 0s)
    * max_attempts: \<number\> - default is never give up; if restart doesnt succeed within window that attempt does count toward max_attempts
    * window: \<number\>s - how long to wait before deciding if a restart has succeded, specified as a duraton (default immediatly)

	deploy:
	    restart_policy:
		condition: on-failure
		delay: 5s
		max_attempts: 3
		window: 120s

* **rollback_config** (v3.7) - configures how the service should be rolledback in case of failing update
    * parallelism: \<number\> - number of containers to roll back at a time, if set to 0 all rollback simultaneously
    * delay: \<number\>s - time to wait between each container group's rollback (default is 0)
    * faulire_action: [continue or pause] - what to do of rollback fails (default is pause)
    * monitor: ns|us|ms|s|m|h - duration after each task update to monitor for fauilre (default is 0)
    * max_faulire_ration: - faulire rate to tolerate (default is 0)
    * order: [stop-first or start-first] - order of operations during rollback, stop - old task is stopped before starting new, start - new task is started first and the running task brieflt overlaps (default is stop-first)

* **update_config** - configures how the service should be updated; same agruments as rollback_config

#### devices

List of device mappings, uses the same format as the --device docker client create option

	devices:
	    - "/dev/ttyUSB0:/dev/ttyUSB0"

#### dns

Custom DNS servers

	dns:
	    - 8.8.8.8
	    - 9.9.9.9

#### dns_search

Custom DNS search domain

	dns_search:
	    - dnc1.example.com
	    - dnc2.example.com

#### entrypoint

Override the default entrypoint, both ENTRYPOINT and CMD instructions in Dockerfile are ignored

	entrypoint: /code/entrypoint.sh

	entrypoint:
	    - php
	    - -d
	    - zend_extension=/usr/local/lib/php/extensions/no-debug-non-zts-20100525/xdebug.so
	    - -d
	    - memory_limit=-1

#### env_file

[to be continued]

#### environment

Add environment variables either as array or dictionary; any boolean values true, false, yes, no... need to be enclosed in quotes; environment variables with only a key are resolved to their values on the machine Compose is running on (helpful for secret or host specific values)

	environment:
	    RACK_ENV: development
	    SHOW: 'true'
	    SESSION_SECRET:

#### expose

Expose ports without publishing then to the host machine - only accessible to linked services, only internal port can be specified

#### external_links

[to be continued]

#### extra_hosts

[to be continued]

#### healthcheck

[to be continued]

#### image

Specify image to start the container from, can either be a repository/tag or partial image ID

#### init

[to be continued]

#### isolation

[to be continued]

#### labels

[to be continued]

#### links [legacy feature]

[to be continued]

#### logging

[to be continued]

#### network_mode

Network mode, uses the same values as the docker client --network parameter, plus the special form _service:[service name]_

[to be continued]

#### networks

Networks to join, referencing entries under the top-level network key

	services:
	    some-service:
		networks:
		    - some-network
		    - other-network

* **aliases** - alternative hostnames for the service on te network, other services can use either service name or this aliases to connect to one of the service's containers; same service can have different aliases on different networks:

		networks:
		    some-netwerk:
			aliases:
			    - alies1
			    - alies3
		    other-netwerk:
			aliases:
			    - alies2

* **ipv4_address**, **ipv6_address** - specify a static IP address for containers when joining networks; corresponding networks configuration in the top-level networks section must have an ipam block with subnet configurations covering each static address

		version: "3.7"

		services:
		  app:
		    image: nginx:alpine
		    networks:
		      app_net:
			ipv4_address: 172.16.238.10
			ipv6_address: 2001:3984:3989::10

		networks:
		  app_net:
		    ipam:
		      driver: default
		      config:
			- subnet: "172.16.238.0/24"
			- subnet: "2001:3984:3989::/64"

#### pid

[to be continued]

#### ports

[to be continued]

#### restart

[to be continued]

#### secrets

[to be continued]

#### security_opt

[to be continued]

#### stop_grace_period

Specify how long to wait when attempting to stop a container if it doesnt handle SIGTER (or whatever top signal has been specified with stop_signal), before sending SIGKILL; specified as duration (default is 10s)

#### stop_signal

Set alternate signal to stop the container (default is SIGTERM)

#### sysctls

[to be continued]

#### tmpfs

[to be continued]

#### ulimits

Override the default ulimits for a container, can either specify a single limit as an integer or soft/hard limits as a mapping

	ulimits:
	    nproc: 65535
	    nofile:
		soft: 20000
		hard: 40000

#### userns_mode

[to be continued]

#### volumes

Section that describes the volumes that will be used by containers.

Mount host paths or named volumes, specified as sub-options to a service. Can mount a host path as part of a definition for a single service, for reusable volume across multiple services define a named volume in top-level volume key(service, swarms, stack files).

When working with services, swarms and stacks use named volumes and constraints to run on manager only for databases, as anonymous volumes may be created on different nodes each time

* short syntax

	volumes:
	    # specify path in container, docker engine will automatically create volume on host and assign it
	    - /var/lib/mysql

	    # absolute path mapping
	    - /opt/data:/var/lib/mysql

	    # path on the host relative to docker-compose file (must start with . or ..)
	    - ./cache:/var/lib/mysql

	    # named volume
	    - databolume:/var/lib/mysql

* long syntax (v3.2) - allows additional configuration fields to be added
    + **type**: [volume, bind, tmpfs, npipe]
    + **source**: source of the mount, path on the host for a bind mount, or name of a volume
    + **target**: path in the container
    + **read_only**: flag to set as read-only
    + **bind**: additional bind options (?)
    + **volume**: additional volume options
	- **nocopy**: [true, false] flag to disable copying of data from a container when a volume is created
    + **tmpfs**: additional tmpfs options
	- **size**: size in bytes
    + **consistency**: [**consistent**(host and container have identical view), **cached**(read cache, host view is authorative), **delegated**(read-write cache, container's view is authorative)]

Top-level key
* **driver**: specify which volume driver should be used (default is local)
* **driver_opts**: specify a list of options to pass to driver (driver dependent)
* **external**: [**true**(specifies that volume has been created outside Compose, thus doesnt try to create and raises an error if doesnt exist)]
* **labels**: [to be continued]
* **name**: set a custon name, can be used to reference volumes that can contain special character

#### single value

Analogous to docker run counterpart:
* user
* working_dir
* domainname
* hostname
* ipc
* mac_address (legacy)
* privileged
* read_only
* shm_size
* stdin_open
* tty

### Links


------------

# Kubernetes

## Contents

+ [General](#general)
	+ [namaspace](#namespace)
+ [Objects](#objects)
+ [Manifest file](#manifest-file)
+ [Volumes](#volumes)
+ [Services](#services)
+ [kubectl](#kubectl)
+ [Additional notes](#additional-notes)

## General

Containers are isolated user spaces per running application code. The user
space is all the code that resides above the kernel, and includes the
applications and their dependencies. Abstraction is at the level of the
application and its dependencies.

Containerization helps with dependacy isolation and integration problem
troubleshooting.

Core technologies that enhanced contanerization:
+ process - each process has its own virtual memory address space seperate from
others
+ Linux namespaces - used to control what application can see (process ID
numbers, directory trees, IP addresses, etc)
+ cgroups - controls what application can use (CPU time, memory, IO bandwidth,
etc)
+ union file system - incapsulating application with its dependacies

Google's public image registry - gcr.io

Google Kubernetes Engine features:
+ fully managed - manages all underlying resources
+ uses Google's container-optimized OS
+ auto-upgrade
+ auto repais nodes by gracefully draining unhealthy ones and restarting them
+ cluster scaling
+ integrates with Cloud Build and Cloud Registry
+ integrates with IAM services
+ integrates with logging and monitoring
+ integrates with Google's VPC
+ provides insights to resources and clusters via GCP Console

Kubernetes control plane components:
+ kube-APIserver - accepts commands to view or change the state of cluster (user
interracts with it via `kubectl` command); also authenticates commands and
manages admission control
+ etcd - cluster's database; stores cluster's state, configuration and dynamic
data
+ kube-scheduler - schedules pods into nodes; discovers policies, hardware and
software to assign node to a pod by simply writing it in pod object.
+ kube-controller-manager - continuously monitors cluster's state through
kube-APIserver and applies changes if they are needed
+ kube-cloud-manager - manages controllers that interact with underlying cloud
providers
+ kubelet (runs on nodes) - kubernetes agent that kube-APIserver talks to
+ kube-proxy (runs on nodes) - provides network connectivity among pods in a
cluster

Kubernetes doesn't create nodes nor it handles failed nodes. Open-source Kuber
ADM can automate some of the initial cluster setup, GKE takes the responsibility
of managing and provisioning nodes, creating them and recreating when needed. In
GKE master node is abstracted away for customer and is not included in billing.

### Namespace

Namespaces can abstract single physical layer into multiple clusters. They
provide scope for naming resources like pods, controllers and deployments. User
can create namespaces, while Kubernetes has 3 default ones:
+ default - for objects with no namespace defined
+ Kube-system - for objects created by Kubernetes itsef (ConfigMap, Secrets,
Controllers, Deployments); by default these items are excluded, when using
kubectl command (can be viewed explicitly)
+ Kube-public - for objects publicly readable for all users

Can be applied for resource in command line or manifest file, while first option
is preferred for manifest file to be more flexible.

## Objects

Everything in Kubernetes is represented by an object with state and attributes
that user can change. Each object has two elements: object spec (desired state)
and object state (current state). All Kubernetes objects are identified by a
unique name(set by user) and a unique identifier(set by Kubernetes).

### Pod

Pod is the smallest deployable object (not
container). Pod embodies the environment where container lives, which can hold
one or more containers. If there are several containers in a pod, they share all
resources like networking and storage. Kubernetes assigns unique IP address to a
pod, which containers inside share among each other (containers in a pod can
also communicate through localhost).

---

### Controller

Manages state of pods. Good choice for long-living software components.

Examples:
+ ReplicaSets
+ Deployments
+ Replication Controllers
+ StatefulSets
+ DaemonSets
+ Jobs

---

#### ReplicaSet

ReplicaSet controller ensures that a population of Pods, all identical to one
another, are running at the same time.

---

#### Deployment

Deployment object really creates a ReplicaSet object to manage the pods, while
user acrually interacts with Deployments object. This allows the latter to
perform a rolling upgrade by creating a second ReplicaSet object and increase
the number of upgraded Pods in the second ReplicaSet while it decreases the
number in the first ReplicaSet.

---

#### Replication

Replication Controllers perform a similar role to the combination of ReplicaSets
and Deployments, but their use is no longer recommended. Deployments provide a
helpful "front end" to ReplicaSets.

---

#### StatefulSet

Better choice for applications that maintain local state. Unlike Deployment,
Pods in StatefulSet have persistent identities with stable network identity and
persistent disk storage.

---

#### DaemonSet

Good choice to run certain Pods on all the nodes within the cluster or on a
selection of nodes. DaemonSet ensures that a specific Pod is always running on
all or some subset of the nodes. If new nodes are added, DaemonSet will
automatically set up Pods in those nodes with the required specification. The
word "daemon" is a computer science term meaning a non-interactive process that
provides useful services to other processes. A Kubernetes cluster might use a
DaemonSet to ensure that a logging agent like fluentd is running on all nodes in
the cluster.

---

#### Job

The Job controller creates one or more Pods required to run a task. When the
task is completed, Job will then terminate all those Pods. A related controller
is CronJob, which runs Pods on a time-based schedule.

## Manifest file

File describing objects that you Kubernetes to create and maintain. Can be json
or yamn format.

Required fields:
+ `apiVersion` - Kubernetes API version
+ `kind` - type of object
+ `metadata` - identifies the object with `name` and optionally `labels`
(key-value pair that can be tagged during or after creating)
+ `spec`

## Volumes

## Services

Services provide durable endpoints to Pods. It is a static IP address that
represents a service or a function (you can group several Pods into one that
provide same service). Thus, dynamically created Pods can have persistent
endpoint for other services to communicate with. By default, the master assigns
a virtual IP address also known as a cluster IP to the service from internal IP
tables. With GKE, this is assigned from the clusters VPC network.

To get service quickly by asking Kubernetes to expose a deployment.

3 primary types of services:
+ ClusterIP: Exposes the service on an IP address that is only accessible from
within this cluster. This is the default type.
+ NodePort: Exposes the service on the IP address of each node in the cluster,
at a specific port number.
+ LoadBalancer: Exposes the service externally, using a load balancing service
provided by a cloud provider.

## kubectl

First `kubectl` should be configured to be able to access the cluster. Congig
file resides at $HOME/.kube/config, which contains cluster names and credentials
to access them. `kubectl config view` to view config file.  The kubeconfig file
can contain information for many clusters. The currently active context (the
cluster that kubectl commands manipulate) is indicated by the current-context
property.

`kubectl config use-context [CONTEXT] - switch context` - switch context

`gcloud container clusters get-credentials [CLUSTER NAME] --zone [ZONE]` - write
into config file in `.kube` directory in home by default.

```shell
source <(kubectl completion bash)
```

```shell
> kubectl [command] [TYPE] [NAME] [flags]
```
+ command - action to perform (get, describe, logs...)
+ TYPE - type of object to perfrom an action on (pods, deployments, nodes)
+ NAME - name of object
+ flags - additional behavior (`-o=yaml` - output in yaml format, `-o=wide` -
get additional columns of information)

### Examples

```shell
> kubectl top nodes # info on nodes status
> kubectl cp ~/test.html $my_nginx_pod:/usr/share/nginx/html/test.html # copy files
> kubectl apply -f ./new-nginx-pod.yaml # apply manifest file and start a container
```

### Introspection

Commands to view info about kubernetes objects.

+ `kubectl get pods` - view all pods in a cluster and info on them; possible
states:
	+ `Pending` - image is retrived, but container hasn't started yet
	+ `Running` - successfully attached to a node and all containers are running
	+ `Succeded` - containers terminated successfully and won't be restarting
	+ `Failed` - containers terminated with a failure
	+ `Unknown` - most likely comminucatin error between master and kubelet
	+ `CrashLoopBackOff` - one of containers unexpectdly exited after it was
	restarted at least once (most likely pod isn't configured correctly)
+ `kubectl describe pod [POD_NAME]` - get more detailed info on specific pod
including containers' states
+ `kubectl exec [POD_NAME] -- [COMMAND]` - execute commands and application on a
pod; use `-c` flag to specify container
	+ `kubectl exec -ti [POD_NAME] -- /bin/bash` - get interactive shell
+ `kubectl logs [POD_NAME]` - view logs of a pod; use `-c` flag to specify
container; contains both stdout and stderr


## Additional notes

+ https://cloud.google.com/kubernetes-engine/
+ `gcloud container clusters create $my_cluster --num-nodes 3 --zone $my_zone
--enable-ip-alias` - create cluster in GKE, more info at
[https://cloud.google.com/sdk/gcloud/reference/container/clusters/create]

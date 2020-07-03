# Kubernetes

# Contents

+ [General](#general)
	+ [namaspace](#namespace)
+ [Objects](#objects)
	+ [Controller](#controller)
		+ [Deployment](#deployment)
+ [Manifest file](#manifest-file)
+ [Volumes](#volumes)
+ [Services](#services)
+ [kubectl](#kubectl)
+ [Additional notes](#additional-notes)

# General

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

## Namespace

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

# Objects

Everything in Kubernetes is represented by an object with state and attributes
that user can change. Each object has two elements: object spec (desired state)
and object state (current state). All Kubernetes objects are identified by a
unique name(set by user) and a unique identifier(set by Kubernetes).

## Pod

Pod is the smallest deployable object (not
container). Pod embodies the environment where container lives, which can hold
one or more containers. If there are several containers in a pod, they share all
resources like networking and storage. Kubernetes assigns unique IP address to a
pod, which containers inside share among each other (containers in a pod can
also communicate through localhost).

---

## Controller

Manages state of pods. Good choice for long-living software components.

Examples:
+ ReplicaSets
+ Deployments
+ Replication Controllers
+ StatefulSets
+ DaemonSets
+ Jobs

---

### ReplicaSet

ReplicaSet controller ensures that a population of Pods, all identical to one
another, are running at the same time.

---

### Deployment

Deployments describe the desired state of pods.

Deployment object really creates a ReplicaSet object to manage the pods
(specific version), while user acrually interacts with Deployments object. This
allows the latter to perform a rolling upgrade by creating a second ReplicaSet
object and increase the number of upgraded Pods in the second ReplicaSet while
it decreases the number in the first ReplicaSet.

Designed for stateless applications, like web front end, that don't store data
or application state to a persistent storage.

Changes in yaml config file automatically trigger rollign updates (?). You can
pause, resume and check status of this behaviour.
```shell
> kubectl rollout [pause|resume|status] deployment [DEPLOYMENT_NAME]
```

Delete deployment with `kubectl delete deployment [DEPLOYMENT_NAME]`.

#### Yaml example

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: my-app
spec:
  replicas: 3
  template:
    metadata:
      labels:
        app: my-app
    spec:
      containers:
      - name: my-app
        image: gcr.io/demo/my-app:1.0
        ports:
        - containerPort: 8080
```

#### Creation

3 ways to create deployment:
1. declaratively via yaml file:
```shell
> kubectl apply -f [DEPLOYMENT_FILE]
```
2. imperatively using `kubectl run` command:
```shell
> kubectl run [DEPLOYMENT_NAME] \
			--image [IMAGE]:[TAG]
			--replicas 3
			--labels [KEY]:[VALUE]
			--port 8080
			--generator deployment/apps.v1 # api version to be used
			--save-config # saves the yaml config for future use
```
3. GKE workloads menu in GCP Console (also can view resulting yaml config file)

#### Inspection

```shell
> kubectl get deployment [DEPLOYMENT_NAME]
> kubectl describe deployment [DEPLOYMENT_NAME]
```

Save deployment configuration:
```shell
> kubectl get deployment [DEPLOYMENT_NAME] -o yaml > this.yaml
```

#### Scaling

```shell
> kubectl scale deployment [DEPLOYMENT_NAME] -replicas=5 # manual scaling
> kubectl autoscale deployment [DEPLOYMENT_NAME] \
				--min=5 \
				--max=15 \
				--cpu-percent=75 # autoscale based on cpu threshold
```

Autoscaling creates `horizontal pod autoscaler` object. Autoscaling has a
thrashing problem, that is when the target metric changes frequently, which
results in frequent up/down scaling. Use
`--horizontal-pod-autoscaler-downscale-delay` flag to control this behavior (by
specifying a wait period before next down scale; default is 5 minute delay.

#### Updating

1. `kubectl apply -f [DEPLOYMENT_NAME]` with updated config file
2. `kubectl set` command - can change pod template, specifications for
deployment (image, resources, selector values)
```shell
> kubectl set image deployment [DEPLOYMENT_NAME] [IMAGE] [IMAGE]:[TAG]
```
3. `kubectl edit deployment` command, which opens specification file in vim and
applies changes once you save and exit
4. GCP Console

Strategies:
+ rampt strategy - neww Pods are launched in new ReplicaSet, then old ones are
deleted; ensures availability, but doesn't provide control of traffic to old and
new instances.
+ rolling update strategy provides `maxUnavailable` and `maxSurge` fields to
control number of unavailable pods and number of concurrenty pods in a new
ReplicaSet; both can be specified by a number of pods or percentage (percentage
is taken from the total number - old and new pods)
```yaml
[...]
kind: deployment
spec:
  replicas: 10
  strategy:
    type: RollingUpdate
    rollingUpdate:
      maxSurge: 5
      maxUnavailable: 30%
[...]
```
`minReady` - wait until Pod is considered available (defaults to 0),
`progressDeadlineSeconds` - wait period before deployment reporst that it has
failed to progress
+ blue/green deployment creates completely new delpoyment and ReplicaSet of an
application also changing the app's version; traffic can be redirected using
kubernetes services; good for testing, disadvantage in doubled resources
+ canary deployment is based on blue/green, but traffic is shifted gradually to
a new version; this is achived by avoiding specifying app's version and just by
creating pods of a new version
+ `Recreate` strategy simply deletes old pods before creating new ones (good
when clean break is needed)

Rolling back is done using `kubectl rollout undo deployment [DEPLOYMENT_NAME]`
command. It would simply rollout to a previos version. Use revision number
(`--to-revision=2` flag) to specify a revision to roll back. To expect changes
use `kubectl rollout history deployment [DEPLOYMENT_NAME] --revision=2` command.
By default last 10 ReplicaSets details are retained, can be changed in revison
history limit under deployment specification.

A deployment's rollout is triggered if and only if the deployment's Pod template
(that is, .spec.template) is changed, for example, if the labels or container
images of the template are updated. Other updates, such as scaling the
deployment, do not trigger a rollout.

---

### Replication

Replication Controllers perform a similar role to the combination of ReplicaSets
and Deployments, but their use is no longer recommended. Deployments provide a
helpful "front end" to ReplicaSets.

---

### StatefulSet

Better choice for applications that maintain local state. Unlike Deployment,
Pods in StatefulSet have persistent identities with stable network identity and
persistent disk storage.

---

### DaemonSet

Good choice to run certain Pods on all the nodes within the cluster or on a
selection of nodes. DaemonSet ensures that a specific Pod is always running on
all or some subset of the nodes. If new nodes are added, DaemonSet will
automatically set up Pods in those nodes with the required specification. The
word "daemon" is a computer science term meaning a non-interactive process that
provides useful services to other processes. A Kubernetes cluster might use a
DaemonSet to ensure that a logging agent like fluentd is running on all nodes in
the cluster.

---

### Job

The Job controller creates one or more Pods required to run a task. When the
task is completed, Job will then terminate all those Pods. A related controller
is CronJob, which runs Pods on a time-based schedule.

# Manifest file

File describing objects that you Kubernetes to create and maintain. Can be json
or yamn format.

Required fields:
+ `apiVersion` - Kubernetes API version
+ `kind` - type of object
+ `metadata` - identifies the object with `name` and optionally `labels`
(key-value pair that can be tagged during or after creating)
+ `spec`

# Volumes

# Services

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

Example:
```yaml
apiVersion: v1
kind: Service
metadata:
  name: nginx
spec:
  type: LoadBalancer
  selector:
    app: nginx
  ports:
  - protocol: TCP
    port: 60000
    targetPort: 80
```

```shell
> kubectl apply -f [SERVICE_FILE] # deploy service
> kubectl get service [SERVICE_NAME] # view service
```

`sessionAffinity` field can be used to ensure all subsequent connection go to
the same pod (usefull for canary deployment):
```yaml
apiVersion: v1
kind: Service
metadata:
  name: nginx
spec:
  type: LoadBalancer
  sessionAffinity: ClientIP
  selector:
    app: nginx
  ports:
  - protocol: TCP
    port: 60000
    targetPort: 80
```

# kubectl

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

## Examples

```shell
> kubectl top nodes # info on nodes status
> kubectl cp ~/test.html $my_nginx_pod:/usr/share/nginx/html/test.html # copy files
> kubectl apply -f ./new-nginx-pod.yaml # apply manifest file and start a container
```

## Introspection

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


# Additional notes

+ https://cloud.google.com/kubernetes-engine/
+ `gcloud container clusters create $my_cluster --num-nodes 3 --zone $my_zone
--enable-ip-alias` - create cluster in GKE, more info at
[https://cloud.google.com/sdk/gcloud/reference/container/clusters/create]

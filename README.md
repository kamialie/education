# Kubernetes

# Contents

+ [General](#general)
	+ [namaspace](#namespace)
+ [Objects](#objects)
	+ [Controller](#controller)
		+ [Deployment](#deployment)
		+ [Job](#job)
		+ [CronJob](#cronjob)
+ [Manifest file](#manifest-file)
+ [Management](#management)
	+ [Scaling](#slaling)
	+ [Placement](#placement)
	+ [Helm](#helm)
+ [Volumes](#volumes)
+ [Networking](#networking)
	+ [Services](#services)
	+ [Ingress](#ingress)
	+ [Security](#security)
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

Pod is the smallest deployable object (not container). Pod embodies the
environment where container lives, which can hold one or more containers. If
there are several containers in a pod, they share all resources like networking
and storage. Kubernetes assigns unique IP address to a pod, which containers
inside share among each other (containers in a pod can also communicate through
localhost).

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

Job controller tracks the task completion and recreates pod if run was not
successful. Removes all pods upon completion.

`activeDeadlineSeconds` option can be used to specify an active deadline period
fot the job to finish. Has precedence over `backoffLimit` option.

Failed pods are recreated with an exponentially increasing delay: 10, 20, 40...
seconds, to a maximum of 6 minutes.

+ inspection
```shell
> kubectl describe job [JOB_NAME]
> kubectl get pod -l [job-name=my-app-name] # label selector
> kubectl scale job [JOB_NAME] --replicas [VALUE] # scale job
```
+ deletion
```shell
> kubectl delete -f [JOB_FILE]
> kubectl delete job [JOB_NAME] # all pods are deleted
> kubectl delete job [JOB_NAME] --cascade false # retain job pods
```

#### Non-parallel

Creates only one pod at a time; job is completed when pod terminates
successfully or, if completion counter is defined, when the required number of
completions is performed.

Non-parallel job example:
```yaml
apiVersion: batch/v1
kind: Job
metadata:
  name: pi
spec:
  template:
    spec:
	  containers:
	  - name: pi
	    image: perl
		command: ["perl", "Mbignum=bpi", "-wle", "print bpi(2000)"]
	  # can also be specified as onFailure which would restart container, in
	  # this case new Pod is created upon failure
	  restartPolicy: never
  # number of retries after which Job is considered to have failed entirely,
  # defaults to 6
  backoffLimit: 4
```

```shell
> kubectl run pi	--image perl \
					--restart Never -- perl -Mbignum -wle 'print bpi(2000)'
```

#### Parallel

Parallel job can launch multiple pods to run the same task. There are 2 types of
parallel jobs - fixed task completion count and the other which processes a work
queue.

Work queue is created by leaving `completions` field empty. Job comtroller
launches specified number of pod simultaneously and waits until one of them
signals successfull completion. Then it stops and removes all pods.

In a situation of a job with both completion and parallelism options set, the
controller won't start new containers if the remaining number of completions is
less that parallelism value.

Parallel job example:
```yaml
apiVersion: batch/v1
kind: Job
metadata:
  name: pi
spec:
  completions: 3
  parallelism: 2
  template:
    spec:
[...]
```

---

### CronJob

Kubernetes object that creates job in repeatable manner to a defined schedule.

By default looks at the time a job was scheduled and when failed atttempts reach
100 an error is logged and no more jobs are scheduled.

Depending on how frequently jobs are scheduled and how long it takes ti finish a
job, CronJob might end up executing more than one job concurrenlty.
`concurrencyPolicy` option is used to control that behavior. Available options
are `Allow`,`Forbid`, `Replace`.

Kubernetes retains number of successful and failued jobs in history, which is by
default 3 and 1 respectively. Options `successfulJobsHistoryLimit` and
`failedJobsHistoryLimit` may be used to control this behaviour.

Example:
````yaml
apiVersion: batch/v1
kind: CronJob
metadata:
  name: my-app-job
spec:
  schedule: "*/1 * * * *"
  startingDeadlineSeconds: 3600 # optional, check later
  concurrencyPolicy: Forbid
  successfulJobsHistoryLimit: 3
  failedJobsHistoryLimit: 1
  jobtemplate:
    spec:
	  template:
	    spec:
[...]
```

Actions:
```shell
> kubectl apply -f [FILE]
> kubectl describe cronjob [NAME]
> kubectl delete cronjob [NAME]
```

# Manifest file

File describing objects that you Kubernetes to create and maintain. Can be json
or yamn format.

Required fields:
+ `apiVersion` - Kubernetes API version
+ `kind` - type of object
+ `metadata` - identifies the object with `name` and optionally `labels`
(key-value pair that can be tagged during or after creating)
+ `spec`

# Management

Node pool is a subset of nodes within cluster that have same configuration. Node
pool uses NodeConfig specification. Default node pool is created upon cluster
creation. Later, custom node pools can be added.

## Scaling

Each node has a Kubernetes node label, which is node pool it belongs to. In GCP
Console node pool size represents number of nodes withtin a zone (that is
increase from 3 to 6 in dual-zone node pool will result in 12 total nodes).

Downscaling does not differentiate between nodes running pods and those that do
not (terminates randomly). Thus, if pods aren't managed by replication
controller, they won't be restarted.

GKE auto-scalar adds a node when scheduler can not schedule a pod due to
resource constraints.

Scale cluster using `gcloud` command:
```shell
> gcloud container clusters resize [PROJECT_NAME]	--node-pool [NODE_POOL_NAME] \
													--size 6
```

gcloud commands for autoscaling:
```shell
# create a cluster with autoscaling enabled
> gcloud container clusters create [CLUSTER_NAME]	--num-nodes 30 \
													--enable-autoscaling \
													--min-nodes 15 \
													--max-nodes 50 \
													[--zone COMPUTE_ZONE]
# add a node pool with autoscaling enabled
> gcloud container node-pools create [POOL_NAME]	--cluster [CLUSTER_NAME] \
													--enable-autoscaling \
													--min-nodes 15 \
													--max-nodes 50 \
													[--zone COMPUTE_ZONE]
# enable autoscaling for an existing node pool
> gcloud container clusters update [CLUSTER_NAME]	--enable-autoscaling \
													--min-nodes 1 \
													--max-nodes 10 \
													--zone COMPUTE_ZONE \
													--node-pool [POOL_NAME]
# disable autoscaling for an existing node pool
> gcloud container clusters update [CLUSTER_NAME]	--no-enable-autoscaling \
													--node-pool [POOL_NAME] \
													[--zone [COMPUTE_ZONE] \
													--project [PROJECT_ID]]
```

Get info on horizontal autoscalar:
```shell
> kubectl get hp
> kubectl describe horizontalpodautoscaler [DEPLOYMENT]
> kubectl get horizontalpodautoscaler [DEPLOYMENT] -o yaml
```

## Downscaling

GKE auto-scalar may delete a node if it is underutilized and running Pods can be
run on another node.

GKE auto-scalar can scale down unless any of  below conditions are true:
+ scale up event is pending (if scale up event happens during scale down
process, latter is not executed)
+ pod is not managed by controller
+ pod has local storage
+ pod is restricted by constraint rules (such as running on any other node)
+ pod that has `safe-to-evict` annotation set to false (direct setting at pod
level that tells auto-scalar that pod can not be evicted)
+ pod has restrictive PodDisruptionBudget (can specify number of controller
replicas that must be available at any time)
+ node has `scale-down-disabled` annotation set to true

For each of the remaining nodes auto-scalar adds up total CPU and memory for
pods. If total is less that 50% it starts monitoring that node for the next 10
minutes. If total remains less that 50%, it is deleted.

Some of the node pools can be scaled down to zero, while cluster is required to
have at least one node to run system pods.

## Placement

In Kubernetes placement can be controlled by labels and tains on nodes, and node
affinity rules and toleration in deployment specification.

When creating pods, you can optionally specify CPU and RAM each container needs,
so that scheduler can make better decision on where to run it.

For a pod to run on a specific node, that node must match all the labels present
under the nodes selector field in a pod. Node selector is a pod specification
field that specifies one or more labels. If label on a node has changed, it
doesn't affect running pods. Node selector is only used during pod scheduling.
Some node labels are assigned automatically by kubernetes.

```yaml
apiVersion: v1
kind: Pod
metadata:
  name: nginx
  labels:
    env: test
spec:
  containers:
  - name: nginx
    image: nginx
	imagePullPolicy: IfNotPresent
  nodeSelector:
    beta.kubernetes.io/arch=amd64
[...]

apiVersion: v1
kind: Node
metadata:
  name: node1
  labels:
    beta.kubernetes.io/arch=amd64
[...]
```

Affinity and anti-affinity can be viewed as preferences over hard requirements.
It is possible to specify either `nodeAffinity` or `podAffinity`. Affinity is
denoted either by `requiredDuringSchedulingIgnoredDuringExecution` (hard
requirement) or `preferredDuringSchedulingIgnoredDuringExuction` (preference).

Required example:
```yaml
apiVersion: v1
kind: Pod
[...]
spec:
  affinity:
    nodeAffinity:
	  requiredDuringSchedulingIgnoredDuringExecution:
	    nodeSelectorTerms:
		  - matchExpression:
		    - key: accelerator-type
			operator: In
			values:
			  - gpu
			  - tpu
```
Multiple `matchExpression` can be specified, which all should be matched
(logical AND). Operator choices - `notIn`, `exists`, `.notExists`, `gt` (greater
that), `lt` (less that).

Preference example:
```yaml
apiVersion: v1
kind: Pod
[...]
spec:
  affinity:
    nodeAffinity:
	  preferredDuringSchedulingIgnoredDuringExuction:
	    - weight: 1
	    preference:
		  - matchExpression:
		    - key: accelerator-type
			operator: In
			values:
			  - gpu
			  - tpu
```
Weight ranges from 1 to 100 (100 being closest to requirement, but is still a
preference).

Pod anti-affinity example(checked against already running pods):
```yaml
apiVersion: v1
kind: Pod
[...]
spec:
  affinity:
    podAntiAffinity:
	  preferredDuringSchedulingIgnoredDuringExuction:
	    - weight: 100
	    podAffinityTerm::
		  labelSelector:
		    - matchExpression:
		      - key: app
			  operator: In
			  values:
			    - webserver
		  topologyKey: failure-domain.beta.kubernetes.io/zone
```
`topologyKey` can be used to set rules at a higher level than just nodes, for
example, zone or region. Example above sets affinity sets preferens not to be
scheduled in the same zone as a pod running webserver.

By contrast to affinity, taints are used on nodes that affect whole cluster. To
taint a node a command below is used (multiple taints can be applied, running
pods can be evicted):
```shell
> kubectl taint nodes node1 key=value:NoSchedule
```

A toleration is a mechanism that allows a Pod to counteract the effect of a
taint that would otherwise prevent the Pod from being scheduled or continue to
run on at node. When operator is `equal`, the `value` must also be equal, while
`exists` operator requires only `key` and `effect` must match for toleration to
apply.

```yaml
tolerations:
- key: "key"
 operator: "Equal"
 value: "value"
 effect: "NoSchedule"

tolerations:
- key: "key"
 operator: "Exists"
 effect: "NoSchedule"
```

# Helm

Open-source package manager for Kubernetes (in the same way as `apt-get` for
Linux). It operates with `charts` that are Kubernetes objects and packages
(easily created, version, shared and published). Charts manage the deployment of
complex application like what parameters are needed for the to work, for
example, how many instances are needed or resource constraints should be used.

Helm consists of command line client `helm` and server `tiller`, which runs
within Kubernetes cluster and interracts with Kubernetes api-server.


# Volumes


# Networking

Each pod in a cluster has a unique IP address.

VPC native GKE clusters automatically create an alias IP range to reserve
approximately 4000 IP addresses for cluster-wide services that you may create
late. VPC-native GKE cluster also creates a separate alias IP range for your
pods with a /14 block, which is further divided among nodes by /24 blocks
(that's around 1000 nodes with 100 pods on each).

## Services

Services provide durable endpoints to Pods. It is a static IP address that
represents a service or a function (you can group several Pods into one that
provide same service). Thus, dynamically created Pods can have persistent
endpoint for other services to communicate with. By default, the master assigns
a virtual IP address also known as a cluster IP to the service from internal IP
tables. With GKE, this is assigned from the clusters VPC network (specifically
reserved to services).

A Kubernetes service is an object that creates a dynamic collection of IP
addresses called end points that belong to pods matching the services labeled
selector.

To get service quickly ask Kubernetes to expose a deployment.

### Service discovery

Services can be discovered through pod's environment variables, Kubernetes DNS
and istio.

Environment variables are added only at the time of pod's creation and are not
updated later on. This forces user to recreate a pod, if service is created or
changed after pod's creation.

In Kubernetes DNS has an option add-on, while Google Kubernetes Engine has it
preinstalled (kube-dns). Kube-dns is automatically creates set of DNS records
for any new services and Kubernetes is configured to use it for DNS resolutions.
Pods in the same namespace can use service's short name, while pods in other
namespaces use FQDM.

Open-source service mesh istio is infrastructure layer that is configurable for
micro-services applications. Istio is a service mesh to aid in service
discovery, control, and visibility in your micro-services deployments.

---

### Types of services

`ClusterIP` exposes the service on an IP address that is only accessible from
within this cluster. This is the default type (not specifying type of service
defaults to `ClusterIP`). If service is created before corresponding pods, they
get hostname and IP address as environment variables.

```yaml
apiVersion: v1
kind: Service
metadata:
  name: my-service
spec:
  type: ClusterIP
  selector:
    app: Backend
  ports:
  - protocol: TCP
    port: 80
    targetPort: 9367
```

`NodePort` exposes the service on the IP address of each node in the cluster,
at a specific port number, making it available outside the cluster. Built on top
of `ClusterIP` service. `nodeport` option is set automatically from the range
30000 to 32767 or can be specified by user if falls within range.

```yaml
apiVersion: v1
kind: Service
metadata:
  name: my-service
spec:
  type: NodePort
  selector:
    app: Backend
  ports:
  - protocol: TCP
    nodePort: 30100
    port: 80
    targetPort: 9367
```

`LoadBalancer` exposes the service externally, using a load balancing service
provided by a cloud provider or add-on. Builds on the `ClusterIP` service.

With GKE it is implemented using GCP's network Load Balancer. GCP will assign
assign static IP address to load balancer and it will further direct traffic to
nodes (random). `kube-proxy` will choose random pod, which may reside on a
different node to ensure even balance (this is default). Respond will take same
route back. Use `externalTrafficPolicy: Local` option to disable this behaviour
and enforce `kube-proxy` to direct traffic to local pods.

Example:
```yaml
apiVersion: v1
kind: Service
metadata:
  name: nginx
spec:
  type: LoadBalancer
  externalTrafficPolicy: Local
  selector:
    app: nginx
  ports:
  - protocol: TCP
    port: 80
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

## Ingress

Ingress resource Operates one layer higher than resources (service for
services). In fact it is a collection of rules that direct external traffic to a
set of services inside a cluster. Delivers traffic to either `NodePort` or
`LoadBalancer` service.

In GKE's Kubernetes' ingress resources are implemented using Cloud Load
Balancer.

Simple example:
```yaml
apiVersion: extenstions/v1beta
kind: Ingress
metadata:
  name: test-ingress
spec:
  backend:
    serviceName: demo
    servicePort: 80
```

Example belows introduces host and path resolutions that ingress can you to
direct traffic to different locations (that is multiple hostnames for the same
IP address). Traffic that did not match any rules will be directed to default
backend, which user can customize as well.
```yaml
apiVersion: extenstions/v1beta
kind: Ingress
metadata:
  name: test-ingress
spec:
  rules:
  - host: demo1.example.com
    http:
	  paths:
	  - path: /examplepath
        backend:
          serviceName: demo
          servicePort: 80
  - host: lab.user.com
    http:
	  paths:
	  - path: /labpath
        backend:
          serviceName: lab1
          servicePort: 80
```

```shell
# update ingress
> kubectl edit ingress [NAME]
# or by completely replacing configuration
> kubectl replace -f [FILE]
```

## Container-native load balancing

GKE also provides Network Endpoint Groups data model that represents IP to pod
endpoints. Requires GKE cluster to operate in VPC-native mode. Connection is
made directly from load balancer to pods.

## Security

Network policy is a set of pod-level firewall rules restricting access to other
pods and services.

By default network policy is disabled in GKE. In order to enabled it one must
have at least 2 nodes of n1-standard-1 or higher configuration. Must be applied
at the creation time, otherwise nodes must be restarted.

```shell
# enabling at the time of cluster creation
> gcloud container clusters create [NAME] --enable-network policy

# two-step process for an existing cluster
> gcloud container clusters update [NAME] --update-addones-NetworkPolicy=ENABLED
> gcloud container clusters update [NAME] --enable-network policy
```

If `podSelector` isn't supplied, network policy is applied to all pods in the
namespace. If no `policyType` is specified, default is `Ingress`. `Ingress` -
incoming traffic, `Egress` - outcoming traffic. `Ingress` can specify source by
`ipBlock`, `namespaceSelector` or `podSelector`.
```yaml
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: demo-network-policy
  namespace: default
spec:
  podSelector:
    matchLabels:
	  role: demo-app
  policyType:
  - Ingress
  - Engress
  igress:
  - from:
    - ipBlock:
	  cidr: 172.17.0.0./16
	  except:
	  - 172.17.1.0/24
	- namespaceSelector:
	    matchLabels:
		  project: myproject
	- podSelector:
	    matchLabels:
		  role: frontend
    ports:
	- protocol: TCP
	  port: 6345
  egress:
  - to:
    - ipBlock:
	  cidr: 10.0.0.0/24
	ports:
	- protocol: TCP
	  port: 5978
```

Default network policies:
```yaml
[...]
metadata:
  name: default-deny-incoming
spec:
  podSelector: {}
  policyTypes:
  - Ingress
[...]
metadata:
  name: default-deny-outgoing
spec:
  podSelector: {}
  policyTypes:
  - Engress
[...]
metadata:
  name: allow-all-incoming
spec:
  podSelector: {}
  policyTypes:
  - Ingress
  igress:
  - {}
[...]
```

# kubectl

First `kubectl` should be configured to be able to access the cluster. Config
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

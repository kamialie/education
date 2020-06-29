# Kubernetes

## Contents

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

Everything in Kubernetes is represented by an object with state and attributes
that user can change. Each object has two elements: object spec (desired state)
and object state (current state). Pod is the smallest deployable object (not
container). Pod embodies the environment where container lives, which can hold
one or more containers. If there are several containers in a pod, they share all
resources like networking and storage. Kubernetes assigns unique IP address to a
pod, which containers inside share (containers in a pod can also communicate
through localhost).

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
+ kuberlet (runs on nodes) - kubernetes agent that kube-APIserver talks to
+ kube-proxy (runs on nodes) - provides network connectivity among pods in a
cluster

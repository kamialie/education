# Google Cloud Platform

+ [Compute Engine](#compute-engine)
+ [Storage options](#storage-options)

## Hierarchy

organization node -> folders -> projects -> resources

Project identifying attributes:

+ project ID (globally unique, chosen by you, immutable)
+ project name (not necesseraly unique, chosen by you mutable)
+ project number (globally unique, assigned by GCP, immutable)

Projects inherit IAM (Identity and Access Management) from folders where they
reside.

## IAM (Identity and Access Management)

+ "who"
	+ primitive - are applied to a project and affect all resources on a project
	(owner, editor and viewer roles)
	+ predifined - can affect project, folder or entire organization
	+ custom - can only be used at project or organizational levels (can't be
	used at the folder level)
+ "can do what"
+ "on what resources"

Service account can be created for a particular VM to comunicate with GCP.

## Interact with GCP

1. Console - web based administrative user interface
2. Shell and SDK - SDK includes command-line tools for Cloud products (gcloud,
gsutil(Cloud Storage), bq (BigQuerry)); can be accessed through Console in web
browser or installed on machine
3. Console Mobile App
4. REST-based API - most are turned off by default, associated with limits and
quotas; Console includes `APIs Explorer` to learn about API interactively.

# Compute Engine

Disk options:
+ standard or SSD
+ local SSD (really fast, but is deleted upon VM deletions)

Images:
+ Linux
+ Windows
+ custom imported

Preemptive VM - Compute Engine can stop if resources are needed elsewhere; job
must be configured to be able to stop and resume

Virtual Private Cloud (VPC)
+ belong to GCP project
+ provides isolation, segmenting and resource connection
+ are global (across regions), subnets have regional scopre
+ routing table to forward traffic
+ global distributed firewall

# Storage options

## Cloud Storage

hight performance, internet scale

uses object storage technology, that is data is accessable via keys, here via URL

Cloud storage objects are immutable
ACL (Access Control List) - enhances permissions on buckets (of data), defines
scope, who can perform specified actions (user or group), and permission, what
action can be performed (read or write)

provides optional versioning and versioning policy (keep most recent or delete
older that, etc)

Cloud Storage Classes:
+ regional - frequently used within region
+ multi-regional - most frequently accessed
+ nearline - long-tail content, backups
+ coldline - archiving, disaster recovery

Consider using Cloud Storage if you need to store immutable blobs larger than 10
megabytes such as large images or movies.

## Cloud SQL and Cloud Spanner

MySql and PosteSQL database support

Cloud spanner offers more more horizontal scalability, provides transactional
consistency at global scale and uses ANSI SQL 2011 with extensions.

## Google BigTable

NoSQL big data database service.

Can be viewed as persistent hatch table.

Accessed using HBase API (Hadoop).

Consider using if you need to store a large amount of structured objects.

## Cloud Data Store

Another NoSQL service.

Designed for application backends.

Supports transactions and offers free daily quotas.

Can span App Engine and Compute Engine applications.

Consider using Cloud Datastore if you need to store structured objects or if you
require support for transactions and SQL-like queries.

# Containers, Kubernetes and Kubernetes Engine

Kubernetes is a open-source container orchestrator. Containers can be build by
docker images or any other (like Cloud Build offered by google). Kubernetes
Engine lets one create a cluster (set of nodes that are virtual machines) to run
containers in it - can be created from UI or Console (sample command `gcloud
container cluster craete k1`).

Pod is smallest delpoyable unit in Kubernetes. It's common to have only one
container per pod, but it's also possible multiple stacked into one pod. Each
pod has a unique IP and set of ports. To see running pods - `kubectl get pods`.
To connect load-balancer - `kubectl expose deployments nginx --port=80
--type=LoadBalancer`. To show public IP of a service (group of pods with a
stable IP) - `kubectl get services`.

# App Engine

PaaS - platform as a service, provides everything that is needed to run an app -
databases, in-memory caching, load balancing, etc. No need to provision neither
servers nor containers. Automatically scales application based on incomming
traffic.

Has two environments - Standard and Flexible.

## Standard

Runs in sandbox which is able to run on any hardware/software, but enforces
constraints - can't write to disk, install 3rd party tools, perform network
requests through App Engine. 60s request limit.

Pricing can drop to zero with automatic shutdown.

## Flexible

No sandbox constraints, can access App Engine resources. Let's user specify a
container where app is going to run (this container will run in Google VM).

Able to write to local disk, access via ssh, install 3rd party tools and perform
network requests without App Engine.

Pricing is determined by paying for allocated resource per hour.

# Development in the cloud

## Cloud Source Reposilories

Host your local Git without need to maintain it (to keep code private to GCP
project)

## Cloud Functions (beta)

general purpose funciton that can be triggered by an event and execute without
runtime or server (just pay for function execution)

## Delpoyment Manager

Takes declarative template (either yaml or python) that describes the Google Cloud environment and
applies them.

## Stackdriver

Monitoring, debugging, logging, error reporting, tracing tool.

# Big Data Platform

## DataProc

Cloud Dataproc is a fast, easy, managed way to run Hadoop, Spark, Hive, and Pig
on Google Cloud Platform.

Great for data of known size and if you want to manage the size of cluster
yourself.

## Dataflow

Great for real-time or data of unpredictable size and rate. Automatically
manages underlying processing resources. Can be used to create a Dataflow
pipelines.

ETL - Extract, Trasform, Load model

## BigQuery

Great for a exploring a vast sea of data mode.

## Cloud Pub/Sub

Cloud messaging service. Can configure publishers and subscribers.

# Notes

+ [Bitname LAMP stack](https://docs.bitnami.com/google/infrastructure/lamp/)

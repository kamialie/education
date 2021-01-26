# AWS

# Contents

+ [EC2](#ec2)
+ [VPC](#vpc)
+ [Load balancing](#load-balancing)
+ [CLI](#cli)

## EC2

EC2 (Elastic Compute Cloud) - virtual machine manager (same as Google Cloud Engine)
EBS (Block Store) - persistent storage volumes
Elastic IP - static IPs assigned to dynamic cloud computing

AMI (Amazon Machine Image) - basically OS image that serves as a base to vm instance.
The basic and free of charge is Amazon Linux AMI(outdated).

2 kinds of EC2 storage:
1. Instance store - hard disk on host (lost if vm is shut down)
2. EBS - network drive

EC2 pricing models:
+ on demand instances - pay as you go
+ reserved instances - pay upfront (1-3 years, thus, save some)
+ spot instance - bid if available (spare servers), may terminate with
2 minute notice

EC2 tenancy:
+ dedicated
	+ dedicated instance - your EC2 stay on host that is only used by you
	(physical host may change)
	+ dedicated host - you get physical host
+ multi-tenant

Elastic IP - that is static IP that can be attached to any vm (don't pay if
attached to a running instance??)

Termination protectiona - toggle in `Actions->Instance settings->Change
 Termination Protection` that disables the confirmation button on termination
command (extra layer, bla-bla).

Metadata is accessable from inside in the following url(can also appent
individial tag to access the value):
```shell
$ curl http://169.254.169.254/latest/meta-data/
```

`User data section` of `3. Configure instance` can be used to run command on
start-up

CloudWatch - provides EC2 monitoring

To enable access to other aws resources attach roles with corresponding
permissions (`Actions->Instance settings->Attach/Replace IAM Role`)

Interfaces to EC2:
+ Management Consolse
+ CLI
+ CloudFormation
+ SDK
+ REST API

### CloudWatch agent

[quickstart](https://docs.aws.amazon.com/AmazonCloudWatch/latest/logs/QuickStartEC2Instance.html)

[install](https://docs.aws.amazon.com/AmazonCloudWatch/latest/monitoring/install-CloudWatch-Agent-on-EC2-Instance.html)

[configure](https://docs.aws.amazon.com/AmazonCloudWatch/latest/monitoring/create-cloudwatch-agent-configuration-file-wizard.html)

start the agent:
```shell
$ sudo /opt/aws/amazon-cloudwatch-agent/bin/amazon-cloudwatch-agent-ctl -a fetch-config -m ec2 -c file:/opt/aws/amazon-cloudwatch-agent/bin/config.json -s
```

---

## VPC

Virtual Private Cloud

AWS default VPCs have /16 (65536 hosts) subnet mask. Default subnets have /20 (4096 hosts) mask.

VPC can span multiple availability zones, but subnet is always mapped to a single one.

VPC peering is enabling resources from one VPC to communicate with another VPC.
Possible between account too. Must not have overlaping IP ranges and must be
located in the same region.

Classless Inter-Domain Roiting (CIDR) is simply an IP range.

Subnets are required to launch resources on the VPC.

Internaet gateway requirements:
+ internet gateway must be attached to VPC
+ instances must have either public or elastic IP
+ subnet route table must point to the internet gateway
+ network access control and security groups rules must be configured to allow
in/out traffic

Every subnet must be associated with a route table (only one); route table can
have multiple subnets associated with it.

Every subnet must be associated with an ACL, if not, it is associated with
default ACL. Again, subnet can be associated with only one ACL, while one
ACL can have multiple subnets associated with it.

---

### NAT

NAT device is used to provide internet access to resources in private subnet
(but prevent internet from initiating access).
+ NAT gateway (recommended)
+ NAT instance (runs as an instance in your subnet)

NAT gateway must be launch in public subnet (to have internet access) and have
an elastic IP (can be selected at launch time). Then update route table of
private subnet to direct traffic to NAT gateway.

---

Security groups allows al outbound traffic by default. Rules are only
permissive. SG are stateful (response traffic for outbound request is always
allowed, regardless of inbound rules). Good practice to maintain separate
security group for SSH access. `time out` response most likely means it is
a security group issue, while `connection refused` means it is an application
issue. Other security groups can be referenced as a source when setting a
security group - that means instances that have that security group attached
are authorized to access newly created security group.

Network ACL (Access Control List) - optional security for VPC that acts like
firewall for one or more subnets. Sits between subnet and route table. `Star`
(asterics) rule means if traffic doesn't match any other rule, it is denied;
this rule can not be removed. ACL is a list of numbered rules, which are
applied from lowest to highest (recommended increment is 100, so that there is
space for other rules to add later); when lower number rules is matching, it is
immediately applied, regardless of higher numbered rules. ACL us stateless -
responses for outbound traffic are subject to inbound rules.

It is recommended to use security groups for white-listing traffice and ACLs
for blacklisting traffic.


Elastic IP has associated costs if it is not attached to a resource.

---

## Load balancing

Provides:
+ spread load
+ expose single point of access
+ handle failures (healthcheck)
+ ssl termination (talks https to clients and http to backend)
+ enforce stickiness cookies
+ separate public from privatee traffic

types:
+ Classic LB (2009, v1)
+ Application LB (2016, v2)
+ Network LB (2017, v2)

Application LB (layer 7) allows to load balance multiple HTTP applications across
machines (target groups) or multiple applications on the same  machine
(containers). Load balancing can be done based on route or hostname in URL.
Great for microservices and container-based application. ALB supports HTTP/S and
websocket protocols. Applications don't see real IP of the client, it is rather
inserted as a X-Forwarded-For header.

Network LB (layer 4) forwards TCP traffic, support for elastic IP, highest
performance.

Load balancers (any type) have a static host name (do not resolve underlying IP).
LB 503 error indicates capacity issue or no registered targets. At connection
error check security groups.

---

## CLI

[docs](https://docs.aws.amazon.com/cli/index.html)

create an s3 bucket (`mb` stands for make bucket):
```shell
$ aws s3 mb s3://test-bucket
```

configure credentials (will be then asked `Access Key ID` and
`Secret Acess Key`, which are obtained from `IAM->User->Create User` page and
can be downloade as csv); hard-coding way, not preferred (use roles instead):
```shell
$ aws configure
$ cat ~/.aws/credentials
```

create roles in the same page as `IAM->User`, then attach to running ec2 or
select when creating it; now access token are are automaticaly rotated and
can be accessed as this:
```shell
$ curl 169.254.169.254/latest/meta-data/iam/security-credentials/
$ curl 169.254.169.254/latest/meta-data/iam/security-credentials/{output from previous command}/
```

create an ec2 instance (first do `aws configure` and provide just the default
region, so you don't have to provide it for every commande):
```shell
$ aws configure
$ aws ec2 run-instances --image-id {take from images page} --instance-type t2.micro
```

list instances(shows full info in json format):
```shell
$ aws ec2 describe-instances
```

query specific info from previous command (second line also utilizes filtering):
```shell
$ aws ec2 describe-instances --query 'Reservations[].Instances[].PublicIpAddress'
$ aws ec2 describe-instances --query 'Reservations[].Instances[].PublicIpAddress' --filters "Name=platform,Values=windows"
```

stop/terminate an instance:
```shell
$ aws ec2 stop-instances --instance-ids {id}
$ aws ec2 terminate-instances --instance-ids {id}
```

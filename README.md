# AWS

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

## CloudWatch agent

[quickstart](https://docs.aws.amazon.com/AmazonCloudWatch/latest/logs/QuickStartEC2Instance.html)

[install](https://docs.aws.amazon.com/AmazonCloudWatch/latest/monitoring/install-CloudWatch-Agent-on-EC2-Instance.html)

[configure](https://docs.aws.amazon.com/AmazonCloudWatch/latest/monitoring/create-cloudwatch-agent-configuration-file-wizard.html)

start the agent:
```shell
$ sudo /opt/aws/amazon-cloudwatch-agent/bin/amazon-cloudwatch-agent-ctl -a fetch-config -m ec2 -c file:/opt/aws/amazon-cloudwatch-agent/bin/config.json -s
```

## AWS CLI

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

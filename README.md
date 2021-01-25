# Terraform 🛕

Terraform files can be in terraform or in json format (including var files).

# Contents

+ [Installation](#installation)
+ [First run](#first-run)
+ [State](#state)
+ [Output](#output)
+ [Variables](#variables)
+ [Meta arguments](#meta-arguments)
	+ [lifecycle](#lifecycle)
+ [Expressions](#blocks)
	+ [dynamic](#dynamic)
+ [Modules](#modules)
+ [Tricks](#tricks)
+ [AWS](#aws)
	+ [aws resources](#aws-resources)

## Installation

+ mac via Homebrew
```shell
$ brew tap hashicorp/tap
$ brew install brew install hashicorp/tap/terraform
```

Autocomplete for bash or zsh (run, then open new session):
```shell
$ terraform -install-autocomplete
```

---

## First run

```terraform
terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 2.70"
    }
  }
}

provider "aws" {
  profile = "default"
  region  = "us-west-2"
}

resource "aws_instance" "example" {
  ami           = "ami-830c94e3"
  instance_type = "t2.micro"
}
```

+ `terraform` block is used to provide list of providers to download from
[Terraform registry](https://registry.terraform.io/).
[More info](https://www.terraform.io/docs/configuration/provider-requirements.html)
+ `provider` block configures the named provider; explicit `default` will look
at `~/.aws/credentials` file
+ `resource` block defines a piece of infrastructure; first argument is
provider-specific resource type, while second is the resource name; [provider
documentation](https://www.terraform.io/docs/providers/index.html) to help fill
in required and optional fields for resource

Then initialize the directory (that is download defined plugins for providers)
with `terraform init`

`terraform fmt` (fix formatting) and `terraform validate` (syntax, attributes,
value types, etc) ensure config file is easy to read and is valid.

`terraform apply` to create defined resources, upon creation `terraform.tfstate`
file is created to reflect all properies of the resources to be able to further
manage or destroy them (this file is shared for collaborative purposes).
Inspect current state with `terraform show`. `terraform state` command (along
with subcommands) is used for advanced state management,
[docs](https://www.terraform.io/docs/commands/state/index.html).

to change infrastructure make changes to tf file and run `terraform apply`,
terraform will display what changes are going to be performed.

`terraform destroy` to remove all resources defined in the configuration file

---

## State

State can be inspected with `terraform state` command, which outputs the list
of names of the resources, which could further be inspected with
`terraform show {name_of_resource}` command. `show` command has a `-json`
option, which outputs in corresponding format that can be used by other
wrappers, like `jq`, for example, to run queries.

Visual representation can be achived by using `terraform graph` command, which
outputs infrastructure in DOT syntax, which can be pasted inserted info
[webgraphviz](http://webgraphviz.com/) to output visual graph representation.

[add about saving state to remote repo for team collaboration]

---

## Output

To show specific information upon `apply` or for later inspection via
`terraform output {var}` use `output` block, which can be specified in any file

outputs.tf:
```terraform
output "ip" {
  value = aws_eip.ip.public_ip
}
```

---

## Variables

All files ending with `.tf` in the current directory are loaded.

variables.tf:
```terraform
variable "region" {
  type    = string # can be ommited
  default = "us-east-2"
}
```

In main configuration file access variables defined elsewhere with `var` prefix

main.tf:
```terraform
terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 2.70"
    }
  }
}

provider "aws" {
  profile = "default"
  region  = var.region
}

resource "aws_instance" "example" {
  ami           = "ami-830c94e3"
  instance_type = "t2.micro"
}
```

Another way is to have `some_name.tfvars` file with just list of var = value
and start main configuration file with contents of `variables.tf` file above
without `default` section.

Provide variables from command line, to override default values:
```shell
$ terraform apply -var 'region=us-east-1'
```

To persist variables create a `terraform.tfvars` (terraform automatically loads
`terraform.tfvars` and `*.auto.tfvars` files, anything else must be provided
with `-var-file` flag); `terraform.tfvars`:
```terraform
region = "us-east-2"
```

Variables can also be loaded from environment variables (must be prefixed with
`TF_VAR`, only string type varialbles):
```shell
$ export TF_VAR_region=us-east-2
```

If variables are not specified in any of the previous form, then terraform will
ask to input those when you run it (UI).

On other types of variables refer to
[examples and docs](https://learn.hashicorp.com/tutorials/terraform/aws-variables?in=terraform/aws-get-started)

### Data sources

Snippet below will query amazon for ami owned by current user
```terraform
data "aws_ami" "webserver_ami" {
  most_recent = true

  owners = ["self"]
  tags = {
    Name   = "Webserver"
	Deploy = "true"
  }
}

resource "aws_instance" "web" {
  ami = data.aws_ami.webserver_ami.id
}
```

---

## Meta arguments

Meta arguments add logic to code intepretation by Terraform

+ single meta argument, for example, `count = 2` would create 2 instances
+ block meta argumenst, should go at the end of block definition (following
example implements behaviour of first creating a resource, before destroying
old one):

	```terraform
	resource "aws_instance" "one" {
	  lifecycle {
	    create_before_destroy = true
	  }
	}
	```

Style conventions: first meta arguments, then single arguments, then block
arguments, and last section is block meta arguments (all logical blocks are
separated by empty line)

---

### lifecycle

+ `create_before_destroy` - change the order of resource update
+ `prevent_destroy` - absolutely prevent destroying the resource
+ `ignore_changes` - accepts list of properties that will be ignored to qualify
for resource recreation

```terraform
resouce "aws_instance" "some" {
  lifecycle {
    create_before_destroy = true
	ignore_changes = ["ami", "user_data"]
  }
}
```

---

### depends_on

Accepts a list of resources that should be created before the current one.
Should be considered last thing and contain good explained of why it was used.

```terraform
resouce "aws_instance" "some" {
  depends_on = ["aws_instance.db"]
}
```

---

## Expressions

Blocked used inside terraform resources

---

### dynamic

Used to generate repeating blocks. Example below for multiple ports for ingress
traffic in security group:
```terraform
resource "aws_security_group" "prod" {
  name = "Dynamic security group"

  dynamic "ingress" {
    for_each = ["80", "443", "8080"]
	content {
	  from_port   = ingress.value
	  to_port     = ingress.value
	  protocol    = "tcp"
	  cidr_blocks = ["0.0.0.0/0"]
	}
  }
}
```

---

## Modules

Provide a way to couple logical stuff together. By default everything is
working in `root` module. It is possible to set up remote modules, provide
versioning and set up separate provider block. `registry.terraform.io` - module
registry.

By conventions it is preferred to call terraform resources `this`, since they
don't have identity until they are deployed.

Have to call `terraform init`, if added a module.

Consists of:
+ `main.tf`
+ `variables.tf` - example above
+ `output.tf` - example above
+ README.md (autogenerated?)

Call for module:
```terraform
module "web_server" {
  source = "./modules/servers" # local

  # passing variables
  web_ami     = "ami-something"
  server_name = "prod-web"
}
```

---

## Tricks

+ create multiple servers:
```terraform
resource "aws_instance" "example" {
  count         = 3
  ami           = "ami-830c94e3"
  instance_type = "t2.micro"
}
```

+ refer to resource that has more than one instances
```terraform
resource "aws_eip_association" "prod_web" {
  instance_id   = aws_instance.prod_web[0].id
  # instance_id   = aws_instance.prod_web.0.id # also available dot syntax
  allocation_id = aws_eip.prod_web.id
}

resource "aws_default_subnet" "default_az1" {
  availability_zone = "us-east-1"
}

resource "aws_elb" "prod_web" {
  name            = "prod-web"
  instances       = aws_instance.prod_web[*].id
  subnets         = [aws_default_subnet.default_az1.id]
  security_groups = [aws_security_group.prod_web.id]

  listener {
    instance_port     = 80
	instance_protocol = "http"
    lb_port           = 80
	lb_protocol       = "http"
  }
}

```


+ dependency example (webserver); `from_port` and `to_port` are used to define
a range, so if single port is appointed, both variables have the same value:
```terraform
# user_data section is for bootsraping
# don't leave whitespace in front
resource "aws_instance" "webserver" {
  ami                    = "ami-830c94e3"
  instance_type          = "t2.micro"

  vpc_security_group_ids = [aws.security_group.wb_sg.id] # dependency

  user_data	             = <<EOF
#!/bin/bash
yum -y update
yum -y install httpd
myip =`curl http://169.254.169.254/latest/meta-data/local-ipv4`
echo "<h2>My webserver at $myip<h2>" > /var/www/html/index.html
sudo service httpd start
chkconfig httpd on
EOF

}

resource "aws_security_group" "wb_sg" {
  name          = "WebServer Security Group"

  ingress {
    from_port   = 80
	to_port     = 80
	protocol    = "tcp"
	cidr_blocks = ["0.0.0.0/0"]
  }

  ingress {
    from_port   = 443
	to_port     = 443
	protocol    = "tcp"
	cidr_blocks = ["0.0.0.0/0"]
  }

  engress {
    from_port   = 0
	to_port     = 0
	protocol    = "-1" # any
	cidr_blocks = ["0.0.0.0/0"]
  }
}
```

+ using external static files (provide shell script from previous example):
```terraform
resource "aws_instance" "webserver" {
  ami                    = "ami-830c94e3"
  instance_type          = "t2.micro"

  vpc_security_group_ids = [aws.security_group.wb_sg.id] # dependency

  user_data              = file("user_data.sh") # relative to this file
}
```

+ decouple `aws_eip` into `aws_eip_association`, so that first one becomes
independent of instance:
```terraform
resource "aws_eip_association" "prod_web" {
  instance_id   = aws_instance.prod_web.id
  allocation_id = aws_eip.prod_web.id
}

resource "aws_eip" "prod_web" {
  tags {
    "Terraform" : "true"
  }
```

+ using external dymanic files:
```terraform
resource "aws_instance" "webserver" {
  ami                    = "ami-830c94e3"
  instance_type          = "t2.micro"

  vpc_security_group_ids = [aws.security_group.wb_sg.id] # dependency

  # tpl is common convention for this type of files
  # second argument is used for providing variables of various types
  user_data              = templatefile("init.sh.tpl", {
    name = "learner"
	chapters = ["terraform", "aws", "that's it really"]
  }))
```

external file:
```shell
#!/bin/bash
yum -y update
yum -y install httpd
myip =`curl http://169.254.169.254/latest/meta-data/local-ipv4`
echo "<h2>My webserver at $myip<h2>" > /var/www/html/index.html
cat <<EOF > /var/www/html/index.html
<h2>My name is ${name}</h2>
Here is list of chapters already covered:<br>
{% for chapter in chapters ~}
- ${chapter}
{% endfor ~}
EOF
sudo service httpd start
chkconfig httpd on
```

+ to check contents of generated dynamic file (before actually applying to
infrastructure) use `terraform console` command, simply run `templatefile` with
the same arguments to see the result; exit console with `exit`

+ best practice - add tag `Terraform` - `true` to any resource to easily
distinguish terraform managed resource while in AWS UI

+ to get information from provider about various resource (not neccesary
created by you) use [data sources](https://www.terraform.io/docs/language/data-sources/index.html).

	```terraform
	data "aws_availability_zones" "available" {}

	output "aws_availability_zones" {
	  value = data.aws_availability_zones.available.names
	}
	```
	Other good aws data sources:
	+ `aws_caller_identity`
	+ `aws_vpc`
	+ `aws_vpcs`
	+ `aws_ami` - for automated instance creation, without looking up the exact
	ami in particular zone; to find owner id go to Amazon AMIs page, public
	images and insert ami (info will be in details), for value get the
	unchanged part of the name followed by wildcard

		```terraform
		data "aws_ami" "latest_ubuntu" {
		  owners = ["read-text-above-for-value"]
		  most_recent = true

		  filter {
		    name = "name" # choose type of filter
			values = ["read-text-above-for-value-*"]
		  }
		}
		```
+ `merge` functions is used to merge objects of the same type into one (for
example, two maps - common tags from `variables.tf` and individual tag of the
resource):
```terraform
resource "aws_instance" "webserver" {
  ami                    = "ami-830c94e3"
  instance_type          = "t2.micro"

  vpc_security_group_ids = [aws.security_group.wb_sg.id] # dependency

  tags = merge(var.common_tags, { Name = "my instance" })
}
```

---

## AWS

### aws resources

+ `aws_s3_bucket` - storage
+ `aws_default_vpc` - special `default` vpc configuration, also possible to
create custrom vpc
+ `aws_security_group` - firewall configuration
+ `aws_instance` - ec2, virtual machine
+ `aws_eip` (elastic ip) - static ip address
+ `aws_elb` - load balancer
+ `aws_autoscaling_group`
	```terraform
	resource "aws_launch_template" "prod_web" {
	  name_prefix   = "prod-web"
	  image_id      = "ami-something"
	  instance_type = "t2.micro"
	}

	resource "aws_autoscaling_group" "prod_web" {
	  avalibility_zones = ["us-east-1"]
	  desired_capacity  = 1
	  max_size          = 1
	  min_size          = 1

	  launch_template {
	    id      = aws_launch_template.prod_web.id
		version = "$latest"
	  }
	  # usual tags format is not supported for asg
	  # because it has also to assign to instances as well
	  tag {
	    key                 = "Terraform"
		value               = "true"
		propogate_at_launch = true
	  }
	}

	resource "aws_autoscaling_attachment" "prod_web" {
	  autoscaling_group_name = aws_autoscaling_group.prod_web.id
	  elb                    = aws_elb.prod_web.id
	}
	```


# Terraform 🛕

Terraform files can be in terraform or in json format (including var files).

# Contents

+ [Installation](#installation)
+ [First run](#first-run)
+ [State](#state)
+ [Variables](#variables)
+ [Output](#output)
+ [Tricks](#tricks)

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

## Variables

All files ending with `.tf` in the current directory are loaded.

variables.tf:
```terraform
variable "region" {
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

## Output

To show specific information upon `apply` or for later inspection via
`terraform output {var}` use `output` block, which can be specified in any file

outputs.tf:
```terraform
output "ip" {
  value = aws_eip.ip.public_ip
}
```

## Tricks

+ create multiple servers:
```terraform
resource "aws_instance" "example" {
  count         = 3
  ami           = "ami-830c94e3"
  instance_type = "t2.micro"
}
```

+ dependency example (webserver):
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
```terrafrom
resource "aws_instance" "webserver" {
  ami                    = "ami-830c94e3"
  instance_type          = "t2.micro"

  vpc_security_group_ids = [aws.security_group.wb_sg.id] # dependency

  user_data              = file("user_data.sh") # relative to this file
```

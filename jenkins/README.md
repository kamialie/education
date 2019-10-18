# Jenkins 👨🏼‍🍳

## Get started

* [get started with pipelines in jenkins](https://jenkins.io/pipeline/getting-started-pipelines)
* [simple java maven example](http://jenkins.io/doc/tutorials/build-a-java-app-with-maven/)
* [multibranch example](https://jenkins.io/doc/tutorials/build-a-multibranch-pipeline-project/)
* [checkout step](https://jenkins.io/doc/pipeline/steps/workflow-scm-step/)
* [jenkins docker image](https://github.com/jenkinsci/docker/blob/master/README.md)

# Contents

* [Using credentials](#using-credentials)
* [Pipeline](#pipeline)
* [Defining a Pipeline](#defining-a-pipeline)
* [Using Jenkinsfile](#using-jenkinsfile)
	- [Strings](#string-interpolation)
	- [Environment variable](#using-environment-variables)
	- [Credentials](#handling-credentials)
	- [Parameters](#handling-parameters)
	- [Post action](#handling-failure)
	- [ ] [Multiple agents](#using-multiple-agents)
	- [ ] [Optional step arguments](#optinal-step-argument)
* [Pipeline syntax](#pipeline-syntax)
	- Sections
		+ [ ] [agent](#agent)
		+ [post](#post)
		+ [stages](#stages)
		+ [ ] [steps](#steps)
	- [Directives](#directives)
		+ [x] [environment](#environment)
		+ [ ] [options](#options)
		+ [ ] [parameters](#parameters)
		+ [x] [triggers](#triggers)
		+ [x] [Jenkins cron syntax](#jenkins-cron-syntax)
		+ [x] [tools](#tools)
		+ [ ] [input](#input)
		+ [x] [when](#when)
	- [ ] [Sequencial stages](#sequencial-stages)
	- [ ] [Parallel stages](#parallel-stages)
	- [ ] [Scripted pipeline](#scripted-pipeline)
* [Plugins](#plugins)
* [Other cool features](#other-cool-features)

----------

## Using credentials

[Documentation](https://jenkins.io/doc/book/using/using-credentials/)

[Credential Binding plugin](https://plugins.jenkins.io/credentials-binding)

Jenkins creds can be used globally, by specific project or item, by specific user.

Types of credentials:
* secret text - for example API token (Github personal token)
* username and password
* secret file - essentially secret content in a file
* SSH username with private key - SSH public/private key pair
* certificate
* docker host certificate authentication

----------

## Pipeline

The definition of a Jenkins Pipeline is either written into text file (Jenkinsfile) or in the web UI. Best practice considers Jenkinsfile.

Jenkinsfile can be written using either Declarative or Scripted syntax (see [below](#pipeline-syntax-overview))

Pipeline concepts:
* **Pipeline** - user-defined model of a CD pipeline; Pipelines's code defines entire build process; pipeline block is key part of Declarative Pipeline syntax
* **Node** - a machine which is part of Jenkins environment, capable of executing a Pipeline
* **Stage** - block which defines a conceptually distinct subset of tasks performed through the entire Pipeline (e.g. "Build", "Test", "Deploy")
* **Step** - single task (e.g. sh 'make')

----------

### [Pipeline syntax overview](https://jenkins.io/doc/book/pipeline/syntax)

Declarative Pipeline syntax has pipeline block which definess all the work done
```groovy
pipeline {
	agent any
	stages {
		stage('Build') {
			steps {
				//
			}
		}
		stage('Test') {
			steps {
				//
			}
		}
		stage('Deploy') {
			steps {
				//
			}
		}
	}
}
```

Scripted Pipeline has one or more node blocks, which do the work
```groovy
node {
	stage('Build') {
		//
	}
	stage('Test') {
		//
	}
	stage('Deploy') {
		//
	}
}
```
Although not required, confining all work inside a node block has advantages
* schedules the steps to run by adding to Jenkins queue
* creates a workspace

----------

## Defining a Pipeline

To define Jenkinsfile in SCM (source control management) select *Pipeline script from SCM* option in Pipeline section.

Pipeline ships with built-in documentation features, which is also automatically updated based on the plugins installed. Can be accessed globally at ${YOUR_JENKINS_URL}/pipeline-syntax and at sidebar menu "Pipeline Syntax" on the left.

Pipeline also provides a built-in **Global Variable Reference**, documentation for variables provided by Pipeline of plugins, which are available. Available at ${YOUR_JENKINS_URL}/pipeline-syntax/globals and in the "Pipeline Syntax page"

Declarative directive generator is available in the "Pipeline Syntax" page, can help to generate stages.

----------

## Using Jenkinsfile

Jenkins has a number of plugins invoking practically any build tool in general use. To invoke commands use *sh* for unix/linux based systems and *bat* for windows.

JUnit plugin can provide reporting and visualization for running tests.

currentBuild variable (e.g. currentBuild.result == 'SUCCESS') can be used to determine what to do in deployment stage.

### String interpolation

```groovy
def singleQuoted = 'Hello'
def doubleQuoted = "World"
```

Single quotes do not support string interpolation (dollar sign insertion), while double quotes do

----------

### Using environment variables

Environment variables are exposed vie global varialbe *env*, which is available from anywhere in Jenkinsfile. Full list is available in Pipeline Syntax page and includes:
* **BUILD_ID** - current build ID (identical to **BUILD_NUMBER** for Jenkins versions 1.597+)
* **BUILD_TAG** - string of jenkins ${JOB_NAME}-${BUILD_NUMBER} convinient for identification
* **BUILD_URL** - where results of the build can be found
* **EXECUTER_NUMBER** - unique number of the current executor, start with 0, not 1 unlike in "build executor status"

To set an environment variable use environment directive (two examples have different scopes):
```groovy
pipeline {
	agent any
	environment {
		CC = 'clang'
	}
	stages {
		stage('Example') {
			environment {
				DEBUG_FLAGS = '-g'
			}
			steps {
				sh 'printenv'
			}
		}
	}
}
```

Environment variables can also be set dynamically. [to be continued]

Print all environment variable:

		sh 'printenv'

----------

### Handling credentials

Declarative Pipeline syntax uses *credentials()* helper method (used with environment directove), which supports secret text, username and password, and secret file credentials.

* Secret text example:
```groovy
pipeline {
	agent {
		//
	}
	environment {
		AWS_ACCESS_KEY_ID = credentials('jenkins-aws-secret-key-id')
		AWS_SECRET_ACCESS_KEY = credentials('jenkins-aws-secret-access-key')
	}
}
```
* Username and password example:
```groovy
pipeline {
	agent {
		//
	}
	environment {
		BITBUCKET_COMMON_CREDS = credentials('jenkins-bitbucket-common-creds')
	}
}
```
	Example above actually sets 3 variables: BITBUCKET_COMMON_CREDS (contains username and password separated by a colon), BITBUCKET_COMMON_CREDS_USR (only username), BITBUCKET_COMMON_CREDS_PSW (only password).

* Other credential types:

	To other types of credentials use Snipper Generator: Pipeline Syntax (on the left) -> Snippet Generator -> choose **withCredentials: Bind credentials to variable** in Sample step -> unduer Bindings add the credential type.

	SSH user private key example(SSH_KEY_FOR_ABC variable is now available in the following code block to be used):
```grovy
withCredentials(bindings: [sshUserPrivateKey(credentialsId: 'jenkins-ssh-key-for-abc', \
                                             keyFileVariable: 'SSH_KEY_FOR_ABC', \
                                             passphraseVariable: '', \
                                             usernameVariable: '')]) {
  // some block
}
```
	Combining credentials in single step can be accessed in Snippet Generator.

----------

### Handling parameters

Parameters are defined in parameters directive.

```groovy
pipeline {
    agent any
    parameters {
        string(name: 'Greeting', defaultValue: 'Hello', description: 'How should I greet the world?')
    }
    stages {
        stage('Example') {
            steps {
                echo "${params.Greeting} World!"
            }
        }
    }
}
```

### Handling failure

[to be continued]
https://jenkins.io/doc/book/pipeline/jenkinsfile/

### Using multiple agents

[to be continued]
https://jenkins.io/doc/book/pipeline/jenkinsfile/

----------

## Pipeline syntax

Basic statements and expressions in Declarative Pipeline follow same rules as Groovy's syntax except:
* top level must be a block, pipeline { }
* each statements on its own line (no semicolon separator)
* blocks must only consist of sections, directives, steps, or assignment statements
* property reference statement is treated as no-argument method invocation (e.g. input is treated s input())

**Following is for Declarative syntax!**

----------

### Agent

Specifies where the entire Pipeline or specific stage will execute in the Jenkins environment, depending on where the agent section is placed. Must be defined at the top level inside pipeline block, stage-level usage is optional.

Parameters:
* any - any available agent

		agent any

* none - when used at the top-level, no global agent will be allocated, thus each stage has to declare its own

		agent none

* label - agent available in the Jenkins environment with the provided label

		agent {
			label 'my-defined-label'
		}

* node - same as label, but allows additional options like customWorkspace

		agent {
			node {
				label 'labelNmae'
			}
		}

* docker
[to be continued]

* dockerfile
[to be continued]

* kubernetes
[to be continued]

Common options - [to be continued]

----------

### Post

Defines one or more additional steps that are run upon the completion of Pipeline's or stage's run (depends on the location of post).

Conditions (options):
* always
* changed - if has a different completion status from its previous run
* fixed - if current is successful and previous was failed or unstable
* regression - if current is failure, unstable, or aborted and previous one was successful
* aborted - usually manually aborted
* failure
* success
* unstable - usually caused by test failures, code violations
* unsuccessful - not successful
* cleanup - run after all previous steps, regardless of status

----------

### Stages

Contains one or more stage directives.

Stage directive goes in stages section and should contain a steps section, and optional stage-specific directives (e.g. agent section).
```groovy
pipeline {
	agent any
	stages {
		stage('Example') {
			steps {
				echo 'Hello'
			}
		}
	}
}
```

----------

### Steps

Contains series of one or more steps to be executed.

[Pipeline Steps reference](#https://jenkins.io/doc/pipeline/steps/)

### script

Takes a block of Scripted Pipeline and executes that in the Declarative Pipeline.

----------

## Directives

### Environment

Specifies a sequence of key-value pairs which will be defined as environment variables for all the steps, or stage-specific (depends on location of directive).

Supports special helper method *credentials* which can be used to access pre-defined Credentials (Secret Text, Secret File, Username and password, SSH with Private Key), more in [credentials section](#handling-credentials)

----------

### Options

Allows configuring Pipelines-specific options from within the Pipeline itself.

Available options:
* buildDiscarder - persists artifacts and console outut for the specific number of recent Pipeline runs

		options {
			buildDiscarder(
				logRotator(
					numToKeepStr: '1'
				)
			)
		}

* checkoutToSubDirectory - perform automatic source control checkout in a subdirectory of the workspace

		options {
			checkoutToSubDirectory('foo')
		}

* disableConcurrentBuild - disallow concurrent executions of the Pipeline, useful for preventing simultaneous accesses to shared resources

		options {
			disableConcurrentBuild()
		}

* disableResume - do not let the pipeline ti resume if master restarts

		options {
			disableResume()
		}

* newContainerPerStage - [to be continued]
* overrideIndexTriggers - [to be continued]
* preserveStashes - [to be continued]
* quietPeriod - set the quiet period, overriding the global default

		options {
			quietPeriod(30)
		}

* retry - retry entire Pipeline the specified number of times

		options {
			retry(3)
		}

* skipDefaultCheckout - [to be continued]
* skipStagesAfterUnstable

		options {
			skipStagesAfterUnstable()
		}
* timeout - set timeout, after which Jenkins aborts the Pipeline

		options {
			timeout(
				time: 1,
				unit: 'HOURS'
			)
		}

* timestamps - prepend all console output with the time

		options {
			timestamps()
		}

* parallelAlwaysFailFast - [to be continued]

Inside a stage, the steps inside options directive are invoked before entering agent or checking *when* condition. Can only contain retry, timeout, timestamps and Declarative options relevant to a stage, like skipDefaultCheckout.

----------

### Parameters

----------

### Triggers

Defines the automated ways in which the Pipeline should be re-triggers. Defined at top level pipeline block.

* cron - cron-style string to define a regular interval at whick Pipeline should be re-triggered

		triggers {
			cron('H */4 * * 1-5')
		}

* pollSCM - cron-style string to define interval at which Jenkins should check for new source changes

		triggers {
			pollSCM('H */4 * * 1-5')
		}

* upstream - comma separate string of jobs and a threshhold; when any job finished the minimum threshgold, the Pipeline will be re-triggered.

		triggers {
			upstream(
				upstreamProjects: 'job1, job2,
				threshold: hudson.model.Result.SUCCESS)
		}

[Trigger examples](https://mohamicorp.atlassian.net/wiki/spaces/DOC/pages/136806413/Triggering+Jenkins+for+Commits+to+a+Specific+Branch)

----------

### Jenkins cron syntax

Five specifiers separeted by tab or space are as follows:
* **MINUTE** - minutes within the hour (0-59)
* **HOUR** - the hour of the day (0-23)
* **DOM** - day of the month (1-31)
* **MONTH** - the month (1-12)
* **DOW** - day of the week (0-7 wher 0 and 7 are Sunday)

To specify multiple values:
* \* - specifies all possible values
* M-N - specify range of values
* M-N/X or \*/X - steps by intervals of X through the range
* A,B, ..., Z - enumerate over multiple values

To allow periodically scheduled taks to produce even load use **H** ("hash", of the job name, thus persistent for one job) wherever possible (e.g. 0 0 * * * high load at midnight, H H * * * still once a day, but not all at the same time). **H** can be used with ranges too (e.g. H H(0-7) * * * - some time between 12am to 7:59an).

Convinient aliases - @yearly, @annually, @monthly, @weekly, @daily, @midnight, @hourly, e.g. @hourly is any time during the hour, @midniht is sometime between 12am to 2:59am.

----------

### tools

Section defining tools to autoinstall and put on the PATH (ignored if *agent none* is specified. Defined at the top-level pipeline section

Supported tools: maven, jdk, gradle (?)

----------

### input

Allows to prompt for input using input step.

----------

### when

Determines whether the shage should be executed depending on the given condition. Must at least contain one condition.

More complex conditional structures can be built using - *not*, *allOf*, *anyOf*.

Built-in conditions:
* **branch** - execute the stage when the specified branch in being built. Works only on multibranch Pipeline.
```groovy
when {
	branch 'master'
}
```
* **buildingTag** - execute when the build is building a tag
```groovy
when {
	buildingTag()
}
```
* **changelog** - execute if the build's SCM changelog containes given regular expression pattern.
```groovy
when {
	changelog '.*^\\[DEPENDENCY\\} .+$'
}
```
* **changeset** - [to be continued]
* **changeRequest** - [to be continued]
* **environment** - when specified variable is set to the given value
```groovy
when {
	environment name: 'DEPLOY_TO', value: 'production'
}
```
* **equals** - when the expected value is equal to the actual value
```groovy
when {
	equals expected: 2, actual: currentBuild.number
}
```
* **expression** - [to be continued]
* **tag** - [to be continued]
* **not** - execute when the nested condition is false
```groovy
when {
	not {
		branch 'master'
	}
}
```
* **anyOf** - execute when any condition is true
```groovy
when {
	anyOf {
		branch 'master';
		branch 'staging'
	}
}
```
* **allOf** - execute when all nested conditions are true
```groovy
when {
	anyOf {
		branch 'master';
		branch 'stagin'
	}
}
```

* **triggeredBy** - execute when the current build has been triggered by the param given
```groovy
when {
	triggeredBy ['SCMTrigger', 'TimerTrigger', UpstreamCause', [cause: "UserIdCause", detail: "vlinde"]]
}
```

By default when is evaluated after entering agent for that stage, can be changed by setting **beforeAgent** to true:
```groovy
when {
	beforeAgent true
}
```

Check **beforeInput** specifier (same as above, but for input).

----------

## Sequential stages

Stages may be nested within other in sequential order. A stage can have only one of **steps**, **parallel**, **stages**.

## Parallel

Nested stages cannot contain further **parallel** stages themselves. Any stage containing **parallel** cannot contain **agent** or **tools**.

To force parallel stages to be aborted when one of them fails add **failFast true** to the stage containing **parallel**. Can also add **parallelsAlwaysFailFast()** in options section of the pipeline.

----------

## Scripted Pipeline

[tobe continued]

----------

### Other cool features

* [Development tools(cli)](https://jenkins.io/blog/2017/05/18/pipeline-dev-tools/)

* timeouts, retries...
```groovy
pipeline {
    agent any
    stages {
        stage('Deploy') {
            steps {
                retry(3) {
                    sh './flakey-deploy.sh'
                }

                timeout(time: 3, unit: 'MINUTES') {
                    sh './health-check.sh'
                }
            }
        }
    }
}
```
Wrappers steps such as *timeout* and *retry* may contain other steps include ing *timeout* or *retry*
```groovy
pipeline {
    agent any
    stages {
        stage('Deploy') {
            steps {
                timeout(time: 3, unit: 'MINUTES') {
                    retry(5) {
                        sh './flakey-deploy.sh'
                    }
                }
            }
        }
    }
}
```

* cleanup or other steps based on outcome of the Pipeline, that is *post* section
```groovy
Jenkinsfile (Declarative Pipeline)
pipeline {
    agent any
    stages {
        stage('Test') {
            steps {
                sh 'echo "Fail!"; exit 1'
            }
        }
    }
    post {
        always {
            echo 'This will always run'
        }
        success {
            echo 'This will run only if successful'
        }
        failure {
            echo 'This will run only if failed'
        }
        unstable {
            echo 'This will run only if the run was marked as unstable'
        }
        changed {
            echo 'This will run only if the state of the Pipeline has changed'
            echo 'For example, if the Pipeline was previously failing but is now successful'
        }
    }
}
```

## Topic links

- Extra:
    + https://jenkins.io/pipeline/getting-started-pipelines/ - getting started with pipelines in jenkins
	+ [simple java maven example](http://jenkins.io/doc/tutorials/build-a-java-app-with-maven/)
	+ [multibranch example](https://jenkins.io/doc/tutorials/build-a-multibranch-pipeline-project/)

# Contents

* [Using credentials](#using-credentials)
* [Pipeline](#pipeline)
* [Defining a Pipeline](#defining-a-pipeline)
* [Using Jenkinsfile](#using-jenkinsfile)
	- [Strings](#string-interpolation)
	- [Environment variable](#using-environment-variables)
	- [Credentials](#handling-credentials)
	- [Parameters](#handing-parameters)
	- [Post action](#handling-failure)
	- [ ] [Multiple agents](#using-multiple-agents)
	- [ ] [Optional step arguments](#optinal-step-argument)
* [Pipeline syntax](#pipeline syntax)
	- [ ] [Sections](#section)
		+ [agent](#agent)
		+ [post](#post)
		+ [stages](#stages)
		+ [steps](#steps)
	- [ ] [Directives](#directives)

----------

### Using credentials

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

### Pipeline

The definition of a Jenkins Pipeline is either written into text file (Jenkinsfile) or in the web UI. Best practice considers Jenkinsfile.

Jenkinsfile can be written using either Declarative or Scripted syntax (see [below](#pipeline-syntax-overview))

Pipeline concepts:
* **Pipeline** - user-defined model of a CD pipeline; Pipelines's code defines entire build process; pipeline block is key part of Declarative Pipeline syntax
* **Node** - a machine which is part of Jenkins environment, capable of executing a Pipeline
* **Stage** - block which defines a conceptually distinct subset of tasks performed through the entire Pipeline (e.g. "Build", "Test", "Deploy")
* **Step** - single task (e.g. sh 'make')

#### Pipeline syntax overview

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

### Defining a Pipeline

To define Jenkinsfile in SCM (source control management) select *Pipeline script from SCM* option in Pipeline section.

Pipeline ships with built-in documentation features, which is also automatically updated based on the plugins installed. Can be accessed globally at ${YOUR_JENKINS_URL}/pipeline-syntax and at sidebar menu "Pipeline Syntax" on the left.

Pipeline also provides a built-in **Global Variable Reference**, documentation for variables provided by Pipeline of plugins, which are available. Available at ${YOUR_JENKINS_URL}/pipeline-syntax/globals and in the "Pipeline Syntax page"

Declarative directive generator is available in the "Pipeline Syntax" page, can help to generate stages.

----------

### Using Jenkinsfile

Jenkins has a number of plugins invoking practically any build tool in general use. To invoke commands use *sh* for unix/linux based systems and *bat* for windows.

JUnit plugin can provide reporting and visualization for running tests.

currentBuild variable (e.g. currentBuild.result == 'SUCCESS') can be used to determine what to do in deployment stage.

#### String interpolation

```groovy
def singleQuoted = 'Hello'
def doubleQuoted = "World"
```

Single quotes do not support string interpolation (dollar sign insertion), while double quotes do

----------

#### Using environment variables

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

----------

#### Handling credentials

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

#### Handling parameters

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

#### Handling failure

[to be continued]
https://jenkins.io/doc/book/pipeline/jenkinsfile/

#### Using multiple agents

[to be continued]
https://jenkins.io/doc/book/pipeline/jenkinsfile/

### Usefuls tools, techniques

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

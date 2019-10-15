## Topic links

- Extra:
    + https://jenkins.io/pipeline/getting-started-pipelines/ - getting started with pipelines in jenkins
	+ [simple java maven example](http://jenkins.io/doc/tutorials/build-a-java-app-with-maven/)
	+ [multibranch example](https://jenkins.io/doc/tutorials/build-a-multibranch-pipeline-project/)

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

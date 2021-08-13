pipeline {
  agent any
  stages {
    stage('Build') {
      steps {
        script {
          dockerImage = docker.build("${REPOSITORY}", "-f ${DOCKERFILE_PATH} .")
        }
      }
    }
    stage('Docker Push') {
      steps {
        script {
          docker.withRegistry("${REGISTRY_ENDPOINT}", 'docker.voidwell.com') {
            dockerImage.push("${BUILD_NUMBER}")
            dockerImage.push("latest")
          }
        }
      }
    }
    stage('Update Release') {
      steps {
        dir ('release-tmp') {
          git branch: 'master', credentialsId: 'GithubSSH', url: 'git@github.com:voidwell/server.git'
          sh '''#!/bin/bash
ENV_VAR_KEY="IMAGE_${SERVICE_NAME^^}_VERS"

sed -i "/^${ENV_VAR_KEY}=/{h;s/=.*/=${BUILD_NUMBER}/};\\${x;/^$/{s//${ENV_VAR_KEY}=${BUILD_NUMBER}/;H};x}" $RELEASE_FILE

git add $RELEASE_FILE
git commit -m "Updated ${ENV_VAR_KEY} in ${RELEASE_FILE} with ${BUILD_NUMBER}"
'''
          sshagent(credentials: ['GithubSSH']) {
            sh 'git push origin master'
          }
        }
      }
    }
  }
  environment {
    REGISTRY_ENDPOINT = 'https://docker.voidwell.com'
    REPOSITORY = 'voidwell/daybreakgames'
    SERVICE_NAME = 'daybreakgames'
    DOCKERFILE_PATH = './Dockerfile'
    RELEASE_FILE = '.env.prod'
    dockerImage = ''
  }
}

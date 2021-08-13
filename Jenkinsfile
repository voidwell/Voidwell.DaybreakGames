pipeline {
  agent any
  stages {
    stage('Build') {
      agent any
      steps {
        sh '''#!/bin/bash
docker build -t ${REGISTRY_ENDPOINT}/${REPOSITORY}:latest -t ${REGISTRY_ENDPOINT}/${REPOSITORY}:${BUILD_NUMBER} -f ${DOCKERFILE_PATH} .

echo -e "\\nBuild Completed"'''
      }
    }
    stage('Docker Push') {
      steps {
        sh '''#!/bin/bash
docker push ${REGISTRY_ENDPOINT}/${REPOSITORY}:${$BUILD_NUMBER}
docker push ${REGISTRY_ENDPOINT}/${REPOSITORY}:latest

echo -e "\\nPushed ${REPO_LABEL}:${BUILDTAG}"'''
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
git commit -m "Updated ${ENV_VAR_KEY} with ${BUILDTAG}"
'''
          sshagent(credentials: ['GithubSSH']) {
            sh 'git push origin master'
          }
        }
      }
    }
  }
  environment {
    REGISTRY_USER = credentials('docker-registry-user')
    REGISTRY_PASSWORD = credentials('docker-registry-password')
    REGISTRY_ENDPOINT = 'docker.voidwell.com'
    REPOSITORY = 'voidwell/daybreakgames'
    SERVICE_NAME = 'daybreakgames'
    DOCKERFILE_PATH = './Dockerfile'
    RELEASE_FILE = '.env.prod'
  }
}

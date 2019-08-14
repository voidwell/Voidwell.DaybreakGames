pipeline {
  agent any
  stages {
    stage('Build') {
      agent any
      steps {
        sh '''#!/bin/bash
REGISTRY=$REGISTRY_ENDPOINT
REPOSITORY=$REPOSITORY
DOCKERFILE_PATH=$DOCKERFILE_PATH

docker build -t ${REGISTRY}/${REPOSITORY}:latest -f ${DOCKERFILE_PATH} .

echo -e "\\nBuild Completed"'''
      }
    }
    stage('Docker Push') {
      steps {
        sh '''#!/bin/bash
REGISTRY=$REGISTRY_ENDPOINT
REPOSITORY=$REPOSITORY
REGISTRY_USER=$REGISTRY_USER
REGISTRY_PASSWORD=$REGISTRY_PASSWORD

REGISTRY_CRED="${REGISTRY_USER}:${REGISTRY_PASSWORD}"
REPO_LABEL=${REGISTRY}/${REPOSITORY}

TAGS="`curl -s --user ${REGISTRY_CRED} https://${REGISTRY}/v2/${REPOSITORY}/tags/list | jq -r \'.tags\' | sed \'s/[^0-9]*//g\'`"
LATEST=`echo "${TAGS[*]}" | sort -nr | head -n1`
BUILDTAG=$((LATEST + 1))

docker tag ${REPO_LABEL}:latest ${REPO_LABEL}:${BUILDTAG}

docker push ${REPO_LABEL}:${BUILDTAG}
docker push ${REPO_LABEL}:latest

echo export "${REPO_LABEL}:${BUILDTAG}" > buildlabel
echo -e "\\nPushed ${REPO_LABEL}:${BUILDTAG}"'''
      }
    }
    stage('Deploy') {
      steps {
        sh '''#!/bin/bash
#COMPOSE_PATH=$COMPOSE_PATH

#BUILD_LABEL=readFile(\'buildlabel\').trim()

#echo ${COMPOSE_PATH}
#echo ${BUILD_LABEL}'''
      }
    }
  }
  environment {
    REGISTRY_USER = credentials('docker-registry-user')
    REGISTRY_PASSWORD = credentials('docker-registry-password')
    REGISTRY_ENDPOINT = 'docker.voidwell.com'
    REPOSITORY = 'voidwell/daybreakgames'
    DOCKERFILE_PATH = './Dockerfile'
    COMPOSE_PATH = '/docker-configs/voidwell/docker-compose.yml'
  }
}

#!/bin/bash
REGISTRY="192.168.2.100:5000"
REPOSITORY="voidwell/daybreakgames"

TAGS="`curl -s http://$REGISTRY/v2/$REPOSITORY/tags/list | jq -r '.tags' | sed 's/[^0-9]*//g'`"
LATEST=`echo "${TAGS[*]}" | sort -nr | head -n1`
BUILDTAG=$((LATEST + 1))

docker.exe build -t ${REGISTRY}/${REPOSITORY}:${BUILDTAG} -t ${REGISTRY}/${REPOSITORY}:latest .

docker.exe push ${REGISTRY}/${REPOSITORY}:${BUILDTAG}
docker.exe push ${REGISTRY}/${REPOSITORY}:latest

echo -e "\nCompleted ${REGISTRY}/${REPOSITORY}:${BUILDTAG}"
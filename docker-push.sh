#!/bin/bash

set -ex

CURRENT_DIR="$(echo ${PWD##*/} | tr '[:upper:]' '[:lower:]')"
IMAGE_USER="patrickfnielsen"
IMAGE_NAME="$IMAGE_USER/$CURRENT_DIR"
TAG="${1}"

REGISTRY="docker.io"

docker build -t ${REGISTRY}/${IMAGE_NAME}:${TAG} -t ${REGISTRY}/${IMAGE_NAME}:latest .
docker push ${REGISTRY}/${IMAGE_NAME}:${TAG}
docker push ${REGISTRY}/${IMAGE_NAME}:latest

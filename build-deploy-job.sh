set -e

# PROJECT_ID=123 REPOSITORY=myrepo REGISTRY_REGION=us-central1 ./build-deploy-job.sh

docker buildx build \
  --push \
  --platform linux/amd64 \
  -t $REGISTRY_REGION-docker.pkg.dev/$PROJECT_ID/$REPOSITORY/test-invoke-job \
  -f Dockerfile.job .
set -e

# PROJECT_ID=123 REPOSITORY=myrepo REGISTRY_REGION=us-central1 RUNTIME_REGION=us-east4 ./build-deploy-api.sh

docker buildx build \
  --push \
  --platform linux/amd64 \
  -t $REGISTRY_REGION.pkg.dev/$PROJECT_ID/$REPOSITORY/test-invoke-job-api-svc \
  -f Dockerfile.api .

# Deploy image
gcloud run deploy test-invoke-job-api-svc \
  --image=$REGISTRY_REGION-docker.pkg.dev/$PROJECT_ID/$REPOSITORY/test-invoke-job-api-svc:latest \
  --allow-unauthenticated \
  --min-instances=0 \
  --max-instances=1 \
  --region=$RUNTIME_REGION \
  --cpu-boost \
  --memory=256Mi
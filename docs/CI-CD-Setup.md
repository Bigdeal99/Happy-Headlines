# CI/CD Pipeline Documentation

## Overview

This document describes the Continuous Integration and Continuous Delivery (CI/CD) pipeline for the HappyHeadlines microservices project. The pipeline automatically builds Docker images for all services and pushes them to GitHub Container Registry (GHCR) whenever code is pushed to the `main` or `master` branch.

## Architecture

### Services
The pipeline builds and deploys the following 6 microservices:
1. **ArticleService** - Article management with Redis caching
2. **CommentService** - Comment management with LRU cache
3. **ProfanityService** - Profanity filtering service
4. **DraftService** - Draft article management
5. **PublisherService** - Article publishing with RabbitMQ
6. **NewsletterService** - Newsletter generation

### Pipeline Location
- **Workflow File:** `.github/workflows/ci-cd.yml`
- **Registry:** GitHub Container Registry (`ghcr.io`)
- **Image Prefix:** `ghcr.io/{owner}/happy-headlines/{service-name}`

## How It Works

### Triggers
The pipeline is triggered on:
- **Push to `main`, `master`, or `finnishing-the-compulsory-assignment` branch** - Full build and push to registry
- **Pull Request to `main`, `master`, or `finnishing-the-compulsory-assignment`** - Build only (no push) for validation
- **Manual trigger** - Via GitHub Actions UI (`workflow_dispatch`)

### Build Process

1. **Checkout Code** - Retrieves the latest code from the repository
2. **Setup Docker Buildx** - Configures Docker for multi-platform builds
3. **Login to GHCR** - Authenticates with GitHub Container Registry
4. **Generate Version Tags** - Creates appropriate image tags:
   - `latest` - Only for main/master branch
   - `{full-sha}` - Full commit SHA (e.g., `abc123def456...`)
   - `{short-sha}` - Short commit SHA (first 7 characters)
   - `pr-{number}` - For pull requests
5. **Build and Push** - Builds Docker image and pushes to registry (only on main/master)

### Image Versioning

Images are tagged with multiple versions for flexibility:

| Tag Format | Example | Description |
|------------|---------|-------------|
| `latest` | `article-service:latest` | Always points to latest main/master/finnishing-the-compulsory-assignment build |
| `{full-sha}` | `article-service:abc123def456...` | Full commit SHA for exact version |
| `{short-sha}` | `article-service:abc123d` | Short SHA for easier reference |
| `pr-{number}` | `article-service:pr-42` | Pull request builds (not pushed) |

### Build Caching

The pipeline uses GitHub Actions cache to speed up builds:
- **Cache Type:** `gha` (GitHub Actions cache)
- **Cache Mode:** `max` (maximum cache utilization)
- **Benefits:** Faster subsequent builds by reusing Docker layers

## Usage

### Pulling Images

After a successful build, you can pull images from GHCR:

```bash
# Login to GHCR (first time only)
echo $GITHUB_TOKEN | docker login ghcr.io -u USERNAME --password-stdin

# Pull latest images
docker pull ghcr.io/{owner}/happy-headlines/article-service:latest
docker pull ghcr.io/{owner}/happy-headlines/comment-service:latest
docker pull ghcr.io/{owner}/happy-headlines/profanity-service:latest
docker pull ghcr.io/{owner}/happy-headlines/draft-service:latest
docker pull ghcr.io/{owner}/happy-headlines/publisher-service:latest
docker pull ghcr.io/{owner}/happy-headlines/newsletter-service:latest
```

### Using Specific Versions

```bash
# Pull by commit SHA
docker pull ghcr.io/{owner}/happy-headlines/article-service:abc123def456789

# Pull by short SHA
docker pull ghcr.io/{owner}/happy-headlines/article-service:abc123d
```

### Updating docker-compose.yml

To use images from GHCR instead of local builds, update `docker-compose.yml`:

```yaml
article-service:
  image: ghcr.io/{owner}/happy-headlines/article-service:latest
  # Remove or comment out: build: ./ArticleService
  # ... rest of configuration
```

## Permissions

The workflow requires the following GitHub permissions:
- **Contents: read** - To checkout code
- **Packages: write** - To push images to GHCR

These are automatically granted via the `GITHUB_TOKEN` secret, which is available by default in GitHub Actions.

## Viewing Build Results

1. **GitHub Actions Tab** - Go to your repository → Actions tab
2. **Workflow Runs** - Click on a workflow run to see details
3. **Job Details** - Click on a job to see individual service builds
4. **Build Summary** - The `build-summary` job shows all built images with tags

## Troubleshooting

### Build Failures

**Issue:** Build fails with "permission denied"
- **Solution:** Ensure the repository has `packages: write` permission enabled

**Issue:** Docker build fails with "file not found"
- **Solution:** Verify Dockerfile paths in the workflow matrix are correct

**Issue:** Image push fails
- **Solution:** Check that `GITHUB_TOKEN` has package write permissions

### Image Not Found

**Issue:** Cannot pull image - "not found"
- **Solution:** 
  1. Verify the image was built successfully in Actions
  2. Check the image name matches exactly (case-sensitive)
  3. Ensure you're logged into GHCR: `docker login ghcr.io`

### Version Tagging Issues

**Issue:** `latest` tag not created
- **Solution:** Ensure you're pushing to `main`, `master`, or `finnishing-the-compulsory-assignment` branch (not a feature branch)

## Best Practices

1. **Always use specific versions in production** - Avoid `latest` tag for production deployments
2. **Test PR builds locally** - Pull request builds don't push, but you can test the build process
3. **Monitor build times** - Use build caching to reduce build duration
4. **Review build logs** - Check for warnings or deprecation notices
5. **Tag releases** - Consider adding semantic versioning tags for releases

## Security Considerations

- **Secrets:** The workflow uses `GITHUB_TOKEN` which is automatically scoped to the repository
- **Image Scanning:** Consider adding vulnerability scanning in the pipeline
- **Access Control:** GHCR images inherit repository visibility (private repos = private images)
- **Token Expiry:** `GITHUB_TOKEN` expires after the workflow completes

## Future Enhancements

Potential improvements to the pipeline:
- [ ] Add unit and integration tests before building
- [ ] Add security scanning (Trivy, Snyk) for Docker images
- [ ] Add deployment automation to staging/production
- [ ] Add semantic versioning support (v1.0.0, v1.1.0, etc.)
- [ ] Add multi-platform builds (ARM64, AMD64)
- [ ] Add build notifications (Slack, Teams, email)
- [ ] Add performance testing in the pipeline

## Related Documentation

- [FMEA Risk Analysis](./FMEA-CICD-Risk-Analysis.md) - Risk analysis for CI/CD implementation
- [Docker Compose Setup](../docker-compose.yml) - Local development setup
- [Service Documentation](../README.md) - General project documentation

---

**Last Updated:** 2025-01-06  
**Pipeline Version:** 1.0


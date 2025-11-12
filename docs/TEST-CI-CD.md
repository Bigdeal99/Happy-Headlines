# Testing the CI/CD Pipeline

## Quick Test Guide

Follow these steps to test if your CI/CD pipeline works:

### Step 1: Commit and Push the Workflow

```powershell
# Add all new files
git add .github/workflows/ci-cd.yml
git add docs/
git add README.md

# Commit
git commit -m "Add CI/CD pipeline with GitHub Actions"

# Push to trigger the workflow
git push origin DevOps-CI/CD
```

**Note:** If your main branch is called `finnishing-the-compulsory-assignment`, make sure to push to that branch instead.

### Step 2: Check GitHub Actions

1. Go to your GitHub repository in a web browser
2. Click on the **Actions** tab
3. You should see a workflow run called "CI/CD Pipeline" starting
4. Click on it to see the build progress

### Step 3: What to Expect

The workflow will:
- ✅ Build 6 Docker images in parallel (one for each service)
- ✅ Tag each image with multiple versions (latest, SHA, short-SHA)
- ✅ Push images to GitHub Container Registry (GHCR)
- ✅ Show a build summary at the end

### Step 4: Verify Images Were Pushed

After the workflow completes successfully:

1. Go to your GitHub repository
2. Click on **Packages** (on the right side, or in the top menu)
3. You should see 6 packages:
   - `happy-headlines/article-service`
   - `happy-headlines/comment-service`
   - `happy-headlines/profanity-service`
   - `happy-headlines/draft-service`
   - `happy-headlines/publisher-service`
   - `happy-headlines/newsletter-service`

### Step 5: Test Pulling an Image (Optional)

```powershell
# Login to GHCR (use your GitHub username and a Personal Access Token)
docker login ghcr.io -u YOUR_GITHUB_USERNAME

# Pull a test image
docker pull ghcr.io/YOUR_USERNAME/happy-headlines/article-service:latest
```

## Troubleshooting

### Workflow doesn't trigger
- **Check:** Make sure you pushed to a branch listed in the workflow (`main`, `master`, `finnishing-the-compulsory-assignment`, or `DevOps-CI/CD`)
- **Solution:** Update the workflow file to include your branch name

### Build fails
- **Check:** Look at the error message in GitHub Actions
- **Common issues:**
  - Dockerfile path incorrect
  - Missing dependencies
  - Permission issues with GHCR

### Images not visible in Packages
- **Check:** Make sure the workflow completed successfully (green checkmark)
- **Check:** Wait a few minutes - packages may take time to appear
- **Check:** Make sure you're looking at the correct GitHub account/organization

## Manual Trigger (Alternative)

If you want to test without pushing:

1. Go to **Actions** tab in GitHub
2. Click on "CI/CD Pipeline" workflow
3. Click "Run workflow" button (top right)
4. Select your branch
5. Click "Run workflow"

This will trigger the workflow manually!

---

**Expected Build Time:** 5-15 minutes (depending on GitHub Actions queue)


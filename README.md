# Happy-Headlines

A microservices-based news platform with caching layers, monitoring, and CI/CD automation.

## Architecture

See `docs/c4-l1.puml` for the updated C4 Level 1 diagram including tracing/logging.

## Services

- **ArticleService** - Article management with Redis caching and offline cache warming
- **CommentService** - Comment management with LRU cache (30-article limit)
- **ProfanityService** - Profanity filtering service
- **DraftService** - Draft article management with centralized logging
- **PublisherService** - Article publishing with RabbitMQ messaging
- **NewsletterService** - Newsletter generation service

## CI/CD Pipeline

This project includes automated CI/CD using GitHub Actions. Every push to `main`, `master`, or `finnishing-the-compulsory-assignment` branch automatically:

- Builds Docker images for all 6 services
- Tags images with version numbers (SHA, latest)
- Pushes images to GitHub Container Registry (GHCR)

**See [CI/CD Setup Documentation](docs/CI-CD-Setup.md) for details.**

### Quick Start

1. Push code to `main`, `master`, or `finnishing-the-compulsory-assignment` branch
2. GitHub Actions automatically builds and pushes images
3. Pull images from GHCR:
   ```bash
   docker pull ghcr.io/{owner}/happy-headlines/article-service:latest
   ```

## Documentation

- [CI/CD Setup](docs/CI-CD-Setup.md) - CI/CD pipeline documentation
- [FMEA Risk Analysis](docs/FMEA-CICD-Risk-Analysis.md) - Risk analysis for CI/CD implementation
- [C4 Architecture Diagram](docs/c4-l1.puml) - System architecture

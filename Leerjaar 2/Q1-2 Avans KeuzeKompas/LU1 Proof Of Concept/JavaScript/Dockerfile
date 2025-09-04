# Use a small Node image
FROM node:22-alpine AS base
WORKDIR /app

# Copy package manifests first to leverage Docker layer cache
COPY package*.json ./

# Install production dependencies (use package-lock if present)
RUN if [ -f package-lock.json ]; then \
  npm ci --only=production; \
    else \
  npm install --production; \
    fi

# Copy application sources
COPY . .

# Create a non-root user and give ownership of the app dir
RUN addgroup -S appgroup && adduser -S appuser -G appgroup && chown -R appuser:appgroup /app

USER appuser

# Default env config (app should respect PORT env var)
ENV NODE_ENV=production
ENV PORT=3000

# Expose the port your Express app listens on
EXPOSE 3000

# Basic healthcheck (expects the app to respond on /; adjust if needed)
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD wget -qO- --timeout=2 http://localhost:${PORT}/ || exit 1

# Start the app (expects a "start" script in package.json)
CMD ["npm", "start"]
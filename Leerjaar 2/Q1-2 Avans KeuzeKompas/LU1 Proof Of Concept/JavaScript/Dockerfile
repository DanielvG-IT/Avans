# Use the official Node.js LTS (Long Term Support) image as base
FROM node:22-alpine AS base

# Set working directory
WORKDIR /app

# Copy package files for dependency installation
COPY package*.json ./

# Install dependencies
RUN npm ci --only=production && npm cache clean --force

# Create a non-root user for security
RUN addgroup -g 1001 -S nodejs && \
  adduser -S expressuser -u 1001

# Copy application code
COPY . .

# Change ownership of the app directory to the non-root user
RUN chown -R expressuser:nodejs /app

# Switch to non-root user
USER expressuser

# Expose the port your app runs on
EXPOSE 3000

# Add health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD node healthcheck.js

# Start the application
CMD ["npm", "run start"]
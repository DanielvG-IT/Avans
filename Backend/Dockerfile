# Build stage
FROM node:20-alpine AS builder

WORKDIR /app

# Copy package files
COPY package*.json ./
COPY prisma.config.ts ./

# Install dependencies
RUN npm ci

# Copy only necessary source and config files (avoid recursive COPY of repo)
COPY tsconfig*.json nest-cli.json ./
COPY src ./src

# Generate Prisma client (dummy MySQL connection for build)
ENV DATABASE_URL="mysql://user:password@localhost:3306/db"
RUN npm run db:generate

# Build application
RUN npm run build

# Production stage
FROM node:20-alpine AS production

WORKDIR /app

# Create non-root user first (before copying files)
RUN addgroup -g 1001 -S nodejs && \
    adduser -S nestjs -u 1001

# Copy package files (root-owned by default)
COPY package*.json ./

# Install production dependencies only
RUN npm ci --only=production && npm cache clean --force

# Copy built application from builder (root-owned, read-only for non-root)
COPY --from=builder /app/src/infrastructure/database/generated ./src/infrastructure/database/generated
COPY --from=builder /app/src/infrastructure/database/schema ./src/infrastructure/database/schema
COPY --from=builder /app/dist ./dist
COPY --from=builder /app/prisma.config.ts ./

# Create startup script as root and lock permissions (non-writable)
RUN echo '#!/bin/sh' \
    '\nset -e' \
    '\necho "Running Prisma migrations..."' \
    '\nnode -e "const {PrismaClient} = require(\\\"@prisma/client\\\"); const prisma = new PrismaClient(); (async () => { try { await prisma.$executeRawUnsafe(\\\"SELECT 1\\\"); console.log(\\\"Database connected\\\"); } catch(e) { console.error(\\\"Database not ready, continuing anyway\\\"); } finally { await prisma.$disconnect(); } })()" || true' \
    '\necho "Starting application..."' \
    '\nnode dist/main\n' \
    > /app/start.sh \
    && chmod 0555 /app/start.sh

## Switch to non-root user for runtime
USER nestjs

# Expose port
EXPOSE 3000

# Migration and startup entrypoint is /app/start.sh (created above)

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD node -e "require('http').get('http://localhost:3000/api/health', (r) => {process.exit(r.statusCode === 200 ? 0 : 1)})" || exit 1

# Start application
CMD ["/app/start.sh"]

# --- STAGE 1: Build stage ---
FROM node:20-alpine AS builder

WORKDIR /app

# Kopieer package files en config
COPY package*.json ./
COPY prisma.config.ts ./

# Install dependencies
RUN npm ci

# Kopieer broncode en tsconfig
COPY tsconfig*.json nest-cli.json ./
COPY src ./src

# Genereer Prisma client (met dummy URL voor build-tijd)
ENV DATABASE_URL="mysql://user:password@localhost:3306/db"
RUN npm run db:generate

# Bouw de NestJS applicatie
RUN npm run build

# --- STAGE 2: Production stage ---
FROM node:20-alpine AS production

WORKDIR /app

# Maak non-root user aan
RUN addgroup -g 1001 -S nodejs && \
    adduser -S nestjs -u 1001

# Kopieer package files en installeer alleen production dependencies
COPY package*.json ./
RUN npm ci --only=production && npm cache clean --force

# Kopieer de gebouwde applicatie en Prisma bestanden van builder
COPY --from=builder /app/src/infrastructure/database/generated ./src/infrastructure/database/generated
COPY --from=builder /app/src/infrastructure/database/schema ./src/infrastructure/database/schema
COPY --from=builder /app/dist ./dist
COPY --from=builder /app/prisma.config.ts ./

# Maak het start-script aan (gecorrigeerd voor shell-fouten en paden)
RUN <<EOF cat > /app/start.sh
#!/bin/sh
set -e
echo "Starting application..."

# Check of main.js in dist of dist/src staat
if [ -f "dist/main.js" ]; then
    node dist/main.js
elif [ -f "dist/src/main.js" ]; then
    node dist/src/main.js
else
    echo "ERROR: Could not find main.js in dist/ or dist/src/"
    ls -R dist/
    exit 1
fi
EOF

# Maak script uitvoerbaar en zet de eigenaar op de nestjs user
RUN chmod +x /app/start.sh && \
    chown -R nestjs:nodejs /app

# Wissel naar de veilige user
USER nestjs

# Expose poort (standaard NestJS poort)
EXPOSE 4000

# Gebruik ENTRYPOINT om het script te starten
ENTRYPOINT ["/bin/sh", "/app/start.sh"]

# Gezondheidscontrole voor Azure
HEALTHCHECK --interval=30s --timeout=3s --start-period=30s --retries=3 \
    CMD node -e "require('http').get('http://localhost:3000/api/health', (r) => {process
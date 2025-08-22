FROM node:20-alpine
WORKDIR /app
COPY package.json ./
RUN npm install --production
COPY index.js ./
COPY prometheus.js ./
COPY azure.js ./
EXPOSE 3000
# Application Insights environment variable (will be set via docker-compose or runtime)
ENV APPLICATIONINSIGHTS_CONNECTION_STRING=""
CMD ["npm", "start"]

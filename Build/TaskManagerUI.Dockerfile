# Stage 1: Build the Angular application
FROM node:16-alpine AS build

WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install
COPY . .
RUN npm run build --prod

# Stage 2: Serve the built Angular app with Nginx
FROM nginx:alpine

# Copy the built Angular app from the previous stage
COPY --from=build /app/dist/task-manager /usr/share/nginx/html

# Expose port 4200 (default Angular development server port)
EXPOSE 4200

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]

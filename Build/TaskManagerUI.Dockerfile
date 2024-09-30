# Stage 1: Build the Angular application
FROM node:18-alpine AS build

WORKDIR /app
COPY Source/Ui/task-manager/package.json Source/Ui/task-manager/package-lock.json ./
RUN npm install
COPY Source/Ui/task-manager/. .
RUN npm run build --prod

# Stage 2: Serve the built Angular app with Nginx
FROM nginx:alpine

# Copy the built Angular app from the previous stage
COPY --from=build /app/dist/task-manager-ui/browser /usr/share/nginx/html

COPY Source/Ui/task-manager/cert.pem /etc/ssl/certs/cert.pem
COPY Source/Ui/task-manager/key.pem /etc/ssl/private/key.pem

COPY Source/Ui/task-manager/nginx.conf /etc/nginx/nginx.conf


EXPOSE 443
EXPOSE 80
# Start Nginx
CMD ["nginx", "-g", "daemon off;"]

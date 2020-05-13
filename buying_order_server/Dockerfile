FROM node
RUN npm install pm2 -g
COPY ./dist /app
CMD ["pm2-runtime", "app/bundle.js"]
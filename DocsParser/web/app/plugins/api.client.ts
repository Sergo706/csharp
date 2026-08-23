import { client } from '~~/client/client.gen';

export default defineNuxtPlugin(() => {
  client.setConfig({
    baseURL: '/',
  });
});
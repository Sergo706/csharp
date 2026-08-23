import { defineConfig } from '@hey-api/openapi-ts';

export default defineConfig({
  input: 'http://localhost:5113/openapi/v1.json',

  output: {
    path: './client',
    postProcess: ['prettier'],
  },

  plugins: [
    '@hey-api/client-nuxt',
    '@hey-api/typescript',

    {
      name: 'zod',
      types: {
        infer: true,
      },
    },

    {
      name: '@hey-api/sdk',
      validator: true,
      transformer: true,
    },
  ],
});
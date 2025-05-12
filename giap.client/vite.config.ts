import {fileURLToPath, URL} from 'node:url';

import {defineConfig} from 'vite';
import plugin from '@vitejs/plugin-react';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [plugin()],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    server: {
        proxy: {
            '^/api.*': { // regex to proxy any api call
                target: 'http://localhost:7179',
                secure: false
            }
        },
        port: 62858
    }
})

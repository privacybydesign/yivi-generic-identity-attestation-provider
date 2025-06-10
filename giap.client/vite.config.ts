import {fileURLToPath, URL} from 'node:url';

import {defineConfig} from 'vite';
import plugin from '@vitejs/plugin-react';

import commonjs from 'vite-plugin-commonjs'

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [
        plugin(),
        commonjs()
    ],
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

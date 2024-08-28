import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [react()],
    server: {
        proxy: {
            '/api': {
                target: process.env.services__bff__https__0 || process.env.services__api__https__0,
                secure: false,
                changeOrigin: true,
                ws: true
            },
            '/auth': {
                target: process.env.services__bff__https__0,
                secure: false,
                changeOrigin: true,
            }
        }
    }
})

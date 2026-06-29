import path from "node:path"
import { defineConfig } from "vitest/config"

export default defineConfig({
  root: __dirname,
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "."),
    },
  },
  test: {
    environment: "jsdom",
    setupFiles: [path.resolve(__dirname, "vitest.setup.ts")],
    globals: true,
    include: ["**/*.{test,spec}.{ts,tsx}"],
  },
})

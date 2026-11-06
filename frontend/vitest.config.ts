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
    coverage: {
      provider: "v8",
      reporter: ["text", "cobertura"],
      reportsDirectory: path.resolve(__dirname, "coverage"),
      include: ["lib/**/*.{ts,tsx}"],
      exclude: ["**/*.{test,spec}.{ts,tsx}", "**/*.d.ts", "**/types.ts", "**/index.ts"],
      thresholds: {
        lines: 10,
        functions: 10,
        statements: 10,
        branches: 5,
      },
    },
  },
})

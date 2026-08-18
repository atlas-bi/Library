import "@testing-library/jest-dom/vitest"
import { vi } from "vitest"

global.ResizeObserver = class ResizeObserver {
  observe = vi.fn()
  unobserve = vi.fn()
  disconnect = vi.fn()
}

vi.mock("next/navigation", async () => {
  const actual = await vi.importActual<typeof import("next/navigation")>("next/navigation")

  return {
    ...actual,
    useRouter: () => ({
      back: vi.fn(),
      forward: vi.fn(),
      prefetch: vi.fn(),
      push: vi.fn(),
      refresh: vi.fn(),
      replace: vi.fn(),
    }),
  }
})

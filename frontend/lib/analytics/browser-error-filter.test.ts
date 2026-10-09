import { describe, expect, it } from "vitest"
import { shouldIgnoreBrowserError } from "./browser-error-filter"

describe("shouldIgnoreBrowserError", () => {
  it("ignores extension URLs", () => {
    expect(
      shouldIgnoreBrowserError({
        errorMsg: "x",
        url: "chrome-extension://abc/page.js",
      }),
    ).toBe(true)
  })

  it("ignores cross-origin script errors without URL", () => {
    expect(
      shouldIgnoreBrowserError({
        errorMsg: "Script error.",
        url: "",
      }),
    ).toBe(true)
  })

  it("keeps real application errors", () => {
    expect(
      shouldIgnoreBrowserError({
        errorMsg: "Cannot read property 'x'",
        url: "http://localhost:3001/terms",
        trace: "Error: ...",
      }),
    ).toBe(false)
  })
})

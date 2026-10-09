"use client"

import { useEffect } from "react"
import { reportError } from "@/lib/error-reporting"

export default function GlobalError({
  error,
  reset,
}: {
  error: Error & { digest?: string }
  reset: () => void
}) {
  useEffect(() => {
    reportError(error, { boundary: "app/global-error" })
  }, [error])

  return (
    <html lang="en">
      <body className="bg-background text-foreground">
        <main className="mx-auto flex min-h-screen w-full max-w-3xl items-center px-4">
          <div className="space-y-4">
            <h1 className="text-2xl font-semibold">Application error</h1>
            <p className="text-sm text-muted-foreground">
              An unexpected error occurred. Please retry or return to the home page.
            </p>
            <div className="flex gap-2">
              <button
                className="inline-flex h-8 items-center justify-center rounded-lg border border-transparent bg-primary px-3 text-sm font-medium text-primary-foreground"
                onClick={reset}
                type="button"
              >
                Retry
              </button>
              <a
                className="inline-flex h-8 items-center justify-center rounded-lg border border-border px-3 text-sm font-medium"
                href="/"
              >
                Home
              </a>
            </div>
          </div>
        </main>
      </body>
    </html>
  )
}

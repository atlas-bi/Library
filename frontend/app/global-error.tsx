"use client"

import Link from "next/link"
import { Button } from "@/components/ui/button"
import { reportError } from "@/lib/error-reporting"

export default function GlobalError({
  error,
  reset,
}: {
  error: Error & { digest?: string }
  reset: () => void
}) {
  reportError(error, { boundary: "app/global-error" })

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
              <Button onClick={reset}>Retry</Button>
              <Button asChild variant="outline">
                <Link href="/">Home</Link>
              </Button>
            </div>
          </div>
        </main>
      </body>
    </html>
  )
}

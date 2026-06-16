"use client"

import { useRouter, useSearchParams } from "next/navigation"
import { Suspense, useEffect } from "react"

import { Card, CardContent } from "@/components/ui/card"
import { Skeleton } from "@/components/ui/skeleton"

function CallbackHandler() {
  const router = useRouter()
  const params = useSearchParams()

  useEffect(() => {
    const token = params.get("token")
    if (token) {
      const maxAgeSeconds = 8 * 60 * 60
      try {
        // Prefer the Cookie Store API when available.
        if ("cookieStore" in window) {
          void window.cookieStore.set({
            name: "atlas_token",
            value: token,
            path: "/",
            expires: Date.now() + maxAgeSeconds * 1000,
            sameSite: "lax",
          })
        } else {
          // biome-ignore lint/suspicious/noDocumentCookie: Fallback for browsers without Cookie Store API.
          document.cookie = `atlas_token=${token}; path=/; max-age=${maxAgeSeconds}; SameSite=Lax`
        }
      } catch {
        // biome-ignore lint/suspicious/noDocumentCookie: Final fallback if Cookie Store API throws.
        document.cookie = `atlas_token=${token}; path=/; max-age=${maxAgeSeconds}; SameSite=Lax`
      }
      router.replace("/")
    } else {
      router.replace("/auth/login")
    }
  }, [params, router])

  return (
    <div className="flex items-center justify-center min-h-screen bg-background px-6">
      <Card className="w-full max-w-sm">
        <CardContent className="py-10 flex flex-col items-center text-center gap-4">
          <Skeleton className="h-8 w-8 rounded-full" />
          <p className="text-muted-foreground text-sm">Signing you in...</p>
        </CardContent>
      </Card>
    </div>
  )
}

export default function CallbackPage() {
  return (
    <Suspense>
      <CallbackHandler />
    </Suspense>
  )
}

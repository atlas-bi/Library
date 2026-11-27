"use client"

import { useRouter } from "next/navigation"
import { useEffect, useState } from "react"

export function HomeSearchClient({ placeholder = "type to search.." }: { placeholder?: string }) {
  const router = useRouter()
  const [query, setQuery] = useState("")

  useEffect(() => {
    const trimmed = query.trim()
    if (trimmed.length === 0) {
      return
    }

    const timeoutId = window.setTimeout(() => {
      router.push(`/search?q=${encodeURIComponent(trimmed)}`)
    }, 250)

    return () => window.clearTimeout(timeoutId)
  }, [query, router])

  return (
    <form action="/search" method="get" className="w-full flex-1">
      <input
        type="text"
        name="q"
        value={query}
        onChange={(event) => setQuery(event.target.value)}
        placeholder={placeholder}
        maxLength={80}
        className="inline-flex h-10 w-full items-center rounded-md border border-input bg-background px-4 py-2 text-sm font-normal text-muted-foreground shadow-none outline-none transition-colors placeholder:font-normal placeholder:text-muted-foreground hover:bg-accent hover:text-accent-foreground focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2"
      />
    </form>
  )
}

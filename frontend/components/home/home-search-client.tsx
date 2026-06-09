"use client"

import { Search } from "lucide-react"
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
    <form action="/search" method="get" className="relative flex w-full items-center">
      <div className="atlas-home-search-icon pointer-events-none absolute left-3">
        <Search className="h-4 w-4" />
      </div>
      <input
        type="text"
        name="q"
        value={query}
        onChange={(event) => setQuery(event.target.value)}
        placeholder={placeholder}
        maxLength={80}
        className="atlas-home-search-shell h-10 w-full bg-white pl-10 pr-4 text-sm shadow-none outline-none"
      />
    </form>
  )
}

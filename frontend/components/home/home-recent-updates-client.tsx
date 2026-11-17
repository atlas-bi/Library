"use client"

import { useEffect, useState } from "react"
import { loadRecentUpdatesAction } from "@/app/home/recent-updates-actions"
import { HomeRecentUpdatesView } from "@/components/home/home-recent-updates-view"
import type { RecentUpdatesBundle } from "@/lib/home/recent-updates-types"

type LoadState =
  | { status: "loading" }
  | { status: "error"; error: "auth_required" | "unknown" }
  | { status: "ready"; data: RecentUpdatesBundle | null }

export function HomeRecentUpdatesClient() {
  const [state, setState] = useState<LoadState>({ status: "loading" })

  useEffect(() => {
    let cancelled = false

    loadRecentUpdatesAction()
      .then((result) => {
        if (cancelled) return

        if (result.error === "auth_required") {
          setState({ status: "error", error: "auth_required" })
          return
        }

        if (result.error || !result.data) {
          setState({ status: "error", error: "unknown" })
          return
        }

        setState({ status: "ready", data: result.data })
      })
      .catch(() => {
        if (!cancelled) {
          setState({ status: "error", error: "unknown" })
        }
      })

    return () => {
      cancelled = true
    }
  }, [])

  if (state.status === "loading") {
    return <HomeRecentUpdatesView status="loading" />
  }

  if (state.status === "error") {
    return <HomeRecentUpdatesView status="error" error={state.error} />
  }

  return <HomeRecentUpdatesView status="ready" data={state.data} />
}

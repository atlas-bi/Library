"use client"

import { useEffect, useState } from "react"
import { loadRecentTermsAction } from "@/app/home/actions"
import { HomeRecentTermsView } from "@/components/home/home-recent-terms-view"
import type { RecentTermsDto } from "@/lib/terms/types"

type LoadState =
  | { status: "loading" }
  | { status: "error"; error: "auth_required" | "unknown" }
  | { status: "ready"; data: RecentTermsDto | null }

export function HomeRecentTermsClient() {
  const [state, setState] = useState<LoadState>({ status: "loading" })

  useEffect(() => {
    let cancelled = false

    loadRecentTermsAction()
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
    return <HomeRecentTermsView status="loading" />
  }

  if (state.status === "error") {
    return <HomeRecentTermsView status="error" error={state.error} />
  }

  return <HomeRecentTermsView status="ready" data={state.data} />
}

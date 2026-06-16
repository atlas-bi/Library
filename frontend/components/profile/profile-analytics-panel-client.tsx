"use client"

import { useEffect, useState } from "react"
import { loadProfileAnalyticsAction } from "@/app/profile/actions"
import { ProfileAnalyticsPanelView } from "@/components/profile/profile-analytics-panel-view"

export function ProfileAnalyticsPanelClient({ id, type }: { id: number; type: string }) {
  const [status, setStatus] = useState<"loading" | "ready" | "error">("loading")
  const [error, setError] = useState<string | null>(null)

  const [data, setData] =
    useState<Awaited<ReturnType<typeof loadProfileAnalyticsAction>>["data"]>(null)

  useEffect(() => {
    let cancelled = false
    setStatus("loading")
    setError(null)

    void loadProfileAnalyticsAction(id, type).then((result) => {
      if (cancelled) return
      if (!result.data) {
        setStatus("error")
        setError(result.error ?? "unknown")
        return
      }
      setData(result.data)
      setStatus("ready")
    })

    return () => {
      cancelled = true
    }
  }, [id, type])

  if (status === "loading") {
    return <p className="text-sm text-muted-foreground">Loading profile...</p>
  }

  if (status === "error" || !data) {
    return <p className="text-sm text-muted-foreground">Unable to load profile analytics.</p>
  }

  return <ProfileAnalyticsPanelView data={data} />
}

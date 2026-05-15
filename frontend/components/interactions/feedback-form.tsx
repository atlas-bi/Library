"use client"

import { useState, useTransition } from "react"
import { submitFeedbackAction } from "@/app/interactions/actions"
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"

export function FeedbackForm({ reportName, reportUrl }: { reportName: string; reportUrl: string }) {
  const [description, setDescription] = useState("")
  const [message, setMessage] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [pending, startTransition] = useTransition()

  return (
    <form
      className="space-y-3"
      onSubmit={(event) => {
        event.preventDefault()
        setError(null)
        setMessage(null)
        const trimmed = description.trim()
        if (!trimmed) {
          setError("Please describe the issue.")
          return
        }
        startTransition(() => {
          void (async () => {
            const result = await submitFeedbackAction({
              reportName,
              reportUrl,
              description: trimmed,
            })
            if (result.error) {
              setError(result.error)
              return
            }
            setMessage("Feedback submitted.")
            setDescription("")
          })()
        })
      }}
    >
      <div className="space-y-2">
        <Label htmlFor="feedback-description">Feedback</Label>
        <textarea
          id="feedback-description"
          value={description}
          onChange={(event) => {
            setDescription(event.target.value)
          }}
          rows={4}
          className="w-full rounded-lg border border-input bg-transparent px-2.5 py-2 text-sm outline-none focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50"
          placeholder="Describe the issue or suggestion…"
        />
      </div>
      {error ? <p className="text-sm text-destructive">{error}</p> : null}
      {message ? <p className="text-sm text-muted-foreground">{message}</p> : null}
      <Button type="submit" size="sm" disabled={pending}>
        {pending ? "Sending…" : "Submit feedback"}
      </Button>
    </form>
  )
}

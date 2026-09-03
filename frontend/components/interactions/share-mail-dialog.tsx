"use client"

import { Share2 } from "lucide-react"
import { useCallback, useEffect, useState, useTransition } from "react"
import { searchRecipientsAction, sendShareMailAction } from "@/app/interactions/actions"
import { InteractionTooltip } from "@/components/interactions/interaction-tooltip"
import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import type { ShareRecipientInput } from "@/lib/interactions/types"

type SelectedRecipient = ShareRecipientInput & { name: string }

export function ShareMailDialog({
  shareName,
  shareUrl,
  iconOnly = false,
  variant = "default",
  open: controlledOpen,
  onOpenChange,
  showTrigger = true,
}: {
  shareName: string
  shareUrl: string
  iconOnly?: boolean
  variant?: "default" | "footer"
  open?: boolean
  onOpenChange?: (open: boolean) => void
  showTrigger?: boolean
}) {
  const [internalOpen, setInternalOpen] = useState(false)
  const open = controlledOpen ?? internalOpen
  const setOpen = onOpenChange ?? setInternalOpen
  const [query, setQuery] = useState("")
  const [suggestions, setSuggestions] = useState<
    Array<{ id: number; name: string; type: string; email?: string | null }>
  >([])
  const [selected, setSelected] = useState<SelectedRecipient[]>([])
  const [subject, setSubject] = useState(`[Share] ${shareName}`)
  const [message, setMessage] = useState("")
  const [status, setStatus] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [pending, startTransition] = useTransition()

  const fetcher = useCallback((q: string) => searchRecipientsAction(q, true), [])

  useEffect(() => {
    if (!open) return
    setSubject(`[Share] ${shareName}`)
    setMessage(
      `Hi!\n\nI would like to share this report with you.\n\n[${shareName}](${shareUrl})\n\nCheck it out sometime!\nRegards!`,
    )
    setError(null)
    setStatus(null)
  }, [open, shareName, shareUrl])

  useEffect(() => {
    const trimmed = query.trim()
    if (!trimmed) {
      setSuggestions([])
      return
    }
    const handle = window.setTimeout(() => {
      void fetcher(trimmed).then(setSuggestions)
    }, 280)
    return () => {
      window.clearTimeout(handle)
    }
  }, [query, fetcher])

  const addRecipient = (item: { id: number; name: string; type: string }) => {
    const recipientType = item.type.toLowerCase().startsWith("g") ? "g" : "u"
    setSelected((prev) => {
      if (prev.some((r) => r.userId === item.id && r.type === recipientType)) return prev
      return [...prev, { userId: item.id, type: recipientType, name: item.name }]
    })
    setQuery("")
    setSuggestions([])
  }

  const tooltipPlacement = variant === "footer" ? "footer" : "rail"

  const trigger = iconOnly ? (
    variant === "footer" ? (
      <button
        type="button"
        className="inline-flex cursor-pointer items-center text-[var(--atlas-home-muted)] hover:text-[var(--atlas-home-link)]"
      >
        <Share2 className="h-4 w-4" strokeWidth={1.8} />
        <span className="sr-only">Share</span>
      </button>
    ) : (
      <Button type="button" variant="ghost" size="icon" className="atlas-action-rail-button">
        <Share2 className="size-5" />
        <span className="sr-only">Share</span>
      </Button>
    )
  ) : (
    <Button type="button" variant="outline" size="sm">
      Share
    </Button>
  )

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      {showTrigger ? (
        <InteractionTooltip label="Share" placement={tooltipPlacement}>
          <DialogTrigger asChild>{trigger}</DialogTrigger>
        </InteractionTooltip>
      ) : null}
      <DialogContent className="max-w-lg">
        <DialogHeader>
          <DialogTitle>Share {shareName}</DialogTitle>
        </DialogHeader>
        <div className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="share-recipient-search">Recipients</Label>
            <Input
              id="share-recipient-search"
              value={query}
              onChange={(e) => {
                setQuery(e.target.value)
              }}
              placeholder="Search users or groups…"
              autoComplete="off"
            />
            {suggestions.length > 0 ? (
              <ul className="max-h-32 overflow-auto rounded-md border text-sm">
                {suggestions.map((item) => (
                  <li key={`${item.type}-${item.id}`} className="border-b last:border-b-0">
                    <button
                      type="button"
                      className="w-full px-3 py-2 text-left hover:bg-muted"
                      onClick={() => {
                        addRecipient(item)
                      }}
                    >
                      {item.name}
                      {item.email ? (
                        <span className="ml-2 text-xs text-muted-foreground">{item.email}</span>
                      ) : null}
                    </button>
                  </li>
                ))}
              </ul>
            ) : null}
            {selected.length > 0 ? (
              <ul className="flex flex-wrap gap-2">
                {selected.map((r) => (
                  <li
                    key={`${r.type}-${r.userId}`}
                    className="rounded-md border bg-muted px-2 py-1 text-xs"
                  >
                    {r.name}
                    <button
                      type="button"
                      className="ml-1 text-muted-foreground hover:text-foreground"
                      onClick={() => {
                        setSelected((prev) =>
                          prev.filter((x) => !(x.userId === r.userId && x.type === r.type)),
                        )
                      }}
                    >
                      ×
                    </button>
                  </li>
                ))}
              </ul>
            ) : null}
          </div>
          <div className="space-y-2">
            <Label htmlFor="share-subject">Subject</Label>
            <Input
              id="share-subject"
              value={subject}
              onChange={(e) => {
                setSubject(e.target.value)
              }}
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="share-message">Message</Label>
            <textarea
              id="share-message"
              value={message}
              onChange={(e) => {
                setMessage(e.target.value)
              }}
              rows={4}
              className="w-full rounded-lg border border-input bg-transparent px-2.5 py-2 text-sm outline-none"
            />
          </div>
          {error ? <p className="text-sm text-destructive">{error}</p> : null}
          {status ? <p className="text-sm text-muted-foreground">{status}</p> : null}
          <Button
            type="button"
            disabled={pending || selected.length === 0}
            onClick={() => {
              setError(null)
              setStatus(null)
              const text = message.trim()
              startTransition(() => {
                void (async () => {
                  const result = await sendShareMailAction({
                    to: selected.map(({ userId, type }) => ({ userId, type })),
                    subject: subject.trim() || `[Share] ${shareName}`,
                    message: text ? `<p>${text}</p>` : `<p>Shared via Atlas Library</p>`,
                    text: text || "Shared via Atlas Library",
                    share: true,
                    shareName,
                    shareUrl,
                  })
                  if (result.error) {
                    setError(result.error)
                    return
                  }
                  setStatus(result.data?.message ?? "Successfully shared.")
                })()
              })
            }}
          >
            {pending ? "Sending…" : "Send"}
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  )
}

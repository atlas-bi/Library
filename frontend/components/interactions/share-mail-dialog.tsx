"use client"

import { Bold, Code, ExternalLink, Eye, Heading2, Italic, Link, List } from "lucide-react"
import { Share2 } from "lucide-react"
import { useCallback, useEffect, useRef, useState, useTransition } from "react"
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
  const [preview, setPreview] = useState(false)
  const textareaRef = useRef<HTMLTextAreaElement>(null)

  const fetcher = useCallback((q: string) => searchRecipientsAction(q, true), [])

  // Wrap selection at cursor with markdown syntax
  const wrapSelection = (before: string, after: string = before) => {
    const el = textareaRef.current
    if (!el) return
    const start = el.selectionStart
    const end = el.selectionEnd
    const selected = message.slice(start, end)
    const newMessage =
      message.slice(0, start) + before + selected + after + message.slice(end)
    setMessage(newMessage)
    // restore cursor
    requestAnimationFrame(() => {
      el.focus()
      el.setSelectionRange(start + before.length, start + before.length + selected.length)
    })
  }

  const insertAtLineStart = (prefix: string) => {
    const el = textareaRef.current
    if (!el) return
    const start = el.selectionStart
    const lineStart = message.lastIndexOf("\n", start - 1) + 1
    const newMessage = message.slice(0, lineStart) + prefix + message.slice(lineStart)
    setMessage(newMessage)
    requestAnimationFrame(() => {
      el.focus()
      el.setSelectionRange(start + prefix.length, start + prefix.length)
    })
  }

  useEffect(() => {
    if (!open) return
    setSubject(`[Share] ${shareName}`)
    setMessage(
      `Hi!\n\nI would like to share this report with you.\n\n[${shareName}](${shareUrl})\n\nCheck it out sometime!\nRegards!`,
    )
    setError(null)
    setStatus(null)
    setPreview(false)
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
      <DialogContent className="max-w-[700px]" aria-describedby={undefined}>
        <DialogHeader>
          <DialogTitle>Share {shareName}</DialogTitle>
        </DialogHeader>
        <div className="space-y-4">
          {/* ── To: ───────────────────────────────── */}
          <div className="space-y-2">
            <Label htmlFor="share-recipient-search">To:</Label>
            {/* Selected recipient tags sit above the search input, exactly like Razor */}
            {selected.length > 0 ? (
              <ul className="flex flex-wrap gap-1.5">
                {selected.map((r) => (
                  <li
                    key={`${r.type}-${r.userId}`}
                    className="inline-flex items-center gap-1 rounded-md border bg-muted px-2 py-1 text-xs"
                  >
                    {r.name}
                    <button
                      type="button"
                      className="text-muted-foreground hover:text-foreground"
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
            <div className="relative">
              <Input
                id="share-recipient-search"
                value={query}
                onChange={(e) => { setQuery(e.target.value) }}
                placeholder="search for someone.."
                autoComplete="off"
              />
              {suggestions.length > 0 ? (
                <ul className="absolute left-0 right-0 top-full z-10 mt-1 max-h-40 overflow-auto rounded-md border bg-popover text-sm shadow-md">
                  {suggestions.map((item) => (
                    <li key={`${item.type}-${item.id}`} className="border-b last:border-b-0">
                      <button
                        type="button"
                        className="w-full px-3 py-2 text-left hover:bg-muted"
                        onClick={() => { addRecipient(item) }}
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
            </div>
          </div>

          {/* ── Subject ───────────────────────────── */}
          <div className="space-y-2">
            <Label htmlFor="share-subject">Subject</Label>
            <Input
              id="share-subject"
              value={subject}
              onChange={(e) => { setSubject(e.target.value) }}
            />
          </div>

          {/* ── Message (with Razor-style toolbar) ─ */}
          <div className="space-y-2">
            <Label htmlFor="share-message">Message</Label>
            <div className="overflow-hidden rounded-lg border border-input">
              {/* Toolbar */}
              <div className="flex items-center gap-0.5 border-b bg-muted/40 px-2 py-1">
                <ToolbarBtn title="Bold" onClick={() => wrapSelection("**")}>
                  <Bold className="size-3.5" />
                </ToolbarBtn>
                <ToolbarBtn title="Italic" onClick={() => wrapSelection("_")}>
                  <Italic className="size-3.5" />
                </ToolbarBtn>
                <ToolbarBtn title="Heading" onClick={() => insertAtLineStart("## ")}>
                  <Heading2 className="size-3.5" />
                </ToolbarBtn>
                <ToolbarBtn title="Blockquote" onClick={() => insertAtLineStart("> ")}>
                  <span className="font-bold">&ldquo;&rdquo;</span>
                </ToolbarBtn>
                <ToolbarBtn title="Code" onClick={() => wrapSelection("`")}>
                  <Code className="size-3.5" />
                </ToolbarBtn>
                <ToolbarBtn title="Ordered list" onClick={() => insertAtLineStart("1. ")}>
                  <List className="size-3.5" />
                </ToolbarBtn>
                <ToolbarBtn
                  title="Insert link"
                  onClick={() => wrapSelection("[", "](url)")}
                >
                  <Link className="size-3.5" />
                </ToolbarBtn>
                <div className="mx-1 h-4 w-px bg-border" />
                <ToolbarBtn
                  title={preview ? "Edit" : "Preview"}
                  onClick={() => setPreview((p) => !p)}
                  active={preview}
                >
                  <Eye className="size-3.5" />
                </ToolbarBtn>
              </div>

              {preview ? (
                <div
                  className="min-h-[9rem] whitespace-pre-wrap px-3 py-2 text-sm"
                  // Simple preview: render newlines, keep markdown as plain text
                >
                  {message || <span className="text-muted-foreground">Nothing to preview.</span>}
                </div>
              ) : (
                <textarea
                  ref={textareaRef}
                  id="share-message"
                  value={message}
                  onChange={(e) => { setMessage(e.target.value) }}
                  rows={7}
                  className="w-full resize-none bg-transparent px-3 py-2 text-sm outline-none"
                />
              )}
            </div>
          </div>

          {error ? <p className="text-sm text-destructive">{error}</p> : null}
          {status ? <p className="text-sm text-muted-foreground">{status}</p> : null}

          <div className="flex">
            <Button
              type="button"
              variant="outline"
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
        </div>
      </DialogContent>
    </Dialog>
  )
}

// ── Toolbar button helper ────────────────────────────────────────────────────
function ToolbarBtn({
  title,
  onClick,
  children,
  active = false,
}: {
  title: string
  onClick: () => void
  children: React.ReactNode
  active?: boolean
}) {
  return (
    <button
      type="button"
      title={title}
      onClick={onClick}
      className={`rounded p-1 text-muted-foreground hover:bg-background hover:text-foreground ${
        active ? "bg-background text-foreground" : ""
      }`}
    >
      {children}
    </button>
  )
}

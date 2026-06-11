"use client"

import { Accessibility } from "lucide-react"
import { useCallback, useEffect, useState, useTransition } from "react"
import { searchRecipientsAction, submitAccessRequestAction } from "@/app/interactions/actions"
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
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip"

export function RequestAccessDialog({
  reportName,
  reportUrl,
  iconOnly = true,
  variant = "default",
}: {
  reportName: string
  reportUrl: string
  iconOnly?: boolean
  variant?: "default" | "footer"
}) {
  const [open, setOpen] = useState(false)
  const [query, setQuery] = useState("")
  const [directorName, setDirectorName] = useState("")
  const [suggestions, setSuggestions] = useState<
    Array<{ id: number; name: string; type: string; email?: string | null }>
  >([])
  const [status, setStatus] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [pending, startTransition] = useTransition()

  const fetcher = useCallback((q: string) => searchRecipientsAction(q, false), [])

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

  const trigger =
    iconOnly && variant === "footer" ? (
      <button
        type="button"
        className="inline-flex cursor-pointer items-center text-[var(--atlas-home-muted)] hover:text-[var(--atlas-home-link)]"
      >
        <Accessibility className="h-4 w-4" strokeWidth={1.8} />
        <span className="sr-only">Request access</span>
      </button>
    ) : iconOnly ? (
      <Button type="button" variant="ghost" size="icon" className="size-10">
        <Accessibility className="size-5" />
        <span className="sr-only">Request access</span>
      </Button>
    ) : (
      <Button type="button" variant="outline" size="sm">
        Request access
      </Button>
    )

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <TooltipProvider>
        <Tooltip>
          <TooltipTrigger asChild>
            <DialogTrigger asChild>{trigger}</DialogTrigger>
          </TooltipTrigger>
          <TooltipContent side="right">Request access</TooltipContent>
        </Tooltip>
      </TooltipProvider>
      <DialogContent className="max-w-lg">
        <DialogHeader>
          <DialogTitle>Request report access</DialogTitle>
        </DialogHeader>
        <form
          className="space-y-4"
          onSubmit={(event) => {
            event.preventDefault()
            setError(null)
            setStatus(null)
            const trimmedDirector = directorName.trim()
            if (!trimmedDirector) {
              setError("Director is required.")
              return
            }
            startTransition(() => {
              void (async () => {
                const result = await submitAccessRequestAction({
                  reportName,
                  reportUrl,
                  directorName: trimmedDirector,
                })
                if (result.error) {
                  setError(result.error)
                  return
                }
                setStatus("Your request has been submitted.")
                setDirectorName("")
                setQuery("")
                setSuggestions([])
              })()
            })
          }}
        >
          <p className="text-sm text-muted-foreground">{reportName}</p>
          <div className="space-y-2">
            <Label htmlFor="director-search">Find your director</Label>
            <Input
              id="director-search"
              value={query}
              onChange={(event) => setQuery(event.target.value)}
              placeholder="Type to search…"
              autoComplete="off"
            />
            {suggestions.length > 0 ? (
              <ul className="max-h-40 overflow-y-auto rounded-md border">
                {suggestions.map((item) => (
                  <li key={`${item.type}-${item.id}`}>
                    <button
                      type="button"
                      className="flex w-full flex-col px-3 py-2 text-left text-sm hover:bg-muted"
                      onClick={() => {
                        setDirectorName(item.name)
                        setQuery(item.name)
                        setSuggestions([])
                      }}
                    >
                      <span className="font-medium">{item.name}</span>
                      {item.email ? (
                        <span className="text-xs text-muted-foreground">{item.email}</span>
                      ) : null}
                    </button>
                  </li>
                ))}
              </ul>
            ) : null}
            {directorName ? (
              <p className="text-sm text-muted-foreground">
                Selected director: <strong>{directorName}</strong>
              </p>
            ) : null}
          </div>
          {error ? <p className="text-sm text-destructive">{error}</p> : null}
          {status ? <p className="text-sm text-muted-foreground">{status}</p> : null}
          <Button type="submit" size="sm" disabled={pending}>
            {pending ? "Submitting…" : "Request access"}
          </Button>
        </form>
      </DialogContent>
    </Dialog>
  )
}

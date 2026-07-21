"use client"

import { ArrowLeft, ArrowRight, ChevronDown, ChevronUp, X } from "lucide-react"
import Link from "next/link"
import type { Dispatch, SetStateAction } from "react"
import { useCallback, useEffect, useMemo, useState, useTransition } from "react"
import {
  createCollectionAction,
  searchCollectionReportsAction,
  searchCollectionTermsAction,
  updateCollectionAction,
} from "@/app/collections/actions"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Switch } from "@/components/ui/switch"
import type { CollectionDetailDto, CollectionTermDto } from "@/lib/collections/types"

type PickerRow = { id: number; label: string; subtitle?: string | null }

function termIdFromDto(term: CollectionTermDto): number | null {
  if (typeof term.id === "number") return term.id
  if (typeof term.termId === "number") return term.termId
  return null
}

function sortByRank<T extends { rank?: number }>(items: T[]): T[] {
  return [...items].sort((a, b) => (a.rank ?? 0) - (b.rank ?? 0))
}

function useDebouncedTypeahead(
  query: string,
  fetcher: (q: string) => Promise<{ id: number; name: string; description?: string | null }[]>,
) {
  const [results, setResults] = useState<
    { id: number; name: string; description?: string | null }[]
  >([])
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    const trimmed = query.trim()
    if (!trimmed) {
      setResults([])
      setLoading(false)
      return
    }

    setLoading(true)
    const handle = window.setTimeout(() => {
      void fetcher(trimmed)
        .then(setResults)
        .catch(() => {
          setResults([])
        })
        .finally(() => {
          setLoading(false)
        })
    }, 280)

    return () => {
      window.clearTimeout(handle)
    }
  }, [query, fetcher])

  return { results, loading }
}

function LinkedItemTags({
  rows,
  onRemove,
  onMove,
}: {
  rows: PickerRow[]
  onRemove: (id: number) => void
  onMove: (id: number, direction: "up" | "down") => void
}) {
  if (rows.length === 0) {
    return <p className="text-sm text-muted-foreground">None selected.</p>
  }

  return (
    <ul className="flex flex-col gap-2">
      {rows.map((row, index) => (
        <li
          key={row.id}
          className="flex flex-wrap items-center gap-2 rounded-lg border bg-muted/20 px-3 py-2"
        >
          <Badge variant="secondary" className="max-w-full truncate font-normal">
            {row.label}
          </Badge>
          <div className="ml-auto flex items-center gap-0.5">
            <Button
              type="button"
              variant="ghost"
              size="icon-xs"
              aria-label={`Move ${row.label} up`}
              disabled={index === 0}
              onClick={() => {
                onMove(row.id, "up")
              }}
            >
              <ChevronUp className="size-3.5" />
            </Button>
            <Button
              type="button"
              variant="ghost"
              size="icon-xs"
              aria-label={`Move ${row.label} down`}
              disabled={index === rows.length - 1}
              onClick={() => {
                onMove(row.id, "down")
              }}
            >
              <ChevronDown className="size-3.5" />
            </Button>
            <Button
              type="button"
              variant="ghost"
              size="icon-xs"
              aria-label={`Remove ${row.label}`}
              onClick={() => {
                onRemove(row.id)
              }}
            >
              <X className="size-3.5" />
            </Button>
          </div>
        </li>
      ))}
    </ul>
  )
}

const textareaClassName =
  "w-full rounded-lg border border-input bg-transparent px-3 py-2 text-sm outline-none focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50"

export function CollectionForm({
  mode,
  collectionId,
  initial,
  cancelHref,
}: {
  mode: "create" | "edit"
  collectionId?: number
  initial?: CollectionDetailDto | null
  cancelHref: string
}) {
  const [name, setName] = useState(initial?.name ?? "")
  const [description, setDescription] = useState(initial?.description ?? "")
  const [purpose, setPurpose] = useState(initial?.purpose ?? "")
  const [hidden, setHidden] = useState((initial?.hidden ?? "N").toUpperCase() === "Y")

  const initialTermRows = useMemo((): PickerRow[] => {
    const terms = initial?.terms ? sortByRank(initial.terms) : []
    const rows: PickerRow[] = []
    for (const term of terms) {
      const id = termIdFromDto(term)
      if (id == null) continue
      rows.push({
        id,
        label: term.name?.trim() || `Term ${id}`,
        subtitle: term.summary ?? null,
      })
    }
    return rows
  }, [initial?.terms])

  const initialReportRows = useMemo(() => {
    const reports = initial?.reports ? sortByRank(initial.reports) : []
    return reports.map((report) => ({
      id: report.id,
      label: report.name?.trim() || `Report ${report.id}`,
      subtitle: report.description ?? null,
    }))
  }, [initial?.reports])

  const [termRows, setTermRows] = useState<PickerRow[]>(initialTermRows)
  const [reportRows, setReportRows] = useState<PickerRow[]>(initialReportRows)

  const [termQuery, setTermQuery] = useState("")
  const [reportQuery, setReportQuery] = useState("")

  const termFetcher = useCallback((q: string) => searchCollectionTermsAction(q), [])
  const reportFetcher = useCallback((q: string) => searchCollectionReportsAction(q), [])

  const termTypeahead = useDebouncedTypeahead(termQuery, termFetcher)
  const reportTypeahead = useDebouncedTypeahead(reportQuery, reportFetcher)

  const [formError, setFormError] = useState<string | null>(null)
  const [isPending, startTransition] = useTransition()

  const pageTitle =
    mode === "create" ? "Create a Collection" : `Editing ${initial?.name?.trim() || "collection"}`

  const moveRow = (
    setter: Dispatch<SetStateAction<PickerRow[]>>,
    id: number,
    direction: "up" | "down",
  ) => {
    setter((rows) => {
      const index = rows.findIndex((row) => row.id === id)
      if (index < 0) return rows
      const target = direction === "up" ? index - 1 : index + 1
      if (target < 0 || target >= rows.length) return rows
      const next = [...rows]
      const temp = next[index]
      const swap = next[target]
      if (!temp || !swap) return rows
      next[index] = swap
      next[target] = temp
      return next
    })
  }

  const submit = () => {
    setFormError(null)
    const trimmedName = name.trim()
    if (!trimmedName) {
      setFormError("Name is required.")
      return
    }

    const body = {
      name: trimmedName,
      description: description.trim() ? description.trim() : null,
      purpose: purpose.trim() ? purpose.trim() : null,
      hidden: hidden ? "Y" : "N",
      termIds: termRows.map((row) => row.id),
      reportIds: reportRows.map((row) => row.id),
    }

    startTransition(() => {
      void (async () => {
        if (mode === "create") {
          const result = await createCollectionAction(body)
          if (result?.error) {
            setFormError(result.error)
          }
          return
        }

        if (typeof collectionId !== "number") {
          setFormError("Missing collection id.")
          return
        }

        const result = await updateCollectionAction(collectionId, body)
        if (result?.error) {
          setFormError(result.error)
        }
      })()
    })
  }

  return (
    <div className="space-y-8">
      <h1 className="font-serif text-4xl font-semibold tracking-tight">{pageTitle}</h1>

      <div className="flex flex-wrap items-stretch justify-between gap-4">
        <Button asChild variant="outline" size="lg" className="h-auto min-h-14 px-5 py-3">
          <Link href={cancelHref}>
            <ArrowLeft className="mr-3 size-5 shrink-0" />
            <span className="text-left">
              <span className="block font-semibold">Cancel</span>
              <span className="block text-xs font-normal text-muted-foreground">Go back</span>
            </span>
          </Link>
        </Button>
        <Button
          type="button"
          size="lg"
          className="h-auto min-h-14 px-5 py-3"
          disabled={isPending}
          onClick={submit}
        >
          <span className="text-left">
            <span className="block font-semibold">{isPending ? "Saving…" : "Save"}</span>
            <span className="block text-xs font-normal opacity-90">and continue</span>
          </span>
          <ArrowRight className="ml-3 size-5 shrink-0" />
        </Button>
      </div>

      <Card>
        <CardHeader className="border-b bg-muted/20">
          <CardTitle className="text-lg">Basics</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4 pt-6">
          <div className="space-y-2">
            <Label htmlFor="collection-name">Name</Label>
            <Input
              id="collection-name"
              value={name}
              placeholder="e.g. Data Sorting"
              onChange={(event) => {
                setName(event.target.value)
              }}
              required
            />
          </div>
          <div className="flex items-center gap-3 rounded-lg border bg-muted/10 px-4 py-3">
            <Switch
              id="collection-hidden"
              checked={hidden}
              onCheckedChange={(checked) => {
                setHidden(checked)
              }}
            />
            <Label htmlFor="collection-hidden" className="cursor-pointer font-normal">
              Hide collection from search?
            </Label>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader className="border-b bg-muted/20">
          <CardTitle className="text-lg">Content</CardTitle>
        </CardHeader>
        <CardContent className="space-y-6 pt-6">
          <div className="space-y-2">
            <Label htmlFor="collection-purpose">Purpose</Label>
            <p className="text-xs text-muted-foreground">
              Supports Markdown. Shown as search summary.
            </p>
            <textarea
              id="collection-purpose"
              value={purpose}
              onChange={(event) => {
                setPurpose(event.target.value)
              }}
              rows={4}
              className={textareaClassName}
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="collection-description">Description</Label>
            <p className="text-xs text-muted-foreground">
              Supports Markdown. Shown on the collection page.
            </p>
            <textarea
              id="collection-description"
              value={description}
              onChange={(event) => {
                setDescription(event.target.value)
              }}
              rows={5}
              className={textareaClassName}
            />
          </div>
        </CardContent>
      </Card>

      <div className="grid gap-6 lg:grid-cols-2">
        <Card>
          <CardHeader className="border-b bg-muted/20">
            <CardTitle className="text-lg">Linked Terms</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4 pt-6">
            <div className="space-y-2">
              <Label htmlFor="term-search">Search terms</Label>
              <Input
                id="term-search"
                value={termQuery}
                onChange={(event) => {
                  setTermQuery(event.target.value)
                }}
                placeholder="Search for terms…"
                autoComplete="off"
              />
              {termTypeahead.loading ? (
                <p className="text-xs text-muted-foreground">Searching…</p>
              ) : null}
              {termQuery.trim() && termTypeahead.results.length > 0 ? (
                <ul className="max-h-48 overflow-auto rounded-md border bg-background text-sm shadow-sm">
                  {termTypeahead.results.map((item) => (
                    <li key={item.id} className="border-b last:border-b-0">
                      <button
                        type="button"
                        className="flex w-full flex-col items-start gap-0.5 px-3 py-2 text-left hover:bg-muted"
                        onClick={() => {
                          setTermRows((rows) => {
                            if (rows.some((row) => row.id === item.id)) return rows
                            return [
                              ...rows,
                              {
                                id: item.id,
                                label: item.name,
                                subtitle: item.description ?? null,
                              },
                            ]
                          })
                          setTermQuery("")
                        }}
                      >
                        <span className="font-medium">{item.name}</span>
                        {item.description ? (
                          <span className="text-xs text-muted-foreground">{item.description}</span>
                        ) : null}
                      </button>
                    </li>
                  ))}
                </ul>
              ) : null}
            </div>
            <LinkedItemTags
              rows={termRows}
              onRemove={(id) => {
                setTermRows((rows) => rows.filter((row) => row.id !== id))
              }}
              onMove={(id, direction) => {
                moveRow(setTermRows, id, direction)
              }}
            />
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="border-b bg-muted/20">
            <CardTitle className="text-lg">Linked Reports</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4 pt-6">
            <div className="space-y-2">
              <Label htmlFor="report-search">Search reports</Label>
              <Input
                id="report-search"
                value={reportQuery}
                onChange={(event) => {
                  setReportQuery(event.target.value)
                }}
                placeholder="Search for reports…"
                autoComplete="off"
              />
              {reportTypeahead.loading ? (
                <p className="text-xs text-muted-foreground">Searching…</p>
              ) : null}
              {reportQuery.trim() && reportTypeahead.results.length > 0 ? (
                <ul className="max-h-48 overflow-auto rounded-md border bg-background text-sm shadow-sm">
                  {reportTypeahead.results.map((item) => (
                    <li key={item.id} className="border-b last:border-b-0">
                      <button
                        type="button"
                        className="flex w-full flex-col items-start gap-0.5 px-3 py-2 text-left hover:bg-muted"
                        onClick={() => {
                          setReportRows((rows) => {
                            if (rows.some((row) => row.id === item.id)) return rows
                            return [
                              ...rows,
                              {
                                id: item.id,
                                label: item.name,
                                subtitle: item.description ?? null,
                              },
                            ]
                          })
                          setReportQuery("")
                        }}
                      >
                        <span className="font-medium">{item.name}</span>
                        {item.description ? (
                          <span className="text-xs text-muted-foreground">{item.description}</span>
                        ) : null}
                      </button>
                    </li>
                  ))}
                </ul>
              ) : null}
            </div>
            <LinkedItemTags
              rows={reportRows}
              onRemove={(id) => {
                setReportRows((rows) => rows.filter((row) => row.id !== id))
              }}
              onMove={(id, direction) => {
                moveRow(setReportRows, id, direction)
              }}
            />
            <p className="text-xs text-muted-foreground">
              Order defines report rank on the collection page.
            </p>
          </CardContent>
        </Card>
      </div>

      {formError ? <p className="text-sm text-destructive">{formError}</p> : null}
    </div>
  )
}

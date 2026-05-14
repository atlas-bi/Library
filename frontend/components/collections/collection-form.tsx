"use client"

import { ChevronDown, ChevronUp, Trash2 } from "lucide-react"
import Link from "next/link"
import type { Dispatch, SetStateAction } from "react"
import { useCallback, useEffect, useMemo, useState, useTransition } from "react"
import {
  createCollectionAction,
  searchCollectionReportsAction,
  searchCollectionTermsAction,
  updateCollectionAction,
} from "@/app/collections/actions"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
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

function OrderedPicker({
  title,
  rows,
  onRemove,
  onMove,
}: {
  title: string
  rows: PickerRow[]
  onRemove: (id: number) => void
  onMove: (id: number, direction: "up" | "down") => void
}) {
  return (
    <div className="space-y-2">
      <div className="text-sm font-medium">{title}</div>
      {rows.length === 0 ? (
        <p className="text-sm text-muted-foreground">None selected.</p>
      ) : (
        <ul className="space-y-2">
          {rows.map((row, index) => (
            <li
              key={row.id}
              className="flex items-center justify-between gap-2 rounded-md border bg-background px-3 py-2 text-sm"
            >
              <div className="min-w-0 flex-1">
                <div className="truncate font-medium">{row.label}</div>
                {row.subtitle ? (
                  <div className="truncate text-xs text-muted-foreground">{row.subtitle}</div>
                ) : null}
              </div>
              <div className="flex shrink-0 items-center gap-1">
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
                  <Trash2 className="size-3.5" />
                </Button>
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}

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
  const [hidden, setHidden] = useState((initial?.hidden ?? "N").toUpperCase() === "Y" ? "Y" : "N")

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
      hidden,
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
    <Card>
      <CardHeader>
        <CardTitle>{mode === "create" ? "Create collection" : "Edit collection"}</CardTitle>
      </CardHeader>
      <CardContent className="space-y-6">
        <div className="grid gap-4 md:grid-cols-2">
          <div className="space-y-2 md:col-span-2">
            <Label htmlFor="collection-name">Name</Label>
            <Input
              id="collection-name"
              value={name}
              onChange={(event) => {
                setName(event.target.value)
              }}
              required
            />
          </div>
          <div className="space-y-2 md:col-span-2">
            <Label htmlFor="collection-description">Description</Label>
            <textarea
              id="collection-description"
              value={description}
              onChange={(event) => {
                setDescription(event.target.value)
              }}
              rows={3}
              className="w-full rounded-lg border border-input bg-transparent px-2.5 py-2 text-sm outline-none focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50"
            />
          </div>
          <div className="space-y-2 md:col-span-2">
            <Label htmlFor="collection-purpose">Purpose</Label>
            <textarea
              id="collection-purpose"
              value={purpose}
              onChange={(event) => {
                setPurpose(event.target.value)
              }}
              rows={3}
              className="w-full rounded-lg border border-input bg-transparent px-2.5 py-2 text-sm outline-none focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50"
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="collection-hidden">Visibility</Label>
            <select
              id="collection-hidden"
              value={hidden}
              onChange={(event) => {
                setHidden(event.target.value)
              }}
              className="h-8 w-full rounded-lg border border-input bg-transparent px-2 text-sm outline-none"
            >
              <option value="N">Visible</option>
              <option value="Y">Hidden</option>
            </select>
          </div>
        </div>

        <div className="grid gap-6 lg:grid-cols-2">
          <div className="space-y-3">
            <Label htmlFor="term-search">Add terms</Label>
            <Input
              id="term-search"
              value={termQuery}
              onChange={(event) => {
                setTermQuery(event.target.value)
              }}
              placeholder="Search terms…"
              autoComplete="off"
            />
            {termTypeahead.loading ? (
              <p className="text-xs text-muted-foreground">Searching…</p>
            ) : null}
            {termQuery.trim() && termTypeahead.results.length > 0 ? (
              <ul className="max-h-48 overflow-auto rounded-md border bg-background text-sm">
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
            <OrderedPicker
              title="Linked terms (order = rank)"
              rows={termRows}
              onRemove={(id) => {
                setTermRows((rows) => rows.filter((row) => row.id !== id))
              }}
              onMove={(id, direction) => {
                moveRow(setTermRows, id, direction)
              }}
            />
          </div>

          <div className="space-y-3">
            <Label htmlFor="report-search">Add reports</Label>
            <Input
              id="report-search"
              value={reportQuery}
              onChange={(event) => {
                setReportQuery(event.target.value)
              }}
              placeholder="Search reports…"
              autoComplete="off"
            />
            {reportTypeahead.loading ? (
              <p className="text-xs text-muted-foreground">Searching…</p>
            ) : null}
            {reportQuery.trim() && reportTypeahead.results.length > 0 ? (
              <ul className="max-h-48 overflow-auto rounded-md border bg-background text-sm">
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
            <OrderedPicker
              title="Linked reports (order = rank)"
              rows={reportRows}
              onRemove={(id) => {
                setReportRows((rows) => rows.filter((row) => row.id !== id))
              }}
              onMove={(id, direction) => {
                moveRow(setReportRows, id, direction)
              }}
            />
          </div>
        </div>

        {formError ? <p className="text-sm text-destructive">{formError}</p> : null}

        <div className="flex flex-wrap items-center gap-2">
          <Button type="button" disabled={isPending} onClick={submit}>
            {isPending ? "Saving…" : mode === "create" ? "Create" : "Save changes"}
          </Button>
          <Button asChild type="button" variant="outline" disabled={isPending}>
            <Link href={cancelHref}>Cancel</Link>
          </Button>
        </div>
      </CardContent>
    </Card>
  )
}

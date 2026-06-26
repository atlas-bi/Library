"use client"

import Link from "next/link"
import { useCallback, useEffect, useMemo, useState, useTransition } from "react"
import {
  searchReportCollectionsAction,
  searchReportTermsAction,
  updateReportAction,
} from "@/app/reports/actions"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import type { ReportDetail, ReportUpdateBody } from "@/lib/reports/types"

type PickerRow = { id: number; label: string }

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

export function ReportEditForm({
  reportId,
  initial,
  cancelHref,
}: {
  reportId: number
  initial: ReportDetail
  cancelHref: string
}) {
  const doc = (initial.document ?? {}) as Record<string, unknown>

  const [gitLabProjectUrl, setGitLabProjectUrl] = useState(String(doc.gitLabProjectUrl ?? ""))
  const [developerDescription, setDeveloperDescription] = useState(
    String(doc.developerDescription ?? initial.detailedDescription ?? initial.description ?? ""),
  )
  const [keyAssumptions, setKeyAssumptions] = useState(String(doc.keyAssumptions ?? ""))
  const [developerNotes, setDeveloperNotes] = useState(String(doc.developerNotes ?? ""))
  const [executiveVisibilityYn, setExecutiveVisibilityYn] = useState(
    String(doc.executiveVisibilityYn ?? "N").toUpperCase() === "Y" ? "Y" : "N",
  )
  const [hidden, setHidden] = useState(String(doc.hidden ?? "N").toUpperCase() === "Y" ? "Y" : "N")

  const initialTermRows = useMemo((): PickerRow[] => {
    return (initial.terms ?? []).map((t) => ({
      id: t.id,
      label: t.name?.trim() || `Term ${t.id}`,
    }))
  }, [initial.terms])

  const initialCollectionRows = useMemo((): PickerRow[] => {
    return (initial.collections ?? []).map((c) => ({
      id: c.id,
      label: c.name?.trim() || `Collection ${c.id}`,
    }))
  }, [initial.collections])

  const [termRows, setTermRows] = useState<PickerRow[]>(initialTermRows)
  const [collectionRows, setCollectionRows] = useState<PickerRow[]>(initialCollectionRows)
  const [termQuery, setTermQuery] = useState("")
  const [collectionQuery, setCollectionQuery] = useState("")

  const termFetcher = useCallback((q: string) => searchReportTermsAction(q), [])
  const collectionFetcher = useCallback((q: string) => searchReportCollectionsAction(q), [])
  const termTypeahead = useDebouncedTypeahead(termQuery, termFetcher)
  const collectionTypeahead = useDebouncedTypeahead(collectionQuery, collectionFetcher)

  const [formError, setFormError] = useState<string | null>(null)
  const [pending, startTransition] = useTransition()

  const imageIds = useMemo(() => (initial.images ?? []).map((img) => img.id), [initial.images])

  const submit = () => {
    setFormError(null)
    const body: ReportUpdateBody = {
      gitLabProjectUrl: gitLabProjectUrl.trim() || null,
      developerDescription: developerDescription.trim() || null,
      keyAssumptions: keyAssumptions.trim() || null,
      developerNotes: developerNotes.trim() || null,
      executiveVisibilityYn,
      enabledForHyperspace: "N",
      doNotPurge: "N",
      hidden,
      termIds: termRows.map((r) => r.id),
      collectionIds: collectionRows.map((r) => r.id),
      fragilityTagIds: [],
      imageIds,
      serviceRequestIds: [],
    }

    startTransition(() => {
      void (async () => {
        const result = await updateReportAction(reportId, body)
        if (result?.error) {
          setFormError(result.error)
        }
      })()
    })
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>Edit report documentation</CardTitle>
      </CardHeader>
      <CardContent className="space-y-6">
        <div className="grid gap-4">
          <div className="space-y-2">
            <Label htmlFor="gitlab-url">GitLab project URL</Label>
            <Input
              id="gitlab-url"
              value={gitLabProjectUrl}
              onChange={(e) => {
                setGitLabProjectUrl(e.target.value)
              }}
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="developer-description">Developer description</Label>
            <textarea
              id="developer-description"
              value={developerDescription}
              onChange={(e) => {
                setDeveloperDescription(e.target.value)
              }}
              rows={4}
              className="w-full rounded-lg border border-input bg-transparent px-2.5 py-2 text-sm outline-none focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50"
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="key-assumptions">Key assumptions</Label>
            <textarea
              id="key-assumptions"
              value={keyAssumptions}
              onChange={(e) => {
                setKeyAssumptions(e.target.value)
              }}
              rows={3}
              className="w-full rounded-lg border border-input bg-transparent px-2.5 py-2 text-sm outline-none"
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="developer-notes">Developer notes</Label>
            <textarea
              id="developer-notes"
              value={developerNotes}
              onChange={(e) => {
                setDeveloperNotes(e.target.value)
              }}
              rows={3}
              className="w-full rounded-lg border border-input bg-transparent px-2.5 py-2 text-sm outline-none"
            />
          </div>
          <div className="grid gap-4 sm:grid-cols-2">
            <div className="space-y-2">
              <Label htmlFor="executive-visibility">Executive visibility</Label>
              <select
                id="executive-visibility"
                value={executiveVisibilityYn}
                onChange={(e) => {
                  setExecutiveVisibilityYn(e.target.value)
                }}
                className="h-8 w-full rounded-lg border border-input bg-transparent px-2 text-sm"
              >
                <option value="N">No</option>
                <option value="Y">Yes</option>
              </select>
            </div>
            <div className="space-y-2">
              <Label htmlFor="report-hidden">Hidden</Label>
              <select
                id="report-hidden"
                value={hidden}
                onChange={(e) => {
                  setHidden(e.target.value)
                }}
                className="h-8 w-full rounded-lg border border-input bg-transparent px-2 text-sm"
              >
                <option value="N">Visible</option>
                <option value="Y">Hidden</option>
              </select>
            </div>
          </div>
        </div>

        <div className="grid gap-6 lg:grid-cols-2">
          <TypeaheadPicker
            label="Terms"
            query={termQuery}
            onQueryChange={setTermQuery}
            loading={termTypeahead.loading}
            results={termTypeahead.results}
            rows={termRows}
            onAdd={(item) => {
              setTermRows((rows) => {
                if (rows.some((r) => r.id === item.id)) return rows
                return [...rows, { id: item.id, label: item.name }]
              })
              setTermQuery("")
            }}
            onRemove={(id) => {
              setTermRows((rows) => rows.filter((r) => r.id !== id))
            }}
          />
          <TypeaheadPicker
            label="Collections"
            query={collectionQuery}
            onQueryChange={setCollectionQuery}
            loading={collectionTypeahead.loading}
            results={collectionTypeahead.results}
            rows={collectionRows}
            onAdd={(item) => {
              setCollectionRows((rows) => {
                if (rows.some((r) => r.id === item.id)) return rows
                return [...rows, { id: item.id, label: item.name }]
              })
              setCollectionQuery("")
            }}
            onRemove={(id) => {
              setCollectionRows((rows) => rows.filter((r) => r.id !== id))
            }}
          />
        </div>

        {formError ? <p className="text-sm text-destructive">{formError}</p> : null}

        <div className="flex flex-wrap gap-2">
          <Button type="button" disabled={pending} onClick={submit}>
            {pending ? "Saving…" : "Save changes"}
          </Button>
          <Button asChild type="button" variant="outline" disabled={pending}>
            <Link href={cancelHref}>Cancel</Link>
          </Button>
        </div>
      </CardContent>
    </Card>
  )
}

function TypeaheadPicker({
  label,
  query,
  onQueryChange,
  loading,
  results,
  rows,
  onAdd,
  onRemove,
}: {
  label: string
  query: string
  onQueryChange: (value: string) => void
  loading: boolean
  results: { id: number; name: string; description?: string | null }[]
  rows: PickerRow[]
  onAdd: (item: { id: number; name: string }) => void
  onRemove: (id: number) => void
}) {
  return (
    <div className="space-y-2">
      <Label>{label}</Label>
      <Input
        value={query}
        onChange={(e) => {
          onQueryChange(e.target.value)
        }}
        placeholder={`Search ${label.toLowerCase()}…`}
        autoComplete="off"
      />
      {loading ? <p className="text-xs text-muted-foreground">Searching…</p> : null}
      {query.trim() && results.length > 0 ? (
        <ul className="max-h-40 overflow-auto rounded-md border text-sm">
          {results.map((item) => (
            <li key={item.id} className="border-b last:border-b-0">
              <button
                type="button"
                className="w-full px-3 py-2 text-left hover:bg-muted"
                onClick={() => {
                  onAdd(item)
                }}
              >
                {item.name}
              </button>
            </li>
          ))}
        </ul>
      ) : null}
      {rows.length > 0 ? (
        <ul className="space-y-1 text-sm">
          {rows.map((row) => (
            <li
              key={row.id}
              className="flex items-center justify-between rounded-md border px-2 py-1"
            >
              <span>{row.label}</span>
              <button
                type="button"
                className="text-xs text-muted-foreground hover:text-foreground"
                onClick={() => {
                  onRemove(row.id)
                }}
              >
                Remove
              </button>
            </li>
          ))}
        </ul>
      ) : (
        <p className="text-sm text-muted-foreground">None selected.</p>
      )}
    </div>
  )
}

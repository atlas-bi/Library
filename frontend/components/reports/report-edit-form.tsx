"use client"

import Link from "next/link"
import { useCallback, useEffect, useMemo, useState, useTransition } from "react"
import {
  searchReportCollectionsAction,
  searchReportTermsAction,
  searchReportUsersAction,
  updateReportAction,
} from "@/app/reports/actions"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import type { ReportDetail, ReportEditLookupOptions, ReportUpdateBody } from "@/lib/reports/types"

type PickerRow = { id: number; label: string }

type ReportEditMode = "full" | "description" | "meta" | "maintenance"

const EMPTY_LOOKUPS: ReportEditLookupOptions = {
  organizationalValues: [],
  runFrequencies: [],
  fragilities: [],
  maintenanceSchedules: [],
  fragilityTags: [],
  maintenanceLogStatuses: [],
}

function lookupOptionLabel(item: { id: number; name?: string | null; label?: string | null }) {
  return item.label?.trim() || item.name?.trim() || `Option ${item.id}`
}

function userLabel(person?: { id: number; fullName?: string; username?: string } | null) {
  if (!person) return ""
  return person.fullName?.trim() || person.username?.trim() || `User ${person.id}`
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

export function ReportEditForm({
  reportId,
  initial,
  cancelHref,
  mode = "full",
  lookupOptions = EMPTY_LOOKUPS,
  onBack,
  onContinue,
}: {
  reportId: number
  initial: ReportDetail
  cancelHref: string
  mode?: ReportEditMode
  lookupOptions?: ReportEditLookupOptions
  onBack?: () => void
  onContinue?: () => void
}) {
  const doc = initial.document ?? {}

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
  const [enabledForHyperspace, setEnabledForHyperspace] = useState(
    String(doc.enabledForHyperspace ?? "N").toUpperCase() === "Y" ? "Y" : "N",
  )
  const [doNotPurge, setDoNotPurge] = useState(
    String(doc.doNotPurge ?? "N").toUpperCase() === "Y" ? "Y" : "N",
  )
  const [operationalOwnerUserId, setOperationalOwnerUserId] = useState<number | null>(
    doc.operationalOwner?.id ?? null,
  )
  const [operationalOwnerLabel, setOperationalOwnerLabel] = useState(
    userLabel(doc.operationalOwner),
  )
  const [requesterUserId, setRequesterUserId] = useState<number | null>(doc.requester?.id ?? null)
  const [requesterLabel, setRequesterLabel] = useState(userLabel(doc.requester))
  const [organizationalValueId, setOrganizationalValueId] = useState(
    doc.organizationalValue?.id ? String(doc.organizationalValue.id) : "",
  )
  const [estimatedRunFrequencyId, setEstimatedRunFrequencyId] = useState(
    doc.estimatedRunFrequency?.id ? String(doc.estimatedRunFrequency.id) : "",
  )
  const [fragilityId, setFragilityId] = useState(
    doc.fragility?.id ? String(doc.fragility.id) : "",
  )
  const [maintenanceScheduleId, setMaintenanceScheduleId] = useState(
    doc.maintenanceSchedule?.id ? String(doc.maintenanceSchedule.id) : "",
  )
  const [fragilityTagIds, setFragilityTagIds] = useState<number[]>(
    (doc.fragilityTags ?? []).map((tag) => tag.id),
  )
  const [maintenanceComment, setMaintenanceComment] = useState("")
  const [maintenanceLogStatusId, setMaintenanceLogStatusId] = useState(() => {
    const first = lookupOptions.maintenanceLogStatuses[0]?.id
    return first ?? 1
  })

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
  const [operationalOwnerQuery, setOperationalOwnerQuery] = useState("")
  const [requesterQuery, setRequesterQuery] = useState("")

  const termFetcher = useCallback((q: string) => searchReportTermsAction(q), [])
  const collectionFetcher = useCallback((q: string) => searchReportCollectionsAction(q), [])
  const userFetcher = useCallback((q: string) => searchReportUsersAction(q), [])
  const termTypeahead = useDebouncedTypeahead(termQuery, termFetcher)
  const collectionTypeahead = useDebouncedTypeahead(collectionQuery, collectionFetcher)
  const operationalOwnerTypeahead = useDebouncedTypeahead(operationalOwnerQuery, userFetcher)
  const requesterTypeahead = useDebouncedTypeahead(requesterQuery, userFetcher)

  const [formError, setFormError] = useState<string | null>(null)
  const [successMessage, setSuccessMessage] = useState<string | null>(null)
  const [pending, startTransition] = useTransition()

  const imageIds = useMemo(() => (initial.images ?? []).map((img) => img.id), [initial.images])

  const buildBody = (): ReportUpdateBody => ({
    gitLabProjectUrl: gitLabProjectUrl.trim() || null,
    developerDescription: developerDescription.trim() || null,
    keyAssumptions: keyAssumptions.trim() || null,
    developerNotes: developerNotes.trim() || null,
    operationalOwnerUserId,
    requesterUserId,
    organizationalValueId: organizationalValueId ? Number(organizationalValueId) : null,
    estimatedRunFrequencyId: estimatedRunFrequencyId ? Number(estimatedRunFrequencyId) : null,
    fragilityId: fragilityId ? Number(fragilityId) : null,
    maintenanceScheduleId: maintenanceScheduleId ? Number(maintenanceScheduleId) : null,
    executiveVisibilityYn,
    enabledForHyperspace,
    doNotPurge,
    hidden,
    termIds: termRows.map((r) => r.id),
    collectionIds: collectionRows.map((r) => r.id),
    fragilityTagIds,
    imageIds,
    serviceRequestIds: [],
    newMaintenanceLog: maintenanceComment.trim()
      ? {
          maintenanceLogStatusId,
          comment: maintenanceComment.trim(),
        }
      : null,
  })

  const submit = (continueAfterSave = false) => {
    setFormError(null)
    setSuccessMessage(null)
    const body = buildBody()

    startTransition(() => {
      void (async () => {
        const result = await updateReportAction(reportId, body)
        if (result?.error) {
          setFormError(result.error)
          return
        }
        setSuccessMessage("Changes saved.")
        if (continueAfterSave) {
          onContinue?.()
        }
      })()
    })
  }

  const showDescription = mode === "full" || mode === "description"
  const showMeta = mode === "full" || mode === "meta"
  const showMaintenance = mode === "full" || mode === "maintenance"
  const isWizardStep = mode !== "full"

  return (
    <Card>
      <CardHeader>
        <CardTitle>
          {mode === "description"
            ? "Description"
            : mode === "meta"
              ? "Meta"
              : mode === "maintenance"
                ? "Maintenance notes"
                : "Edit report documentation"}
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-6">
        {showDescription ? (
          <div className="grid gap-4">
            <div className="space-y-2">
              <Label htmlFor="developer-description">Developer description</Label>
              <textarea
                id="developer-description"
                value={developerDescription}
                onChange={(e) => {
                  setDeveloperDescription(e.target.value)
                }}
                rows={6}
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
                rows={4}
                className="w-full rounded-lg border border-input bg-transparent px-2.5 py-2 text-sm outline-none"
              />
            </div>
          </div>
        ) : null}

        {showMeta ? (
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

            <div className="grid gap-6 lg:grid-cols-2">
              <SingleUserPicker
                label="Operational owner"
                query={operationalOwnerQuery}
                onQueryChange={setOperationalOwnerQuery}
                loading={operationalOwnerTypeahead.loading}
                results={operationalOwnerTypeahead.results}
                selectedLabel={operationalOwnerLabel}
                onSelect={(item) => {
                  setOperationalOwnerUserId(item.id)
                  setOperationalOwnerLabel(item.name)
                  setOperationalOwnerQuery("")
                }}
                onClear={() => {
                  setOperationalOwnerUserId(null)
                  setOperationalOwnerLabel("")
                }}
              />
              <SingleUserPicker
                label="Requester"
                query={requesterQuery}
                onQueryChange={setRequesterQuery}
                loading={requesterTypeahead.loading}
                results={requesterTypeahead.results}
                selectedLabel={requesterLabel}
                onSelect={(item) => {
                  setRequesterUserId(item.id)
                  setRequesterLabel(item.name)
                  setRequesterQuery("")
                }}
                onClear={() => {
                  setRequesterUserId(null)
                  setRequesterLabel("")
                }}
              />
            </div>

            <div className="grid gap-4 sm:grid-cols-2">
              <LookupSelect
                id="organizational-value"
                label="Organizational value"
                value={organizationalValueId}
                options={lookupOptions.organizationalValues}
                onChange={setOrganizationalValueId}
              />
              <LookupSelect
                id="run-frequency"
                label="Estimated run frequency"
                value={estimatedRunFrequencyId}
                options={lookupOptions.runFrequencies}
                onChange={setEstimatedRunFrequencyId}
              />
              <LookupSelect
                id="fragility"
                label="Fragility rating"
                value={fragilityId}
                options={lookupOptions.fragilities}
                onChange={setFragilityId}
              />
              <LookupSelect
                id="maintenance-schedule"
                label="Maintenance schedule"
                value={maintenanceScheduleId}
                options={lookupOptions.maintenanceSchedules}
                onChange={setMaintenanceScheduleId}
              />
            </div>

            {lookupOptions.fragilityTags.length > 0 ? (
              <div className="space-y-2">
                <Label>Fragility tags</Label>
                <div className="grid gap-2 sm:grid-cols-2">
                  {lookupOptions.fragilityTags.map((tag) => {
                    const checked = fragilityTagIds.includes(tag.id)
                    return (
                      <label key={tag.id} className="flex items-center gap-2 text-sm">
                        <input
                          type="checkbox"
                          checked={checked}
                          onChange={() => {
                            setFragilityTagIds((current) =>
                              checked
                                ? current.filter((id) => id !== tag.id)
                                : [...current, tag.id],
                            )
                          }}
                        />
                        {lookupOptionLabel(tag)}
                      </label>
                    )
                  })}
                </div>
              </div>
            ) : null}

            <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
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
              <div className="space-y-2">
                <Label htmlFor="enabled-hyperspace">Enabled for Hyperspace</Label>
                <select
                  id="enabled-hyperspace"
                  value={enabledForHyperspace}
                  onChange={(e) => {
                    setEnabledForHyperspace(e.target.value)
                  }}
                  className="h-8 w-full rounded-lg border border-input bg-transparent px-2 text-sm"
                >
                  <option value="N">No</option>
                  <option value="Y">Yes</option>
                </select>
              </div>
              <div className="space-y-2">
                <Label htmlFor="do-not-purge">Do not purge</Label>
                <select
                  id="do-not-purge"
                  value={doNotPurge}
                  onChange={(e) => {
                    setDoNotPurge(e.target.value)
                  }}
                  className="h-8 w-full rounded-lg border border-input bg-transparent px-2 text-sm"
                >
                  <option value="N">No</option>
                  <option value="Y">Yes</option>
                </select>
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
          </div>
        ) : null}

        {showMaintenance ? (
          <div className="space-y-4">
            {lookupOptions.maintenanceLogStatuses.length > 0 ? (
              <div className="space-y-2">
                <Label htmlFor="maintenance-status">Maintenance log status</Label>
                <select
                  id="maintenance-status"
                  value={maintenanceLogStatusId}
                  onChange={(e) => {
                    setMaintenanceLogStatusId(Number(e.target.value))
                  }}
                  className="h-8 w-full rounded-lg border border-input bg-transparent px-2 text-sm"
                >
                  {lookupOptions.maintenanceLogStatuses.map((status) => (
                    <option key={status.id} value={status.id}>
                      {lookupOptionLabel(status)}
                    </option>
                  ))}
                </select>
              </div>
            ) : null}
            <div className="space-y-2">
              <Label htmlFor="maintenance-comment">Add maintenance log comment</Label>
              <textarea
                id="maintenance-comment"
                value={maintenanceComment}
                onChange={(e) => {
                  setMaintenanceComment(e.target.value)
                }}
                rows={4}
                className="w-full rounded-lg border border-input bg-transparent px-2.5 py-2 text-sm outline-none"
                placeholder="Optional maintenance note"
              />
            </div>
          </div>
        ) : null}

        {formError ? <p className="text-sm text-destructive">{formError}</p> : null}
        {successMessage ? <p className="text-sm text-emerald-700">{successMessage}</p> : null}

        <div className="flex flex-wrap gap-2">
          {isWizardStep ? (
            <>
              {onBack ? (
                <Button type="button" variant="outline" disabled={pending} onClick={onBack}>
                  Back
                </Button>
              ) : null}
              <Button
                type="button"
                disabled={pending}
                onClick={() => {
                  submit(Boolean(onContinue))
                }}
              >
                {pending ? "Saving…" : onContinue ? "Save and continue" : "Save changes"}
              </Button>
            </>
          ) : (
            <>
              <Button type="button" disabled={pending} onClick={() => submit(false)}>
                {pending ? "Saving…" : "Save changes"}
              </Button>
              <Button asChild type="button" variant="outline" disabled={pending}>
                <Link href={cancelHref}>Cancel</Link>
              </Button>
            </>
          )}
        </div>
      </CardContent>
    </Card>
  )
}

function LookupSelect({
  id,
  label,
  value,
  options,
  onChange,
}: {
  id: string
  label: string
  value: string
  options: ReportEditLookupOptions["organizationalValues"]
  onChange: (value: string) => void
}) {
  return (
    <div className="space-y-2">
      <Label htmlFor={id}>{label}</Label>
      <select
        id={id}
        value={value}
        onChange={(e) => {
          onChange(e.target.value)
        }}
        className="h-8 w-full rounded-lg border border-input bg-transparent px-2 text-sm"
      >
        <option value="">Not set</option>
        {options.map((option) => (
          <option key={option.id} value={option.id}>
            {lookupOptionLabel(option)}
          </option>
        ))}
      </select>
    </div>
  )
}

function SingleUserPicker({
  label,
  query,
  onQueryChange,
  loading,
  results,
  selectedLabel,
  onSelect,
  onClear,
}: {
  label: string
  query: string
  onQueryChange: (value: string) => void
  loading: boolean
  results: { id: number; name: string }[]
  selectedLabel: string
  onSelect: (item: { id: number; name: string }) => void
  onClear: () => void
}) {
  return (
    <div className="space-y-2">
      <Label>{label}</Label>
      {selectedLabel ? (
        <div className="flex items-center justify-between rounded-md border px-2 py-1 text-sm">
          <span>{selectedLabel}</span>
          <button type="button" className="text-xs text-muted-foreground hover:text-foreground" onClick={onClear}>
            Clear
          </button>
        </div>
      ) : (
        <>
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
                      onSelect(item)
                    }}
                  >
                    {item.name}
                  </button>
                </li>
              ))}
            </ul>
          ) : null}
        </>
      )}
    </div>
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

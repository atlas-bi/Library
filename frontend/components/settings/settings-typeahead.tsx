"use client"

import { useEffect, useId, useState } from "react"

export type SettingsTypeaheadItem = {
  id: number
  name: string
  description?: string | null
}

interface SettingsTypeaheadProps {
  label: string
  placeholder?: string
  value: string
  selectedId: number | null
  onQueryChange: (query: string) => void
  onSelect: (item: SettingsTypeaheadItem) => void
  onClear: () => void
  fetchResults: (query: string) => Promise<SettingsTypeaheadItem[]>
}

export function SettingsTypeahead({
  label,
  placeholder = "type to search...",
  value,
  selectedId,
  onQueryChange,
  onSelect,
  onClear,
  fetchResults,
}: SettingsTypeaheadProps) {
  const inputId = useId()
  const [results, setResults] = useState<SettingsTypeaheadItem[]>([])
  const [loading, setLoading] = useState(false)
  const [open, setOpen] = useState(false)

  useEffect(() => {
    const trimmed = value.trim()
    if (!trimmed || selectedId !== null) {
      setResults([])
      setLoading(false)
      return
    }

    setLoading(true)
    const handle = window.setTimeout(() => {
      void fetchResults(trimmed)
        .then((items) => {
          setResults(items)
          setOpen(items.length > 0)
        })
        .catch(() => {
          setResults([])
          setOpen(false)
        })
        .finally(() => {
          setLoading(false)
        })
    }, 280)

    return () => {
      window.clearTimeout(handle)
    }
  }, [value, selectedId, fetchResults])

  return (
    <div className="relative space-y-2">
      <label htmlFor={inputId} className="block text-sm font-semibold text-slate-700">
        {label}
      </label>
      <div className="flex gap-2">
        <input
          id={inputId}
          className="w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
          placeholder={placeholder}
          value={value}
          onChange={(e) => {
            onClear()
            onQueryChange(e.target.value)
            setOpen(true)
          }}
          onFocus={() => {
            if (results.length > 0) setOpen(true)
          }}
          autoComplete="off"
        />
        {value && (
          <button
            type="button"
            className="shrink-0 rounded-md border border-slate-300 px-3 py-2 text-sm text-slate-600 hover:bg-slate-50"
            onClick={() => {
              onQueryChange("")
              onClear()
              setResults([])
              setOpen(false)
            }}
            aria-label={`Clear ${label.toLowerCase()} search`}
          >
            Clear
          </button>
        )}
      </div>
      {loading && <p className="text-xs text-slate-500">Searching...</p>}
      {open && results.length > 0 && (
        <ul className="absolute z-10 mt-1 max-h-48 w-full overflow-auto rounded-md border border-slate-200 bg-white shadow-lg">
          {results.map((item) => (
            <li key={item.id}>
              <button
                type="button"
                className="block w-full px-3 py-2 text-left text-sm hover:bg-slate-50"
                onClick={() => {
                  onSelect(item)
                  onQueryChange(item.name)
                  setOpen(false)
                }}
              >
                <span className="font-medium text-slate-900">{item.name}</span>
                {item.description ? (
                  <span className="ml-2 text-slate-500">{item.description}</span>
                ) : null}
              </button>
            </li>
          ))}
        </ul>
      )}
      {open && !loading && value.trim() && results.length === 0 && selectedId === null && (
        <p className="text-xs text-slate-500">No matches found.</p>
      )}
    </div>
  )
}

"use client"

import { RefreshCw } from "lucide-react"
import { useState, useTransition } from "react"
import { updateEtlAction, updateThemeAction } from "@/app/settings/actions"

interface EtlThemePanelProps {
  initialEtl: string | null
  initialTheme: string | null
  defaultEtl: string | null
  themeOnly?: boolean
  etlOnly?: boolean
}

export function EtlThemePanel({
  initialEtl,
  initialTheme,
  defaultEtl,
  themeOnly,
  etlOnly,
}: EtlThemePanelProps) {
  const [etl, setEtl] = useState(initialEtl ?? "")
  const [theme, setTheme] = useState(initialTheme ?? "")
  const [etlError, setEtlError] = useState<string | null>(null)
  const [themeError, setThemeError] = useState<string | null>(null)
  const [etlSuccess, setEtlSuccess] = useState(false)
  const [themeSuccess, setThemeSuccess] = useState(false)
  const [isPending, startTransition] = useTransition()

  function handleSaveEtl() {
    setEtlError(null)
    setEtlSuccess(false)
    startTransition(async () => {
      const res = await updateEtlAction(etl || null)
      if (res.error) {
        setEtlError(res.error)
      } else {
        setEtlSuccess(true)
      }
    })
  }

  function handleSaveTheme() {
    setThemeError(null)
    setThemeSuccess(false)
    startTransition(async () => {
      const res = await updateThemeAction(theme || null)
      if (res.error) {
        setThemeError(res.error)
      } else {
        setThemeSuccess(true)
      }
    })
  }

  function handleRestoreDefault() {
    if (defaultEtl !== null) {
      setEtl(defaultEtl)
      setEtlSuccess(false)
      setEtlError(null)
    }
  }

  return (
    <div className="space-y-8">
      {/* ETL */}
      {!themeOnly && (
        <div className="space-y-4">
          <div>
            <h2 className="text-2xl font-bold font-serif text-slate-800">Report Tag ETL</h2>
            <p className="text-slate-500 mt-1 text-sm">
              SQL script executed to tag reports. Changes take effect on the next ETL run.
            </p>
          </div>

          {etlError && (
            <div className="bg-red-50 text-red-600 border border-red-200 p-3 rounded-md text-sm">
              {etlError}
            </div>
          )}
          {etlSuccess && (
            <div className="bg-green-50 text-green-700 border border-green-200 p-3 rounded-md text-sm">
              ETL script saved.
            </div>
          )}

          <div className="space-y-2">
            <label htmlFor="etl-value" className="block text-sm font-semibold text-slate-700">
              SQL Script
            </label>
            <textarea
              id="etl-value"
              className="w-full min-h-[300px] font-mono text-sm rounded-md border border-slate-300 bg-white p-3 focus:outline-none focus:ring-2 focus:ring-blue-500"
              value={etl}
              onChange={(e) => {
                setEtl(e.target.value)
                setEtlSuccess(false)
              }}
              disabled={isPending}
              placeholder="-- your ETL SQL here"
            />
          </div>

          <div className="flex gap-3">
            <button
              type="button"
              onClick={handleSaveEtl}
              disabled={isPending}
              className="px-4 py-2 bg-blue-500 text-white rounded-md text-sm font-medium hover:bg-blue-600 transition-colors disabled:opacity-50"
            >
              Save ETL
            </button>
            {defaultEtl !== null && (
              <button
                type="button"
                onClick={handleRestoreDefault}
                disabled={isPending}
                className="flex items-center gap-2 px-4 py-2 bg-white text-slate-700 border border-slate-300 rounded-md text-sm font-medium hover:bg-slate-50 transition-colors disabled:opacity-50"
              >
                <RefreshCw size={16} />
                Restore Default
              </button>
            )}
          </div>
        </div>
      )}

      {/* Theme */}
      {!etlOnly && (
        <div className="space-y-4">
          <div>
            <h2 className="text-2xl font-bold font-serif text-slate-800">Global CSS Theme</h2>
            <p className="text-slate-500 mt-1 text-sm">
              Custom CSS injected into every page. Use with care — malformed CSS can break the UI.
            </p>
          </div>

          {themeError && (
            <div className="bg-red-50 text-red-600 border border-red-200 p-3 rounded-md text-sm">
              {themeError}
            </div>
          )}
          {themeSuccess && (
            <div className="bg-green-50 text-green-700 border border-green-200 p-3 rounded-md text-sm">
              Theme saved.
            </div>
          )}

          <div className="space-y-2">
            <label htmlFor="theme-value" className="block text-sm font-semibold text-slate-700">
              CSS
            </label>
            <textarea
              id="theme-value"
              className="w-full min-h-[300px] font-mono text-sm rounded-md border border-slate-300 bg-white p-3 focus:outline-none focus:ring-2 focus:ring-blue-500"
              value={theme}
              onChange={(e) => {
                setTheme(e.target.value)
                setThemeSuccess(false)
              }}
              disabled={isPending}
              placeholder="/* custom CSS */"
            />
          </div>

          <button
            type="button"
            onClick={handleSaveTheme}
            disabled={isPending}
            className="px-4 py-2 bg-blue-500 text-white rounded-md text-sm font-medium hover:bg-blue-600 transition-colors disabled:opacity-50"
          >
            Save Theme
          </button>
        </div>
      )}
    </div>
  )
}

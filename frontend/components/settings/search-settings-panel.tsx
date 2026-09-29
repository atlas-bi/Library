"use client"

import { useState, useTransition } from "react"
import {
  updateSearchReportTypeTextAction,
  updateSearchVisibilityAction,
} from "@/app/settings/actions"
import type { SearchSettingsDto } from "@/lib/settings/types"

interface Props {
  initialData: SearchSettingsDto
}

export function SearchSettingsPanel({ initialData }: Props) {
  const [data, setData] = useState(initialData)
  const [error, setError] = useState<string | null>(null)
  const [isPending, startTransition] = useTransition()

  // Track unsaved override names locally
  const [overrideInputs, setOverrideInputs] = useState<Record<number, string>>(
    Object.fromEntries(initialData.reportTypes.map((t) => [t.id, t.shortName ?? ""])),
  )

  function handleVisibilityToggle(type: string, visible: boolean, reportTypeId?: number) {
    const prev = data

    // Optimistic update
    setData((current) => {
      if (reportTypeId !== undefined) {
        return {
          ...current,
          reportTypes: current.reportTypes.map((t) =>
            t.id === reportTypeId ? { ...t, visible } : t,
          ),
        }
      }
      return {
        ...current,
        visibility: {
          ...current.visibility,
          [type]: visible ? "Y" : "N",
        },
      }
    })

    startTransition(async () => {
      setError(null)
      const res = await updateSearchVisibilityAction(type, visible, reportTypeId)
      if (res && "error" in res) {
        setError(res.error as string)
        setData(prev)
      }
    })
  }

  function handleSaveOverride(reportTypeId: number) {
    const text = overrideInputs[reportTypeId]?.trim() || null
    startTransition(async () => {
      setError(null)
      const res = await updateSearchReportTypeTextAction(reportTypeId, text)
      if (res && "error" in res) {
        setError(res.error as string)
      } else {
        setData((current) => ({
          ...current,
          reportTypes: current.reportTypes.map((t) =>
            t.id === reportTypeId ? { ...t, shortName: text ?? "" } : t,
          ),
        }))
      }
    })
  }

  const objects = [
    { key: "users", label: "Users" },
    { key: "groups", label: "Groups" },
    { key: "terms", label: "Terms" },
    { key: "initiatives", label: "Initiatives" },
    { key: "collections", label: "Collections" },
  ]

  return (
    <div className="space-y-10">
      <h2 className="text-[28px] font-bold font-serif text-[#2c3e50]">Search Settings</h2>

      {error && (
        <div className="bg-red-50 text-red-600 border border-red-200 p-4 rounded-md">{error}</div>
      )}

      <div className="space-y-4">
        <h3 className="text-xl font-bold font-serif text-[#2c3e50]">Visible Objects</h3>
        <div className="w-full">
          <table className="w-full text-[13px] text-left">
            <thead>
              <tr className="border-b border-gray-300">
                <th className="py-2 font-bold text-gray-800 w-16"></th>
                <th className="py-2 font-bold text-gray-800">Object</th>
              </tr>
            </thead>
            <tbody>
              {objects.map((obj) => (
                <tr
                  key={obj.key}
                  className="border-b border-gray-200 last:border-b-0 hover:bg-gray-50 transition-colors"
                >
                  <td className="py-2.5 text-center align-middle">
                    <label className="relative inline-flex items-center cursor-pointer opacity-90 hover:opacity-100">
                      <input
                        type="checkbox"
                        className="sr-only peer"
                        checked={data.visibility[obj.key] === "Y"}
                        disabled={isPending}
                        onChange={(e) => handleVisibilityToggle(obj.key, e.target.checked)}
                      />
                      <div className="w-10 h-5 bg-gray-300 peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-4 after:w-4 after:transition-all peer-checked:bg-[#3b82f6]"></div>
                    </label>
                  </td>
                  <td className="py-2.5 text-gray-800 font-medium">{obj.label}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      <div className="space-y-4">
        <h3 className="text-xl font-bold font-serif text-[#2c3e50]">Visible Report Types</h3>
        <div className="w-full">
          <table className="w-full text-[13px] text-left">
            <thead>
              <tr className="border-b border-gray-300">
                <th className="py-2 font-bold text-gray-800 w-16"></th>
                <th className="py-2 font-bold text-gray-800">Report Type</th>
                <th className="py-2 font-bold text-gray-800 w-[400px]">Search Name Override</th>
              </tr>
            </thead>
            <tbody>
              {data.reportTypes.map((t) => (
                <tr
                  key={t.id}
                  className="border-b border-gray-200 last:border-b-0 hover:bg-gray-50 transition-colors"
                >
                  <td className="py-2.5 text-center align-middle">
                    <label className="relative inline-flex items-center cursor-pointer opacity-90 hover:opacity-100">
                      <input
                        type="checkbox"
                        className="sr-only peer"
                        checked={t.visible}
                        disabled={isPending}
                        onChange={(e) => handleVisibilityToggle("reports", e.target.checked, t.id)}
                      />
                      <div className="w-10 h-5 bg-gray-300 peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-4 after:w-4 after:transition-all peer-checked:bg-[#3b82f6]"></div>
                    </label>
                  </td>
                  <td className="py-2.5 text-gray-800 font-medium">{t.name}</td>
                  <td className="py-2.5">
                    <div className="flex w-full items-center space-x-2">
                      <input
                        type="text"
                        className="flex-1 rounded border border-gray-300 bg-white px-3 py-1 text-[13px] text-gray-700 placeholder-gray-400 focus:outline-none focus:border-blue-500"
                        placeholder={t.name}
                        value={overrideInputs[t.id] ?? ""}
                        onChange={(e) =>
                          setOverrideInputs((prev) => ({ ...prev, [t.id]: e.target.value }))
                        }
                        disabled={isPending}
                      />
                      <button
                        type="button"
                        className="px-4 py-1 bg-[#4a85e6] text-white rounded text-[13px] hover:bg-blue-600 transition-colors disabled:opacity-50 font-medium"
                        onClick={() => handleSaveOverride(t.id)}
                        disabled={isPending}
                      >
                        Save
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  )
}

"use client"

import { useState, useTransition } from "react"
import { createTagAction, deleteTagAction } from "@/app/settings/actions"
import type { TagType } from "@/lib/settings/api"
import type { ParameterDto } from "@/lib/settings/types"

interface TagSectionProps {
  title: string
  tagType: TagType
  initialItems: ParameterDto[]
  hasDescription?: boolean
}

function TagSection({ title, tagType, initialItems, hasDescription = false }: TagSectionProps) {
  const [items, setItems] = useState<ParameterDto[]>(initialItems)
  const [newName, setNewName] = useState("")
  const [newDesc, setNewDesc] = useState("")
  const [error, setError] = useState<string | null>(null)
  const [isPending, startTransition] = useTransition()

  function handleAdd(e: React.FormEvent) {
    e.preventDefault()
    if (!newName.trim()) return
    startTransition(async () => {
      setError(null)
      const res = await createTagAction(tagType, {
        name: newName.trim(),
        description: newDesc.trim() || undefined,
      })
      if (res && "error" in res) {
        setError(res.error as string)
      } else if (res && "data" in res) {
        setItems((prev) => [...prev, res.data as ParameterDto])
        setNewName("")
        setNewDesc("")
      }
    })
  }

  function handleDelete(id: number) {
    if (!confirm("Are you sure you want to remove this?")) return
    startTransition(async () => {
      setError(null)
      const res = await deleteTagAction(tagType, id)
      if (res && "error" in res) {
        setError(res.error as string)
      } else {
        setItems((prev) => prev.filter((i) => i.id !== id))
      }
    })
  }

  return (
    <div className="mb-8 last:mb-0 space-y-2">
      <h4 className="text-[15px] font-bold text-gray-800">{title}</h4>

      {error && (
        <div className="bg-red-50 text-red-600 border border-red-200 p-3 rounded-md text-sm">
          {error}
        </div>
      )}

      <div className="w-full">
        <table className="w-full text-[13px] text-left" aria-label={title}>
          <thead>
            <tr className="border-b border-gray-300">
              <th className="py-2 font-bold text-gray-800">Name</th>
              {hasDescription && <th className="py-2 font-bold text-gray-800">Description</th>}
              <th className="py-2 font-bold text-gray-800 w-24">Used</th>
              <th className="py-2 font-bold text-gray-800 w-24">Remove</th>
            </tr>
          </thead>
          <tbody>
            {items.length === 0 && (
              <tr>
                <td
                  colSpan={hasDescription ? 4 : 3}
                  className="py-4 text-center text-gray-500 border-b border-gray-200"
                >
                  No items.
                </td>
              </tr>
            )}
            {items.map((item) => (
              <tr
                key={item.id}
                className="border-b border-gray-200 last:border-b-0 hover:bg-gray-50 transition-colors"
              >
                <td className="py-2.5 font-bold text-gray-800">{item.name}</td>
                {hasDescription && (
                  <td className="py-2.5 text-gray-700">{item.description ?? ""}</td>
                )}
                <td className="py-2.5 text-gray-700">{item.used}</td>
                <td className="py-2.5">
                  <button
                    type="button"
                    className="text-blue-600 hover:text-blue-800 transition-colors inline-flex align-middle"
                    onClick={() => handleDelete(item.id)}
                    aria-label={`Delete ${item.name}`}
                  >
                    <i className="fas fa-trash"></i>
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <form onSubmit={handleAdd} className="flex max-w-[300px]">
        <input
          className="flex-1 rounded-l border border-gray-300 bg-white px-3 py-1.5 text-[13px] focus:outline-none focus:border-blue-500 placeholder-gray-400"
          type="text"
          placeholder={`Add ${title}...`}
          value={newName}
          onChange={(e) => setNewName(e.target.value)}
          required
        />
        {hasDescription && (
          <input
            className="flex-1 border-y border-gray-300 bg-white px-3 py-1.5 text-[13px] focus:outline-none focus:border-blue-500 placeholder-gray-400"
            type="text"
            placeholder="Description (optional)"
            value={newDesc}
            onChange={(e) => setNewDesc(e.target.value)}
          />
        )}
        <button
          className="px-4 py-1.5 bg-[#4a85e6] text-white rounded-r border border-[#4a85e6] text-[13px] font-medium hover:bg-blue-600 transition-colors disabled:opacity-50"
          type="submit"
          disabled={isPending}
        >
          Add
        </button>
      </form>
    </div>
  )
}

interface Props {
  organizationalValues: ParameterDto[]
  estimatedRunFrequencies: ParameterDto[]
  fragilities: ParameterDto[]
  fragilityTags: ParameterDto[]
  maintenanceSchedules: ParameterDto[]
  maintenanceLogStatuses: ParameterDto[]
  financialImpacts: ParameterDto[]
  strategicImportances: ParameterDto[]
  tags: ParameterDto[]
}

export function TagsSettingsPanel({
  organizationalValues,
  estimatedRunFrequencies,
  fragilities,
  fragilityTags,
  maintenanceSchedules,
  maintenanceLogStatuses,
  financialImpacts,
  strategicImportances,
  tags,
}: Props) {
  return (
    <div className="space-y-6">
      <h3 className="text-[28px] font-bold font-serif text-[#2c3e50] mb-8">Meta Fields</h3>
      <TagSection
        title="Organizational Value"
        tagType="organizational-values"
        initialItems={organizationalValues}
      />
      <TagSection
        title="Estimated Run Frequency"
        tagType="estimated-run-frequencies"
        initialItems={estimatedRunFrequencies}
      />
      <TagSection title="Fragility Types" tagType="fragilities" initialItems={fragilities} />
      <TagSection title="Fragility Tags" tagType="fragility-tags" initialItems={fragilityTags} />
      <TagSection
        title="Maintenance Schedules"
        tagType="maintenance-schedules"
        initialItems={maintenanceSchedules}
      />
      <TagSection
        title="Maintenance Log Status"
        tagType="maintenance-log-statuses"
        initialItems={maintenanceLogStatuses}
      />
      <TagSection
        title="Financial Impact"
        tagType="financial-impacts"
        initialItems={financialImpacts}
      />
      <TagSection
        title="Strategic Importance"
        tagType="strategic-importances"
        initialItems={strategicImportances}
      />
      <TagSection title="Tags" tagType="tags" initialItems={tags} hasDescription />
    </div>
  )
}

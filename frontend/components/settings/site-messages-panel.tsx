"use client"

import { Trash2 } from "lucide-react"
import { useState, useTransition } from "react"
import { addSiteMessageAction, deleteSiteMessageAction } from "@/app/settings/actions"
import type { SiteMessageDto } from "@/lib/settings/types"

interface Props {
  initialMessages: SiteMessageDto[]
}

export function SiteMessagesPanel({ initialMessages }: Props) {
  const [messages, setMessages] = useState<SiteMessageDto[]>(initialMessages)
  const [newValue, setNewValue] = useState("")
  const [newDesc, setNewDesc] = useState("")
  const [error, setError] = useState<string | null>(null)
  const [isPending, startTransition] = useTransition()

  function handleAdd(e: React.FormEvent) {
    e.preventDefault()
    if (!newValue.trim()) {
      setError("Message content is required.")
      return
    }
    setError(null)
    startTransition(async () => {
      const res = await addSiteMessageAction({
        value: newValue.trim(),
        description: newDesc.trim() || undefined,
      })
      if (res && "error" in res) {
        setError(res.error as string)
      } else if (res && "data" in res) {
        setMessages((prev) => [...prev, res.data as SiteMessageDto])
        setNewValue("")
        setNewDesc("")
      }
    })
  }

  function handleDelete(id: number) {
    if (!confirm("Are you sure you want to remove this?")) return
    startTransition(async () => {
      setError(null)
      const res = await deleteSiteMessageAction(id)
      if (res && "error" in res) {
        setError(res.error as string)
      } else {
        setMessages((prev) => prev.filter((m) => m.id !== id))
      }
    })
  }

  return (
    <div className="space-y-6">
      <h2 className="text-2xl font-bold font-serif text-slate-800">Site Messages</h2>

      {error && (
        <div className="bg-red-50 text-red-600 border border-red-200 p-4 rounded-md">{error}</div>
      )}

      <form onSubmit={handleAdd} className="flex gap-2 w-full max-w-2xl">
        <div className="flex-1">
          <input
            className="w-full h-10 rounded-md border border-slate-300 bg-white px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            type="text"
            placeholder="Message Content..."
            value={newValue}
            onChange={(e) => setNewValue(e.target.value)}
            required
          />
        </div>
        <div className="flex-1">
          <input
            className="w-full h-10 rounded-md border border-slate-300 bg-white px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            type="text"
            placeholder="Description (optional)"
            value={newDesc}
            onChange={(e) => setNewDesc(e.target.value)}
          />
        </div>
        <button
          className="h-10 px-4 py-2 bg-blue-500 text-white rounded-md text-sm font-medium hover:bg-blue-600 transition-colors disabled:opacity-50"
          type="submit"
          disabled={isPending}
        >
          Add
        </button>
      </form>

      <div className="overflow-x-auto border border-slate-200 rounded-md">
        <table className="w-full text-sm text-left">
          <thead className="bg-slate-50 border-b border-slate-200 text-slate-700">
            <tr>
              <th className="px-4 py-3 font-semibold">Message</th>
              <th className="px-4 py-3 font-semibold">Description</th>
              <th className="px-4 py-3 font-semibold w-24">Remove</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-200 bg-white">
            {messages.length === 0 && (
              <tr>
                <td colSpan={3} className="px-4 py-8 text-center text-slate-500">
                  No site messages.
                </td>
              </tr>
            )}
            {messages.map((m) => (
              <tr key={m.id} className="hover:bg-slate-50">
                <td className="px-4 py-3 text-slate-900">{m.value}</td>
                <td className="px-4 py-3 text-slate-700">{m.description ?? ""}</td>
                <td className="px-4 py-3 text-center">
                  <button
                    type="button"
                    className="text-red-500 hover:text-red-700 transition-colors"
                    onClick={() => handleDelete(m.id)}
                    aria-label={`Delete message ${m.id}`}
                  >
                    <Trash2 size={16} />
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}

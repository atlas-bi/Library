"use client"

import { Trash2 } from "lucide-react"
import { useCallback, useState, useTransition } from "react"
import {
  addUserRoleAction,
  removeUserRoleAction,
  searchSettingsUsersAction,
} from "@/app/settings/actions"
import { SettingsTypeahead } from "@/components/settings/settings-typeahead"
import type { RoleDto, UserRoleAssignmentDto } from "@/lib/settings/types"

interface Props {
  initialAssignments: UserRoleAssignmentDto[]
  availableRoles: RoleDto[]
}

export function UserRolesPanel({ initialAssignments, availableRoles }: Props) {
  const [assignments, setAssignments] = useState<UserRoleAssignmentDto[]>(initialAssignments)
  const [userSearch, setUserSearch] = useState("")
  const [selectedUserId, setSelectedUserId] = useState<number | null>(null)
  const [selectedRoleId, setSelectedRoleId] = useState<number | "">("")
  const [error, setError] = useState<string | null>(null)
  const [isPending, startTransition] = useTransition()

  const fetchUsers = useCallback((query: string) => searchSettingsUsersAction(query), [])

  async function handleAdd(e: React.FormEvent) {
    e.preventDefault()
    if (!selectedUserId || !selectedRoleId) {
      setError("User and role are required.")
      return
    }
    setError(null)
    startTransition(async () => {
      const res = await addUserRoleAction(selectedUserId, Number(selectedRoleId))
      if (res && "error" in res) {
        setError(res.error as string)
      } else {
        const role = availableRoles.find((r) => r.id === Number(selectedRoleId))
        if (!role) return
        setAssignments((prev) => {
          const existing = prev.find((a) => a.userId === selectedUserId)
          if (existing) {
            return prev.map((a) =>
              a.userId === selectedUserId
                ? { ...a, roles: [...a.roles, { id: role.id, name: role.name }] }
                : a,
            )
          }
          return [
            ...prev,
            { userId: selectedUserId, name: userSearch, roles: [{ id: role.id, name: role.name }] },
          ]
        })
        setUserSearch("")
        setSelectedUserId(null)
        setSelectedRoleId("")
      }
    })
  }

  function handleRemove(userId: number, roleId: number) {
    startTransition(async () => {
      setError(null)
      const res = await removeUserRoleAction(userId, roleId)
      if (res && "error" in res) {
        setError(res.error as string)
      } else {
        setAssignments((prev) =>
          prev
            .map((a) =>
              a.userId === userId ? { ...a, roles: a.roles.filter((r) => r.id !== roleId) } : a,
            )
            .filter((a) => a.roles.length > 0),
        )
      }
    })
  }

  return (
    <div className="space-y-6">
      <h2 className="text-2xl font-bold font-serif text-slate-800">User Roles</h2>

      <div className="space-y-4">
        <h3 className="text-xl font-semibold text-slate-800">Add Privileged User</h3>

        {error && (
          <div className="bg-red-50 text-red-600 border border-red-200 p-4 rounded-md">{error}</div>
        )}

        <form onSubmit={handleAdd} className="space-y-4 max-w-xl">
          <SettingsTypeahead
            label="User"
            value={userSearch}
            selectedId={selectedUserId}
            onQueryChange={setUserSearch}
            onSelect={(item) => setSelectedUserId(item.id)}
            onClear={() => setSelectedUserId(null)}
            fetchResults={fetchUsers}
          />

          <div className="space-y-2">
            <label
              htmlFor="user-role-select"
              className="block text-sm font-semibold text-slate-700"
            >
              Role
            </label>
            <select
              id="user-role-select"
              className="w-full rounded-md border border-slate-300 bg-white px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              value={selectedRoleId}
              onChange={(e) => setSelectedRoleId(e.target.value ? Number(e.target.value) : "")}
            >
              <option value="">Select a role...</option>
              {availableRoles
                .filter((r) => r.name !== "Administrator" && r.name !== "User")
                .map((r) => (
                  <option key={r.id} value={r.id}>
                    {r.name}
                  </option>
                ))}
            </select>
          </div>

          <button
            className="px-4 py-2 bg-blue-500 text-white rounded-md text-sm font-medium hover:bg-blue-600 transition-colors disabled:opacity-50"
            type="submit"
            disabled={isPending}
          >
            Save
          </button>
        </form>
      </div>

      <div className="space-y-4">
        <h3 className="text-xl font-semibold text-slate-800">Privileged Users</h3>
        <div className="overflow-x-auto border border-slate-200 rounded-md">
          <table className="w-full text-sm text-left">
            <thead className="bg-slate-50 border-b border-slate-200 text-slate-700">
              <tr>
                <th className="px-4 py-3 font-semibold">User</th>
                <th className="px-4 py-3 font-semibold">Role</th>
                <th className="px-4 py-3 font-semibold">Remove Role</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-200 bg-white">
              {assignments.length === 0 && (
                <tr>
                  <td colSpan={3} className="px-4 py-8 text-center text-slate-500">
                    No privileged users assigned.
                  </td>
                </tr>
              )}
              {assignments.map((a) =>
                a.roles.map((r) => (
                  <tr key={`${a.userId}-${r.id}`} className="hover:bg-slate-50">
                    <td className="px-4 py-3 text-slate-900">{a.name}</td>
                    <td className="px-4 py-3 text-slate-900">{r.name}</td>
                    <td className="px-4 py-3">
                      <button
                        type="button"
                        className="text-red-500 hover:text-red-700 transition-colors"
                        onClick={() => handleRemove(a.userId, r.id)}
                        aria-label={`Remove role ${r.name} from ${a.name}`}
                      >
                        <Trash2 size={16} />
                      </button>
                    </td>
                  </tr>
                )),
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  )
}

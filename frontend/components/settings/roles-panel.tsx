"use client"

import { Trash2 } from "lucide-react"
import { useOptimistic, useState, useTransition } from "react"
import {
  createRoleAction,
  deleteRoleAction,
  updateRolePermissionAction,
} from "@/app/settings/actions"
import type { PermissionDto, RoleDto } from "@/lib/settings/types"

interface Props {
  initialRoles: RoleDto[]
  permissions: PermissionDto[]
}

const PROTECTED_IDS = new Set([1, 5])
const PROTECTED_NAMES = new Set(["Administrator", "Director"])

function isProtected(role: RoleDto) {
  return PROTECTED_IDS.has(role.id) || PROTECTED_NAMES.has(role.name)
}

export function RolesPanel({ initialRoles, permissions }: Props) {
  const [roles, setRoles] = useOptimistic<RoleDto[]>(initialRoles)
  const [newRoleName, setNewRoleName] = useState("")
  const [error, setError] = useState<string | null>(null)
  const [isPending, startTransition] = useTransition()

  function hasPermission(role: RoleDto, permId: number) {
    return role.name === "Administrator" || role.permissions.some((p) => p.id === permId)
  }

  function handleToggle(roleId: number, permId: number, currentChecked: boolean) {
    startTransition(async () => {
      setError(null)
      const next = !currentChecked
      setRoles((prev) =>
        prev.map((r) => {
          if (r.id !== roleId) return r
          const perms = next
            ? [...r.permissions, { id: permId, name: "" }]
            : r.permissions.filter((p) => p.id !== permId)
          return { ...r, permissions: perms }
        }),
      )
      const res = await updateRolePermissionAction(roleId, permId, next)
      if (res && "error" in res) {
        setError(res.error as string)
        setRoles((prev) =>
          prev.map((r) => {
            if (r.id !== roleId) return r
            const perms = !next
              ? [...r.permissions, { id: permId, name: "" }]
              : r.permissions.filter((p) => p.id !== permId)
            return { ...r, permissions: perms }
          }),
        )
      }
    })
  }

  function handleDeleteRole(roleId: number) {
    if (!confirm("Are you sure you want to remove this?")) return
    startTransition(async () => {
      setError(null)
      setRoles((prev) => prev.filter((r) => r.id !== roleId))
      const res = await deleteRoleAction(roleId)
      if (res && "error" in res) {
        setError(res.error as string)
        setRoles(initialRoles)
      }
    })
  }

  function handleCreateRole(e: React.FormEvent) {
    e.preventDefault()
    if (!newRoleName.trim()) return
    startTransition(async () => {
      setError(null)
      const res = await createRoleAction({ name: newRoleName.trim() })
      if (res && "error" in res) {
        setError(res.error as string)
      } else if (res && "data" in res) {
        setRoles((prev) => [...prev, res.data as RoleDto])
        setNewRoleName("")
      }
    })
  }

  return (
    <div className="space-y-6">
      <h2 className="text-[28px] font-bold font-serif text-[#2c3e50]">Role Configuration</h2>

      {error && (
        <div className="bg-red-50 text-red-600 border border-red-200 p-4 rounded-md">
          {error}
        </div>
      )}

      <form onSubmit={handleCreateRole} className="space-y-1">
        <label className="block text-[13px] font-bold text-gray-800">Add a New Role</label>
        <div className="flex w-full max-w-[300px] items-center space-x-0">
          <input
            className="flex-1 rounded-l border border-gray-300 bg-white px-3 py-1.5 text-[13px] focus:outline-none focus:border-blue-500"
            type="text"
            placeholder="executive"
            value={newRoleName}
            onChange={(e) => setNewRoleName(e.target.value)}
            required
          />
          <button 
            className="px-4 py-1.5 bg-[#4a85e6] text-white rounded-r border border-[#4a85e6] text-[13px] font-medium hover:bg-blue-600 transition-colors disabled:opacity-50"
            type="submit" 
            disabled={isPending}
          >
            Save
          </button>
        </div>
      </form>

      <div className="w-full mt-4">
        <table className="w-full text-[13px] text-left" aria-label="role permissions matrix">
          <thead>
            <tr className="border-b border-gray-300">
              <th scope="col" className="py-2 font-bold text-gray-800 w-64">Permission</th>
              {roles.map((role) => (
                <th scope="col" key={role.id} className="py-2 font-bold text-gray-800 text-center whitespace-nowrap">
                  {role.name}
                  {!isProtected(role) && (
                    <button
                      type="button"
                      onClick={() => handleDeleteRole(role.id)}
                      aria-label={`Delete role ${role.name}`}
                      className="ml-2 text-blue-600 hover:text-blue-800 inline-flex align-middle"
                    >
                      <i className="fas fa-trash"></i>
                    </button>
                  )}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {permissions.map((perm) => (
              <tr key={perm.id} className="border-b border-gray-200 last:border-b-0 hover:bg-gray-50 transition-colors">
                <th scope="row" className="py-2.5 font-bold text-gray-800">{perm.name}</th>
                {roles.map((role) => {
                  const checked = hasPermission(role, perm.id)
                  const isAdmin = role.name === "Administrator"
                  return (
                    <td key={role.id} className="py-2.5 text-center align-middle">
                      <input
                        className="w-[14px] h-[14px] text-blue-600 bg-gray-100 border-gray-300 rounded focus:ring-blue-500 disabled:opacity-50 cursor-pointer"
                        id={`${role.id}-${perm.id}`}
                        type="checkbox"
                        checked={checked}
                        disabled={isAdmin || isPending}
                        aria-label={`${perm.name} for ${role.name}`}
                        onChange={() => !isAdmin && handleToggle(role.id, perm.id, checked)}
                      />
                    </td>
                  )
                })}
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}

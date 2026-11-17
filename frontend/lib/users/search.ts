import { getServerApiBase } from "@/lib/api-base"
import { getToken } from "@/lib/auth"
import { apiFetchJson } from "@/lib/http"
import type { UserSearchResult } from "@/lib/users/types"

export type UserTypeaheadItem = {
  id: number
  name: string
  description?: string | null
}

/** GET /api/users/search — SQL user lookup (up to 20 rows). */
export async function searchUsers(q: string): Promise<UserSearchResult[]> {
  const trimmed = q.trim()
  if (!trimmed) return []

  const token = await getToken()
  if (!token) return []

  const apiBase = getServerApiBase()
  if (!apiBase) return []

  const params = new URLSearchParams({ q: trimmed })
  const result = await apiFetchJson<UserSearchResult[]>(
    `${apiBase}/api/users/search?${params}`,
    {
      headers: { Authorization: `Bearer ${token}` },
      cache: "no-store",
    },
  )

  if (!result.ok) return []
  return Array.isArray(result.data) ? result.data : []
}

export function mapUserSearchResultsToTypeahead(
  users: UserSearchResult[],
): UserTypeaheadItem[] {
  return users.map((user) => ({
    id: user.id,
    name: user.name,
    description: user.email ?? null,
  }))
}

export async function searchUsersForTypeahead(q: string): Promise<UserTypeaheadItem[]> {
  return mapUserSearchResultsToTypeahead(await searchUsers(q))
}

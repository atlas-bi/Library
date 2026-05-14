import { getServerApiBase } from "@/lib/api-base"
import { getToken } from "@/lib/auth"
import type { AppErrorCode } from "@/lib/errors"
import { apiFetchJson } from "@/lib/http"
import type { SearchResponseDto } from "./types"

type SearchQueryValue = string | string[] | undefined

export type SearchParamsInput = Record<string, SearchQueryValue>

export type SearchResponseResult = {
  data: SearchResponseDto | null
  error: AppErrorCode | null
}

function appendQueryValue(params: URLSearchParams, key: string, value: SearchQueryValue) {
  if (typeof value === "string") {
    if (value.trim() !== "") params.append(key, value)
    return
  }

  if (Array.isArray(value)) {
    value
      .filter((v) => v.trim() !== "")
      .forEach((v) => {
        params.append(key, v)
      })
  }
}

export async function searchLibrary(paramsInput: SearchParamsInput): Promise<SearchResponseResult> {
  const token = await getToken()
  if (!token) return { data: null, error: "auth_required" }

  const apiBase = getServerApiBase()
  if (!apiBase) return { data: null, error: "service_unavailable" }

  const query = new URLSearchParams()
  Object.entries(paramsInput).forEach(([key, value]) => {
    appendQueryValue(query, key, value)
  })

  const url = `${apiBase}/api/search${query.toString() ? `?${query.toString()}` : ""}`
  const result = await apiFetchJson<SearchResponseDto>(url, {
    headers: { Authorization: `Bearer ${token}` },
    cache: "no-store",
  })

  if (!result.ok) return { data: null, error: result.error.code }
  return { data: result.data, error: null }
}

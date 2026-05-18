import { getServerApiBase } from "@/lib/api-base"
import { getToken } from "@/lib/auth"
import type { AppErrorCode } from "@/lib/errors"
import { getUserFriendlyErrorMessage, mapHttpStatusToErrorCode } from "@/lib/errors"
import { apiFetchJson } from "@/lib/http"
import type {
  FeedbackRequest,
  InteractionRecipientDto,
  ShareMailRequest,
  ShareMailResponse,
  StarToggleRequest,
  StarToggleResponse,
} from "./types"

export type InteractionMutationOk<T> = { ok: true; data: T }
export type InteractionMutationErr = { ok: false; message: string; code: AppErrorCode }
export type InteractionMutationResult<T> = InteractionMutationOk<T> | InteractionMutationErr

async function parseErrorMessage(res: Response): Promise<string> {
  const fallback = getUserFriendlyErrorMessage(mapHttpStatusToErrorCode(res.status))
  try {
    const text = await res.text()
    if (!text.trim()) return fallback
    try {
      const parsed = JSON.parse(text) as { error?: string; message?: string }
      return parsed.error ?? parsed.message ?? fallback
    } catch {
      return text.length > 300 ? `${text.slice(0, 300)}…` : text
    }
  } catch {
    return fallback
  }
}

function toErrorCode(status: number): AppErrorCode {
  return mapHttpStatusToErrorCode(status)
}

export async function toggleStar(
  body: StarToggleRequest,
): Promise<InteractionMutationResult<StarToggleResponse>> {
  const token = await getToken()
  if (!token) {
    return {
      ok: false,
      message: getUserFriendlyErrorMessage("auth_required"),
      code: "auth_required",
    }
  }

  const apiBase = getServerApiBase()
  if (!apiBase) {
    return {
      ok: false,
      message: getUserFriendlyErrorMessage("service_unavailable"),
      code: "service_unavailable",
    }
  }

  const res = await fetch(`${apiBase}/api/interactions/stars/toggle`, {
    method: "POST",
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
    },
    body: JSON.stringify(body),
  })

  if (res.ok) {
    return { ok: true, data: (await res.json()) as StarToggleResponse }
  }

  return { ok: false, message: await parseErrorMessage(res), code: toErrorCode(res.status) }
}

export async function sendShareMail(
  body: ShareMailRequest,
): Promise<InteractionMutationResult<ShareMailResponse>> {
  const token = await getToken()
  if (!token) {
    return {
      ok: false,
      message: getUserFriendlyErrorMessage("auth_required"),
      code: "auth_required",
    }
  }

  const apiBase = getServerApiBase()
  if (!apiBase) {
    return {
      ok: false,
      message: getUserFriendlyErrorMessage("service_unavailable"),
      code: "service_unavailable",
    }
  }

  const res = await fetch(`${apiBase}/api/interactions/share-mail`, {
    method: "POST",
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
    },
    body: JSON.stringify(body),
  })

  if (res.ok) {
    return { ok: true, data: (await res.json()) as ShareMailResponse }
  }

  return { ok: false, message: await parseErrorMessage(res), code: toErrorCode(res.status) }
}

export async function submitFeedback(
  body: FeedbackRequest,
): Promise<InteractionMutationResult<unknown>> {
  const token = await getToken()
  if (!token) {
    return {
      ok: false,
      message: getUserFriendlyErrorMessage("auth_required"),
      code: "auth_required",
    }
  }

  const apiBase = getServerApiBase()
  if (!apiBase) {
    return {
      ok: false,
      message: getUserFriendlyErrorMessage("service_unavailable"),
      code: "service_unavailable",
    }
  }

  const res = await fetch(`${apiBase}/api/interactions/feedback`, {
    method: "POST",
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
    },
    body: JSON.stringify(body),
  })

  if (res.ok) {
    return { ok: true, data: (await res.json()) as unknown }
  }

  return { ok: false, message: await parseErrorMessage(res), code: toErrorCode(res.status) }
}

export async function searchInteractionRecipients(
  q: string,
  includeGroups = true,
): Promise<InteractionRecipientDto[]> {
  const trimmed = q.trim()
  if (!trimmed) return []

  const token = await getToken()
  if (!token) return []

  const apiBase = getServerApiBase()
  if (!apiBase) return []

  const params = new URLSearchParams({ q: trimmed, includeGroups: String(includeGroups) })
  const result = await apiFetchJson<InteractionRecipientDto[]>(
    `${apiBase}/api/interactions/search/recipients?${params}`,
    {
      headers: { Authorization: `Bearer ${token}` },
      cache: "no-store",
    },
  )

  if (!result.ok) return []
  return Array.isArray(result.data) ? result.data : []
}

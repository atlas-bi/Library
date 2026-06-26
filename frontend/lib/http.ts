import { AppError, type AppErrorCode, mapHttpStatusToErrorCode } from "@/lib/app-error"

export type Ok<T> = { ok: true; data: T }
export type Err = { ok: false; error: AppError }
export type Result<T> = Ok<T> | Err

type ApiFetchOptions = Omit<RequestInit, "headers"> & {
  headers?: HeadersInit
}

function toAppError(err: unknown): AppError {
  if (err instanceof AppError) return err
  return new AppError({ code: "unknown", cause: err })
}

export async function apiFetchJson<T>(url: string, init?: ApiFetchOptions): Promise<Result<T>> {
  try {
    const res = await fetch(url, init)
    if (!res.ok) {
      const code: AppErrorCode = mapHttpStatusToErrorCode(res.status)
      return { ok: false, error: new AppError({ code, status: res.status }) }
    }
    return { ok: true, data: (await res.json()) as T }
  } catch (err) {
    return { ok: false, error: toAppError(err) }
  }
}

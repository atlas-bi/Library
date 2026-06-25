export type AppErrorCode =
  | "auth_required"
  | "forbidden"
  | "not_found"
  | "bad_request"
  | "server_error"
  | "service_unavailable"
  | "unknown"

export type AppErrorOptions = {
  code: AppErrorCode
  status?: number
  message?: string
  cause?: unknown
}

export class AppError extends Error {
  public readonly code: AppErrorCode
  public readonly status?: number
  public readonly cause?: unknown

  constructor({ code, status, message, cause }: AppErrorOptions) {
    super(message ?? code)
    this.name = "AppError"
    this.code = code
    this.status = status
    this.cause = cause
  }
}

export function mapHttpStatusToErrorCode(status: number): AppErrorCode {
  if (status === 400) return "bad_request"
  if (status === 401) return "auth_required"
  if (status === 403) return "forbidden"
  if (status === 404) return "not_found"
  if (status === 503) return "service_unavailable"
  if (status >= 500) return "server_error"
  return "unknown"
}

export function getUserFriendlyErrorMessage(code: AppErrorCode): string {
  switch (code) {
    case "auth_required":
      return "Your session has expired. Please sign in again."
    case "forbidden":
      return "You do not have permission to view this content."
    case "not_found":
      return "The requested content is unavailable or does not exist."
    case "bad_request":
      return "This request was invalid. Please review your input and try again."
    case "server_error":
      return "We hit a server issue while processing your request. Please try again."
    case "service_unavailable":
      return "The service is temporarily unavailable. Please try again shortly."
    default:
      return "Something went wrong. Please try again."
  }
}

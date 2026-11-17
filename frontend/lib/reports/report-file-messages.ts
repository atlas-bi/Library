import type { AppErrorCode } from "@/lib/app-error"
import { getUserFriendlyErrorMessage } from "@/lib/errors"

export function cubeDownloadErrorMessage(code: AppErrorCode): string {
  if (code === "not_found") {
    return "No cube connection file is available for this report."
  }
  if (code === "forbidden") {
    return "You do not have permission to download this cube file."
  }
  return getUserFriendlyErrorMessage(code)
}

export function crystalAttachmentErrorMessage(code: AppErrorCode): string {
  if (code === "not_found") {
    return "That run output is no longer available."
  }
  if (code === "forbidden") {
    return "You do not have permission to open this run output."
  }
  return getUserFriendlyErrorMessage(code)
}

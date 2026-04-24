export type ErrorContext = Record<string, unknown>

export function reportError(error: unknown, context?: ErrorContext): void {
  if (context) {
    console.error(error, context)
    return
  }
  console.error(error)
}

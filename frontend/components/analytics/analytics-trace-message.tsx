import Link from "next/link"

type ParsedTraceMessage = {
  msg?: string
  url?: string
  lineNumber?: string
  column?: string
  errorMsg?: string
}

function parseTraceMessage(raw: string): ParsedTraceMessage {
  try {
    const parsed = JSON.parse(raw) as Record<string, unknown>
    return {
      msg: typeof parsed.msg === "string" ? parsed.msg : undefined,
      url: typeof parsed.url === "string" ? parsed.url : undefined,
      lineNumber:
        typeof parsed["line number"] === "string" || typeof parsed["line number"] === "number"
          ? String(parsed["line number"])
          : undefined,
      column:
        typeof parsed.column === "string" || typeof parsed.column === "number"
          ? String(parsed.column)
          : undefined,
      errorMsg: typeof parsed.errorMsg === "string" ? parsed.errorMsg : undefined,
    }
  } catch {
    return { errorMsg: raw }
  }
}

const TRACE_LEVEL_LABELS: Record<number, string> = {
  1000: "😊 Trace",
  2000: "🙂 Debug",
  3000: "😐 Info",
  4000: "😨 Warning",
  5000: "😠 Error",
  6000: "😡 Fatal",
}

export function analyticsTraceLevelLabel(level: number | null | undefined): string {
  if (level == null) return "—"
  return TRACE_LEVEL_LABELS[level] ?? String(level)
}

export function AnalyticsTraceMessage({
  message,
  referer,
  handled,
}: {
  message: string
  referer: string
  handled: number | null
}) {
  const parsed = parseTraceMessage(message)
  const location =
    parsed.url && parsed.lineNumber
      ? `${parsed.url}:${parsed.lineNumber}${parsed.column ? `:${parsed.column}` : ""}`
      : null
  const isResolved = handled === 1

  return (
    <div className="space-y-1 text-sm">
      {parsed.msg ? (
        <p>
          {parsed.msg} at{" "}
          <Link
            href={referer || "#"}
            target="_blank"
            rel="noreferrer"
            className={isResolved ? "text-muted-foreground" : "text-primary hover:underline"}
          >
            {referer}
          </Link>
        </p>
      ) : null}
      {location ? (
        <p>
          in{" "}
          <Link
            href={location}
            target="_blank"
            rel="noreferrer"
            className={isResolved ? "text-muted-foreground" : "text-primary hover:underline"}
          >
            {location}
          </Link>
        </p>
      ) : null}
      {parsed.errorMsg ? (
        <pre
          className={`mt-2 overflow-x-auto rounded-md p-2 text-xs ${
            isResolved ? "bg-muted/40 text-muted-foreground" : "bg-destructive/10 text-destructive"
          }`}
        >
          {parsed.errorMsg}
        </pre>
      ) : null}
    </div>
  )
}

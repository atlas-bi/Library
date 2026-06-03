import { NextResponse } from "next/server"
import { getCurrentUser } from "@/lib/auth"
import { getHomeTabPanel, getHomeUserPageSummary } from "@/lib/home/api"
import { isHomeTabId } from "@/lib/home/constants"

export async function GET(_request: Request, context: { params: Promise<{ tab: string }> }) {
  const user = await getCurrentUser()
  if (!user) {
    return NextResponse.json({ ok: false, error: "auth_required" }, { status: 401 })
  }

  const { tab } = await context.params
  if (!isHomeTabId(tab)) {
    return NextResponse.json({ ok: false, error: "not_found" }, { status: 404 })
  }

  const userId = Number(user.userId)
  if (!Number.isFinite(userId) || userId <= 0) {
    return NextResponse.json({ ok: false, error: "bad_request" }, { status: 400 })
  }

  const page = await getHomeUserPageSummary(userId)
  if (!page.data) {
    return NextResponse.json({ ok: false, error: page.error ?? "unknown" }, { status: 400 })
  }

  const panel = await getHomeTabPanel(userId, page.data.defaultReportTypeIds, tab)
  if (!panel.data) {
    return NextResponse.json({ ok: false, error: panel.error ?? "unknown" }, { status: 400 })
  }

  return NextResponse.json({ ok: true, data: panel.data })
}

import { cookies } from "next/headers"
import { type NextRequest, NextResponse } from "next/server"
import { getServerApiBase } from "@/lib/api-base"

export async function GET(request: NextRequest) {
  const token = (await cookies()).get("atlas_token")?.value
  const returnTo = request.nextUrl.searchParams.get("returnTo") || "/"
  const redirectUrl = new URL(returnTo, request.nextUrl.origin)

  if (!token) {
    return NextResponse.redirect(redirectUrl)
  }

  const apiBase = getServerApiBase()
  if (!apiBase) {
    return NextResponse.redirect(redirectUrl)
  }

  try {
    await fetch(`${apiBase}/api/users/me/admin-mode/toggle`, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
      },
      cache: "no-store",
    })
  } catch {
    return NextResponse.redirect(redirectUrl)
  }

  return NextResponse.redirect(redirectUrl)
}

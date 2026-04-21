import { redirect } from "next/navigation";
import { NextRequest } from "next/server";
import { getServerApiBase } from "@/lib/api-base";

export function GET(request: NextRequest) {
  const authReturnOrigin =
    process.env.AUTH_RETURN_URL_ORIGIN ?? request.nextUrl.origin;
  const returnUrl = `${authReturnOrigin.replace(/\/$/, "")}/auth/callback`;
  const apiBase = getServerApiBase(request.nextUrl.origin);
  redirect(
    `${apiBase}/api/auth/login?returnUrl=${encodeURIComponent(returnUrl)}`
  );
}

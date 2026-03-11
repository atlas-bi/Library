import { redirect } from "next/navigation";
import { NextRequest } from "next/server";

export function GET(request: NextRequest) {
  const returnUrl = `${request.nextUrl.origin}/auth/callback`;
  redirect(
    `${process.env.NEXT_PUBLIC_API_URL}/api/auth/login?returnUrl=${encodeURIComponent(returnUrl)}`
  );
}

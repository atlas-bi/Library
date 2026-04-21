import { NextRequest, NextResponse } from "next/server";

export function GET(request: NextRequest) {
  const response = NextResponse.redirect(
    new URL("/auth/login", request.nextUrl.origin)
  );
  response.cookies.delete("atlas_token");
  return response;
}

import { NextResponse } from "next/server";

export function GET() {
  const response = NextResponse.redirect(
    new URL("/auth/login", process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:3000")
  );
  response.cookies.delete("atlas_token");
  return response;
}

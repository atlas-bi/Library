import type { NextConfig } from "next"

const nextConfig: NextConfig = {
  output: "standalone",
  async rewrites() {
    const apiBase = process.env.API_URL ?? process.env.NEXT_PUBLIC_API_URL
    if (!apiBase) return []
    const normalized = apiBase.replace(/\/+$/, "")
    return [
      {
        source: "/api/:path*",
        destination: `${normalized}/api/:path*`,
      },
    ]
  },
}

export default nextConfig

import path from "node:path"
import type { NextConfig } from "next"

const nextConfig: NextConfig = {
  output: "standalone",
  outputFileTracingRoot: path.join(__dirname, ".."),
  async rewrites() {
    const apiBase = process.env.API_URL ?? process.env.NEXT_PUBLIC_API_URL
    if (!apiBase) return []
    const normalized = apiBase.replace(/\/+$/, "")
    const legacyPageRoutes = [
      "/settings",
      "/analytics",
      "/tasks",
      "/profile",
      "/initiatives",
      "/terms",
      "/users/settings",
      "/about_analytics",
    ]
    const legacyAssetRoutes = ["/css/:path*", "/js/:path*", "/font/:path*", "/img/:path*"]

    return [
      {
        source: "/api/:path((?!home(?:/|$)).*)",
        destination: `${normalized}/api/:path*`,
      },
      ...legacyAssetRoutes.map((route) => ({
        source: route,
        destination: `${normalized}${route}`,
      })),
      ...legacyPageRoutes.map((route) => ({
        source: route,
        destination: `${normalized}${route}`,
      })),
    ]
  },
}

export default nextConfig

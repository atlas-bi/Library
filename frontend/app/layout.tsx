import type { Metadata } from "next"
import "@fontsource/inter/400.css"
import "@fontsource/inter/500.css"
import "@fontsource/inter/600.css"
import "@fontsource/inter/700.css"
import "@fontsource/rasa/600.css"
import "@fontsource/rasa/700.css"
import "@fontsource/source-code-pro/400.css"
import { ThemeProvider } from "@/components/theme-provider"
import { Toaster } from "@/components/ui/sonner"
import { TooltipProvider } from "@/components/ui/tooltip"
import "./globals.css"

export const metadata: Metadata = {
  title: {
    template: "%s | Atlas BI Library",
    default: "Atlas BI Library",
  },
  description: "Atlas BI Library",
}

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode
}>) {
  return (
    <html lang="en" className="light" style={{ colorScheme: "light" }} suppressHydrationWarning>
      <body className="antialiased font-sans">
        <ThemeProvider attribute="class" forcedTheme="light" enableSystem={false}>
          <TooltipProvider>{children}</TooltipProvider>
          <Toaster />
        </ThemeProvider>
      </body>
    </html>
  )
}

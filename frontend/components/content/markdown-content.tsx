import type { ReactNode } from "react"
import ReactMarkdown from "react-markdown"
import remarkGfm from "remark-gfm"
import { cn } from "@/lib/utils"

const markdownComponents = {
  a: ({ href, children }: { href?: string; children?: ReactNode }) => (
    <a href={href} className="text-link underline underline-offset-2">
      {children}
    </a>
  ),
  p: ({ children }: { children?: ReactNode }) => (
    <p className="mb-3 leading-relaxed last:mb-0">{children}</p>
  ),
  ul: ({ children }: { children?: ReactNode }) => (
    <ul className="mb-3 list-disc space-y-1 pl-5">{children}</ul>
  ),
  ol: ({ children }: { children?: ReactNode }) => (
    <ol className="mb-3 list-decimal space-y-1 pl-5">{children}</ol>
  ),
  h1: ({ children }: { children?: ReactNode }) => (
    <h3 className="mb-2 text-xl font-semibold">{children}</h3>
  ),
  h2: ({ children }: { children?: ReactNode }) => (
    <h4 className="mb-2 text-lg font-semibold">{children}</h4>
  ),
  h3: ({ children }: { children?: ReactNode }) => (
    <h5 className="mb-2 text-base font-semibold">{children}</h5>
  ),
  code: ({ children }: { children?: ReactNode }) => (
    <code className="rounded bg-muted px-1 py-0.5 font-mono text-sm">{children}</code>
  ),
  pre: ({ children }: { children?: ReactNode }) => (
    <pre className="mb-3 overflow-x-auto rounded-md bg-[#363636] p-3 text-sm text-[#f5f5f5] [&_code]:bg-transparent [&_code]:p-0">{children}</pre>
  ),
}

export function MarkdownContent({ content, className }: { content: string; className?: string }) {
  return (
    <div
      className={cn(
        "markdown text-sm leading-relaxed text-foreground",
        className,
      )}
    >
      <ReactMarkdown remarkPlugins={[remarkGfm]} components={markdownComponents}>
        {content}
      </ReactMarkdown>
    </div>
  )
}

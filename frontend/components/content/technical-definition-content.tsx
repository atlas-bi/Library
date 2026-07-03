import { MarkdownContent } from "@/components/content/markdown-content"

/** Renders technical definitions like Razor: markdown when fenced, otherwise a dark code block. */
export function TechnicalDefinitionContent({ content }: { content: string }) {
  const trimmed = content.trim()
  const usesMarkdownFence = trimmed.startsWith("```") || trimmed.includes("\n```")

  if (usesMarkdownFence) {
    return <MarkdownContent content={content} />
  }

  return (
    <pre className="overflow-x-auto rounded-md bg-[#363636] px-4 py-3 text-sm leading-relaxed text-[#f5f5f5] [&_code]:bg-transparent [&_code]:p-0">
      <code>{content}</code>
    </pre>
  )
}

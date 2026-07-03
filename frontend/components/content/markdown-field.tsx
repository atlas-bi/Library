"use client"

import {
  Bold,
  Code,
  Eye,
  EyeOff,
  Heading,
  Italic,
  Link2,
  List,
  ListOrdered,
  Quote,
} from "lucide-react"
import type { ReactNode } from "react"
import { useCallback, useRef, useState } from "react"
import { MarkdownContent } from "@/components/content/markdown-content"
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { cn } from "@/lib/utils"

import CodeMirror, { ReactCodeMirrorRef } from "@uiw/react-codemirror"
import { markdown, markdownLanguage } from "@codemirror/lang-markdown"
import { EditorView } from "@codemirror/view"

function wrapSelection(view: any, before: string, after: string, placeholder: string) {
  if (!view) return
  const selection = view.state.selection.main
  const selected = view.state.sliceDoc(selection.from, selection.to) || placeholder
  const nextValue = `${before}${selected}${after}`
  view.dispatch({
    changes: { from: selection.from, to: selection.to, insert: nextValue },
    selection: { anchor: selection.from + before.length, head: selection.from + before.length + selected.length },
  })
}

function prefixLines(view: any, prefix: string, placeholder: string) {
  if (!view) return
  const selection = view.state.selection.main
  const selected = view.state.sliceDoc(selection.from, selection.to) || placeholder
  const lines = selected.split("\n").map((line: string) => `${prefix}${line}`)
  const nextValue = lines.join("\n")
  view.dispatch({
    changes: { from: selection.from, to: selection.to, insert: nextValue },
    selection: { anchor: selection.from, head: selection.from + nextValue.length },
  })
}

export function MarkdownField({
  id,
  label,
  value,
  onChange,
  rows = 8,
}: {
  id: string
  label: string
  value: string
  onChange: (value: string) => void
  rows?: number
}) {
  const editorRef = useRef<ReactCodeMirrorRef>(null)
  const [preview, setPreview] = useState(false)

  const applyEdit = useCallback(
    (edit: (view: any) => void) => {
      const view = editorRef.current?.view
      if (!view) return
      edit(view)
      window.requestAnimationFrame(() => {
        view.focus()
      })
    },
    []
  )

  const toolbarButton = (
    icon: ReactNode,
    labelText: string,
    onClick: () => void,
  ) => (
    <Button
      type="button"
      variant="ghost"
      size="icon-xs"
      className="size-8 rounded-md text-muted-foreground hover:text-foreground"
      aria-label={labelText}
      onClick={onClick}
    >
      {icon}
    </Button>
  )

  return (
    <div className="space-y-2">
      <Label htmlFor={id}>{label}</Label>
      <div className="overflow-hidden rounded-lg border border-input bg-background shadow-sm">
        <div className="flex items-center justify-between border-b bg-muted/40 px-2 py-1.5 z-10 relative">
          <div className="flex flex-wrap items-center gap-0.5">
            {toolbarButton(<Bold className="size-4" />, "Bold", () => {
              applyEdit((view) => wrapSelection(view, "**", "**", "bold text"))
            })}
            {toolbarButton(<Italic className="size-4" />, "Italic", () => {
              applyEdit((view) => wrapSelection(view, "*", "*", "italic text"))
            })}
            {toolbarButton(<Heading className="size-4" />, "Heading", () => {
              applyEdit((view) => prefixLines(view, "## ", "Heading"))
            })}
            <span className="mx-1 h-5 w-px bg-border" aria-hidden />
            {toolbarButton(<Quote className="size-4" />, "Blockquote", () => {
              applyEdit((view) => prefixLines(view, "> ", "Quote"))
            })}
            {toolbarButton(<Code className="size-4" />, "Code", () => {
              applyEdit((view) => wrapSelection(view, "`", "`", "code"))
            })}
            {toolbarButton(<List className="size-4" />, "Bulleted list", () => {
              applyEdit((view) => prefixLines(view, "- ", "List item"))
            })}
            {toolbarButton(<ListOrdered className="size-4" />, "Numbered list", () => {
              applyEdit((view) => prefixLines(view, "1. ", "List item"))
            })}
            {toolbarButton(<Link2 className="size-4" />, "Link", () => {
              applyEdit((view) => wrapSelection(view, "[", "](https://)", "link text"))
            })}
          </div>
          <Button
            type="button"
            variant="ghost"
            size="icon-xs"
            className="size-8 rounded-md text-muted-foreground hover:text-foreground"
            aria-label={preview ? "Hide preview" : "Show preview"}
            onClick={() => {
              setPreview((current) => !current)
            }}
          >
            {preview ? <EyeOff className="size-4" /> : <Eye className="size-4" />}
          </Button>
        </div>
        {preview ? (
          <div className="min-h-[160px] p-3">
            {value.trim() ? (
              <MarkdownContent content={value} className="border-0 bg-transparent p-0" />
            ) : (
              <p className="text-sm text-muted-foreground">Nothing to preview yet.</p>
            )}
          </div>
        ) : (
          <div className="min-h-[80px] relative">
            <CodeMirror
              ref={editorRef}
              id={id}
              value={value}
              onChange={(val) => {
                onChange(val)
              }}
              extensions={[markdown({ base: markdownLanguage }), EditorView.lineWrapping]}
              className={cn("text-sm", "[&_.cm-editor.cm-focused]:outline-none", "[&_.cm-scroller]:p-3")}
              minHeight="80px"
              basicSetup={{
                lineNumbers: false,
                foldGutter: false,
                highlightActiveLine: false,
                highlightActiveLineGutter: false,
                bracketMatching: true,
                closeBrackets: true,
                autocompletion: false,
              }}
            />
          </div>
        )}
      </div>
    </div>
  )
}

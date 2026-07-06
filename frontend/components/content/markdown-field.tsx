"use client"

import { useRef } from "react"
import { Label } from "@/components/ui/label"
import { cn } from "@/lib/utils"

import CodeMirror, { ReactCodeMirrorRef } from "@uiw/react-codemirror"
import { markdown, markdownLanguage } from "@codemirror/lang-markdown"
import { EditorView } from "@codemirror/view"

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

  return (
    <div className="space-y-2">
      <Label htmlFor={id}>{label}</Label>
      <div className="overflow-hidden rounded-md border border-input bg-background shadow-sm">
        <div className="relative">
          <CodeMirror
            ref={editorRef}
            id={id}
            value={value}
            onChange={(val: string) => {
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
      </div>
    </div>
  )
}

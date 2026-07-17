"use client"

import { Bold, Code, Eye, Heading, Italic, Link as LinkIcon, List, Quote } from "lucide-react"
import { useEffect, useState, useTransition } from "react"
import { Button } from "@/components/ui/button"
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { shareMailAction } from "@/lib/initiatives/actions"

interface ShareModalProps {
  isOpen: boolean
  onClose: () => void
  initiativeName: string
  initiativeId: number
}

export function ShareModal({ isOpen, onClose, initiativeName, initiativeId }: ShareModalProps) {
  const [to, setTo] = useState("")
  const [subject, setSubject] = useState(`[Share] ${initiativeName}`)
  const [message, setMessage] = useState("")

  const [isPending, startTransition] = useTransition()

  useEffect(() => {
    if (isOpen) {
      const origin =
        typeof window !== "undefined" ? window.location.origin : "http://localhost:5001"
      setMessage(`Hi!\n\nI would like to share this initiative with you.\n\n[${initiativeName}](${origin}/initiatives?id=${initiativeId})\n\nCheck it out sometime!\nRegards!`)
    }
  }, [isOpen, initiativeName, initiativeId])

  const handleSend = () => {
    if (!to) return
    startTransition(() => {
      void (async () => {
        const result = await shareMailAction({
          to: [{ type: "Email", userId: null }], // TODO: Update to proper recipient parsing when typeahead is implemented
          subject,
          message,
          share: true,
          shareName: initiativeName,
          shareUrl: `/initiatives?id=${initiativeId}`,
        })
        if (result.error) {
          alert(`Error sharing initiative: ${result.error}`)
        } else {
          onClose()
        }
      })()
    })
  }

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-[500px]">
        <DialogHeader>
          <DialogTitle className="text-[1.1rem] font-bold text-[#363636]">
            Share {initiativeName}
          </DialogTitle>
        </DialogHeader>
        <div className="grid gap-4 py-2">
          <div className="flex flex-col gap-1">
            <label className="text-[0.875rem] font-bold text-[#363636]">To:</label>
            <Input
              placeholder="search for someone.."
              value={to}
              onChange={(e) => setTo(e.target.value)}
              className="h-9 rounded-[4px] border-[#dbdbdb] shadow-none focus-visible:border-[#3273dc] focus-visible:ring-0"
            />
          </div>
          <div className="flex flex-col gap-1">
            <label className="text-[0.875rem] font-bold text-[#363636]">Subject</label>
            <Input
              value={subject}
              onChange={(e) => setSubject(e.target.value)}
              className="h-9 rounded-[4px] border-[#dbdbdb] shadow-none focus-visible:border-[#3273dc] focus-visible:ring-0"
            />
          </div>
          <div className="flex flex-col gap-1">
            <label className="text-[0.875rem] font-bold text-[#363636]">Message</label>
            <div className="overflow-hidden rounded-[4px] border border-[#dbdbdb] bg-white focus-within:border-[#3273dc]">
              {/* WYSIWYG Toolbar mock */}
              <div className="flex items-center gap-1 border-b border-[#dbdbdb] bg-[#fafafa] p-1 text-[#7a7a7a]">
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-7 w-7 rounded-[4px] hover:bg-[#dbdbdb] hover:text-[#363636]"
                >
                  <Bold className="h-[14px] w-[14px]" />
                </Button>
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-7 w-7 rounded-[4px] hover:bg-[#dbdbdb] hover:text-[#363636]"
                >
                  <Italic className="h-[14px] w-[14px]" />
                </Button>
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-7 w-7 rounded-[4px] hover:bg-[#dbdbdb] hover:text-[#363636]"
                >
                  <Heading className="h-[14px] w-[14px]" />
                </Button>
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-7 w-7 rounded-[4px] hover:bg-[#dbdbdb] hover:text-[#363636]"
                >
                  <Quote className="h-[14px] w-[14px]" />
                </Button>
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-7 w-7 rounded-[4px] hover:bg-[#dbdbdb] hover:text-[#363636]"
                >
                  <Code className="h-[14px] w-[14px]" />
                </Button>
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-7 w-7 rounded-[4px] hover:bg-[#dbdbdb] hover:text-[#363636]"
                >
                  <List className="h-[14px] w-[14px]" />
                </Button>
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-7 w-7 rounded-[4px] hover:bg-[#dbdbdb] hover:text-[#363636]"
                >
                  <LinkIcon className="h-[14px] w-[14px]" />
                </Button>
                <div className="flex-1"></div>
                <Button
                  variant="ghost"
                  size="icon"
                  className="h-7 w-7 rounded-[4px] hover:bg-[#dbdbdb] hover:text-[#363636]"
                >
                  <Eye className="h-[14px] w-[14px]" />
                </Button>
              </div>
              <Textarea
                value={message}
                onChange={(e) => setMessage(e.target.value)}
                className="min-h-[160px] resize-none border-0 p-3 text-[1rem] shadow-none focus-visible:ring-0 text-[#363636]"
              />
            </div>
          </div>
        </div>
        <div className="mt-2 flex justify-start">
          <Button
            variant="outline"
            className="h-9 rounded-[4px] border-[#dbdbdb] px-5 font-normal text-[#363636] shadow-none hover:border-[#b5b5b5] hover:text-[#363636]"
            onClick={handleSend}
            disabled={isPending || !to}
          >
            {isPending ? "Sending..." : "Send"}
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  )
}

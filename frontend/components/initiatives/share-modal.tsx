"use client"

import { useState } from "react"
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Bold, Italic, Heading, Quote, Code, List, Link as LinkIcon, Eye } from "lucide-react"

interface ShareModalProps {
  isOpen: boolean
  onClose: () => void
  initiativeName: string
  initiativeId: number
}

export function ShareModal({ isOpen, onClose, initiativeName, initiativeId }: ShareModalProps) {
  const [to, setTo] = useState("")
  const [subject, setSubject] = useState(`[Share] ${initiativeName}`)
  const [message, setMessage] = useState(`Hi!

I would like to share this initiative with you.

[${initiativeName}](http://localhost:5001/initiatives?id=${initiativeId})

Check it out sometime!
Regards!`)

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-[500px]">
        <DialogHeader>
          <DialogTitle className="text-[1.1rem] font-bold text-[#363636]">Share {initiativeName}</DialogTitle>
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
                <Button variant="ghost" size="icon" className="h-7 w-7 rounded-[4px] hover:bg-[#dbdbdb] hover:text-[#363636]"><Bold className="h-[14px] w-[14px]" /></Button>
                <Button variant="ghost" size="icon" className="h-7 w-7 rounded-[4px] hover:bg-[#dbdbdb] hover:text-[#363636]"><Italic className="h-[14px] w-[14px]" /></Button>
                <Button variant="ghost" size="icon" className="h-7 w-7 rounded-[4px] hover:bg-[#dbdbdb] hover:text-[#363636]"><Heading className="h-[14px] w-[14px]" /></Button>
                <Button variant="ghost" size="icon" className="h-7 w-7 rounded-[4px] hover:bg-[#dbdbdb] hover:text-[#363636]"><Quote className="h-[14px] w-[14px]" /></Button>
                <Button variant="ghost" size="icon" className="h-7 w-7 rounded-[4px] hover:bg-[#dbdbdb] hover:text-[#363636]"><Code className="h-[14px] w-[14px]" /></Button>
                <Button variant="ghost" size="icon" className="h-7 w-7 rounded-[4px] hover:bg-[#dbdbdb] hover:text-[#363636]"><List className="h-[14px] w-[14px]" /></Button>
                <Button variant="ghost" size="icon" className="h-7 w-7 rounded-[4px] hover:bg-[#dbdbdb] hover:text-[#363636]"><LinkIcon className="h-[14px] w-[14px]" /></Button>
                <div className="flex-1"></div>
                <Button variant="ghost" size="icon" className="h-7 w-7 rounded-[4px] hover:bg-[#dbdbdb] hover:text-[#363636]"><Eye className="h-[14px] w-[14px]" /></Button>
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
          <Button variant="outline" className="h-9 rounded-[4px] border-[#dbdbdb] px-5 font-normal text-[#363636] shadow-none hover:border-[#b5b5b5] hover:text-[#363636]" onClick={onClose}>Send</Button>
        </div>
      </DialogContent>
    </Dialog>
  )
}

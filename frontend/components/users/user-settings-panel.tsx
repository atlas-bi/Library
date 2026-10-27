"use client"

import { useState, useTransition } from "react"
import { updateUserSettingsAction } from "@/app/users/actions"
import { Label } from "@/components/ui/label"
import { Switch } from "@/components/ui/switch"

interface UserSettingsPanelProps {
  initialShareNotificationEnabled: boolean
}

export function UserSettingsPanel({ initialShareNotificationEnabled }: UserSettingsPanelProps) {
  const [shareNotificationEnabled, setShareNotificationEnabled] = useState(
    initialShareNotificationEnabled,
  )
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState(false)
  const [isPending, startTransition] = useTransition()

  function handleShareNotificationChange(checked: boolean) {
    const previousValue = shareNotificationEnabled

    setShareNotificationEnabled(checked)
    setError(null)
    setSuccess(false)

    startTransition(async () => {
      const result = await updateUserSettingsAction({ shareNotificationEnabled: checked })
      if (result.error) {
        setShareNotificationEnabled(previousValue)
        setError(result.error)
        return
      }

      setSuccess(true)
    })
  }

  return (
    <section className="space-y-4">
      <h2 className="text-xl font-bold font-serif text-[#2c3e50]">Email</h2>

      {error && (
        <div className="bg-red-50 text-red-600 border border-red-200 p-3 rounded-md text-sm">
          {error}
        </div>
      )}

      {success && (
        <div className="bg-green-50 text-green-700 border border-green-200 p-3 rounded-md text-sm">
          Settings saved.
        </div>
      )}

      <div className="flex items-center gap-3">
        <Switch
          id="share-notification"
          checked={shareNotificationEnabled}
          disabled={isPending}
          onCheckedChange={handleShareNotificationChange}
        />
        <Label htmlFor="share-notification">Enable Share Notification</Label>
      </div>
    </section>
  )
}

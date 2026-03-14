"use client";

import { useEffect } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { Suspense } from "react";

import { Card, CardContent } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";

function CallbackHandler() {
  const router = useRouter();
  const params = useSearchParams();

  useEffect(() => {
    const token = params.get("token");
    if (token) {
      document.cookie = `atlas_token=${token}; path=/; max-age=${8 * 60 * 60}; SameSite=Lax`;
      router.replace("/");
    } else {
      router.replace("/auth/login");
    }
  }, [params, router]);

  return (
    <div className="flex items-center justify-center min-h-screen bg-background px-6">
      <Card className="w-full max-w-sm">
        <CardContent className="py-10 flex flex-col items-center text-center gap-4">
          <Skeleton className="h-8 w-8 rounded-full" />
          <p className="text-muted-foreground text-sm">Signing you in...</p>
        </CardContent>
      </Card>
    </div>
  );
}

export default function CallbackPage() {
  return (
    <Suspense>
      <CallbackHandler />
    </Suspense>
  );
}

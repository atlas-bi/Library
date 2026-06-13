import fs from "node:fs"
import path from "node:path"
import { fileURLToPath } from "node:url"

const __filename = fileURLToPath(import.meta.url)
const __dirname = path.dirname(__filename)
const root = path.resolve(__dirname, "..")
const huskyDir = path.join(root, ".husky")

// Avoid running in CI environments (mirrors common Husky patterns)
if (process.env.CI) {
  process.exit(0)
}

const hooksDir = path.join(root, ".git", "hooks")
if (!fs.existsSync(hooksDir)) {
  // Not a git checkout (or .git missing) – silently no-op.
  process.exit(0)
}

const huskySh = `#!/usr/bin/env sh
# Minimal husky shim

if [ -z "$husky_skip_init" ]; then
  debug () {
    if [ "\${HUSKY_DEBUG}" = "1" ]; then
      echo "husky (debug) - $1"
    fi
  }

  readonly hook_name="$(basename "$0")"
  debug "starting $hook_name..."

  if [ "$HUSKY" = "0" ]; then
    debug "HUSKY=0, skipping hook"
    exit 0
  fi

  if [ -f ~/.huskyrc ]; then
    debug "sourcing ~/.huskyrc"
    . ~/.huskyrc
  fi
fi
`

const ensureDir = (p) => {
  if (!fs.existsSync(p)) fs.mkdirSync(p, { recursive: true })
}

ensureDir(path.join(huskyDir, "_"))

// Minimal husky shim so the hook scripts can run consistently.
fs.writeFileSync(path.join(huskyDir, "_", "husky.sh"), huskySh, "utf8")

#!/usr/bin/env sh
# Minimal husky shim

if [ -z "$husky_skip_init" ]; then
  debug () {
    if [ "${HUSKY_DEBUG}" = "1" ]; then
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

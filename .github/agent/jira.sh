#!/usr/bin/env bash
# Minimal Jira Cloud helper for the agent workflow.
# Usage:
#   jira.sh fetch <KEY> <outfile>
#   jira.sh comment <KEY> <text>
#   jira.sh labels <KEY> <label-to-add|-> <label-to-remove|->
#   jira.sh transition <KEY> <transition-name>
# Requires env: JIRA_BASE_URL, JIRA_EMAIL, JIRA_API_TOKEN.
# Free text is passed through jq --arg, never interpolated into JSON or shell.
set -euo pipefail

: "${JIRA_BASE_URL:?}" "${JIRA_EMAIL:?}" "${JIRA_API_TOKEN:?}"
BASE="${JIRA_BASE_URL%/}/rest/api/3"

api() { # method path [json-body]
  local method="$1" path="$2" body="${3:-}"
  local args=(-sS -u "${JIRA_EMAIL}:${JIRA_API_TOKEN}" -H "Accept: application/json" -X "$method" -w '\n%{http_code}')
  if [[ -n "$body" ]]; then
    args+=(-H "Content-Type: application/json" --data "$body")
  fi
  local out code
  out="$(curl "${args[@]}" "${BASE}${path}")"
  code="${out##*$'\n'}"
  if [[ ! "$code" =~ ^2 ]]; then
    echo "Jira API ${method} ${path} failed with HTTP ${code}" >&2
    return 1
  fi
  printf '%s' "${out%$'\n'*}"
}

cmd="${1:?command required}"
shift

case "$cmd" in
  fetch)
    key="$1"
    outfile="$2"
    api GET "/issue/${key}?fields=summary,description,labels,status" > "${outfile}.json"
    jq -r '
      def adf: [.. | objects
                | select(.type == "paragraph" or .type == "heading" or .type == "listItem")
                | [.. | objects | select(.type == "text") | .text] | join("")]
               | map(select(length > 0)) | join("\n");
      "Ticket: \(.key)\nSummary: \(.fields.summary)\nStatus: \(.fields.status.name)\nLabels: \(.fields.labels | join(", "))\n\nDescription:\n\(if .fields.description then (.fields.description | adf) else "(none)" end)"
    ' "${outfile}.json" > "$outfile"
    ;;
  comment)
    key="$1"
    text="$2"
    body="$(jq -n --arg t "$text" '{body:{type:"doc",version:1,content:[{type:"paragraph",content:[{type:"text",text:$t}]}]}}')"
    api POST "/issue/${key}/comment" "$body" > /dev/null
    ;;
  labels)
    key="$1"
    add="$2"
    remove="$3"
    body="$(jq -n --arg a "$add" --arg r "$remove" \
      '{update:{labels:([(if $a != "-" then {add:$a} else empty end), (if $r != "-" then {remove:$r} else empty end)])}}')"
    api PUT "/issue/${key}" "$body" > /dev/null
    ;;
  transition)
    key="$1"
    name="$2"
    id="$(api GET "/issue/${key}/transitions" \
      | jq -r --arg n "$name" '.transitions[] | select((.name | ascii_downcase) == ($n | ascii_downcase)) | .id' \
      | head -n1)"
    if [[ -z "$id" ]]; then
      echo "No transition named '${name}' for ${key}; skipping." >&2
      exit 0
    fi
    api POST "/issue/${key}/transitions" "$(jq -n --arg id "$id" '{transition:{id:$id}}')" > /dev/null
    ;;
  *)
    echo "unknown command: $cmd" >&2
    exit 2
    ;;
esac

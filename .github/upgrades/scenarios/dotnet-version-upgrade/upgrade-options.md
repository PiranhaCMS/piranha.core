# Upgrade Options — Piranha CMS

Assessment: 26 projects, all target net8.0/net9.0, upgrading to net10.0, 7-tier dependency graph, 65 issues identified

## Strategy

### Upgrade Strategy
26 projects with a deep 7-tier dependency graph. Top-Down approach provides incremental buildability at scale, allowing critical applications to be upgraded first while maintaining solution stability through multi-targeting.

| Value | Description |
|-------|-------------|
| **Top-Down** (selected) | Upgrade entry-point applications first, multi-target shared libraries, consolidate in second phase |
| All-at-Once | Upgrade all projects simultaneously in a single atomic pass |

---
inclusion: manual
---

# Phase Execution Prompt Template

Use this prompt when starting a new phase. Replace `[X]` with the phase number.

---

## The Prompt (copy-paste this)

```
Implement Phase [X] completely as defined in /Planning-Docs/phase-[X].md

Context documents (read all before starting):
- /Kiro-read-this-first.md (execution rules and completion report format)
- /docs/kiro-command-permissions.md (all pre-approved commands - execute without asking)
- /docs/architecture-overview.md (system architecture)
- /docs/project-structure.md (folder structure and conventions)
- /docs/domain-model.md (domain entities and relationships)
- /docs/database-design.md (schema design)
- /docs/business-rules.md (validation and logic rules)

Execution rules:
1. Do NOT ask me any questions.
2. Do NOT stop for confirmation at any point.
3. Do NOT present options or alternatives for me to choose from.
4. Do NOT ask "shall I continue?" or "would you like me to proceed?"
5. Make ALL implementation decisions yourself based on project documentation.
6. If you encounter ambiguity, make the best professional decision and document it in your completion report under "Assumptions Made".
7. Execute all pre-approved commands from kiro-command-permissions.md without prompting.
8. Implement ALL layers end-to-end (Domain → Application → Infrastructure → API → Frontend → Tests).
9. Follow existing patterns, naming conventions, and architecture established in the codebase.
10. When complete, provide the full phase completion report as defined in Kiro-read-this-first.md.

Begin immediately. No preamble. No questions. Just implement.
```

---

## Notes

- If a phase requires specific technology choices not in the docs, add a one-liner:
  e.g., "Use SQL Server LocalDB for database. Use ASP.NET Identity for auth."
- Keep it to one message. Do not send follow-ups while execution is in progress.
- The more complete your phase-[X].md document is, the fewer assumptions the agent needs to make.

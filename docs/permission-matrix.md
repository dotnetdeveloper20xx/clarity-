# Permission Matrix

## Role Permissions

| Action | Admin | Consultant | TeamLeader | LegalAssistant | Finance | Compliance | Support | Client |
|--------|-------|-----------|-----------|---------------|---------|-----------|---------|--------|
| **Clients** |
| View clients | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | Own only |
| Create client | ✓ | ✓ | ✓ | ✓ | — | — | — | — |
| Edit client | ✓ | ✓ | ✓ | ✓ | — | — | — | — |
| Archive client | ✓ | — | — | — | — | — | — | — |
| **Matters** |
| View matters | ✓ | Assigned | Team | Assigned | Related | Flagged | — | Own only |
| Create matter | ✓ | ✓ | ✓ | — | — | — | — | — |
| Edit matter | ✓ | ✓ | ✓ | ✓ | — | — | — | — |
| Close matter | ✓ | — | ✓ | — | — | — | — | — |
| Reopen matter | ✓ | — | — | — | — | — | — | — |
| **Time Recording** |
| Record time | ✓ | ✓ | ✓ | Non-billable | — | — | — | — |
| Submit time | ✓ | Own | Own | Own | — | — | — | — |
| Approve time | ✓ | — | ✓ | — | — | — | — | — |
| Reject time | ✓ | — | ✓ | — | — | — | — | — |
| **Billing** |
| View invoices | ✓ | — | ✓ | — | ✓ | — | — | Own only |
| Create invoice | ✓ | — | — | — | ✓ | — | — | — |
| Issue invoice | ✓ | — | — | — | ✓ | — | — | — |
| Record payment | ✓ | — | — | — | ✓ | — | — | — |
| Write off | ✓ | — | — | — | ✓ | — | — | — |
| **Documents** |
| Upload document | ✓ | ✓ | ✓ | ✓ | — | — | — | ✓ |
| Download document | ✓ | Assigned | Team | Assigned | — | Flagged | — | Visible only |
| Archive document | ✓ | ✓ | ✓ | — | — | — | — | — |
| **Compliance** |
| View compliance | ✓ | — | — | — | — | ✓ | — | — |
| Create check | ✓ | — | — | — | — | ✓ | — | — |
| Complete check | ✓ | — | — | — | — | ✓ | — | — |
| **Administration** |
| Manage users | ✓ | — | — | — | — | — | — | — |
| Manage roles | ✓ | — | — | — | — | — | — | — |
| View diagnostics | ✓ | — | — | — | — | — | ✓ | — |
| View audit log | ✓ | — | — | — | — | ✓ | ✓ | — |
| View security log | ✓ | — | — | — | — | — | — | — |
| Manage sessions | ✓ | — | — | — | — | — | — | — |

## Notes

- "Own" means the user can only perform the action on their own records
- "Assigned" means the user must be a team member on the matter
- "Team" means the user can see all matters assigned to their team
- "Flagged" means compliance can see matters that have compliance flags
- "Related" means finance can see billing information for matters they handle
- "Visible only" means clients can only see documents explicitly marked as client-visible

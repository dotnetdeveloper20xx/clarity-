# Support Playbooks

## Common Scenarios

### 1. User Cannot Log In

**Symptoms**: User reports "Invalid login details" error.

**Steps**:
1. Check if account exists: `GET /api/security/locked-accounts` (Admin)
2. If locked: `POST /api/security/users/{id}/unlock`
3. If not locked: verify email is correct (case-sensitive)
4. If password forgotten: admin must reset (or user uses change-password after unlock)
5. Check security audit log for login attempts: `GET /api/security/audit-log?eventType=LoginFailed&userId={id}`

---

### 2. User Gets "Forbidden" Error

**Symptoms**: User sees 403 error when accessing a feature.

**Steps**:
1. Check what role the endpoint requires (see `docs/permission-matrix.md`)
2. Check user's current roles: `GET /api/auth/me` (as the user)
3. If role is missing: Admin assigns via user management
4. Check if matter-level access is blocking (for matter-specific operations)

---

### 3. Missing Data / Blank Page

**Symptoms**: User reports page shows "No data" or loading spinner never ends.

**Steps**:
1. Ask for the **Correlation ID** from the error (if shown)
2. Check API logs for that correlation ID
3. Check browser Network tab for failed requests
4. Verify the data exists in the database
5. Verify the user has permission to see the data (role + matter access)

---

### 4. Invoice Shows Wrong Amount

**Symptoms**: Invoice total doesn't match expected.

**Steps**:
1. Get invoice ID
2. Check invoice line items in database
3. Verify time entry rates match billing rates at time of entry
4. Check if tax rate was applied correctly
5. Review audit trail: `GET /api/timeline/matter/{matterId}`

---

### 5. Background Job Failed

**Symptoms**: Expected notification/email not received, or admin dashboard shows failed jobs.

**Steps**:
1. Check job status: `GET /api/diagnostics/jobs?status=Failed`
2. Review error message in the response
3. Fix underlying issue (data, connection, etc.)
4. Retry: `POST /api/diagnostics/jobs/{id}/retry`
5. If dead-lettered: investigate manually, fix, then retry

---

### 6. Slow Dashboard Loading

**Symptoms**: Dashboard takes more than 3 seconds to load.

**Steps**:
1. Check API logs for "Long running request" warnings
2. Check database query performance (execution plans)
3. Verify indexes exist on commonly queried columns
4. Check if data volume has grown beyond expected thresholds
5. Consider adding caching for dashboard aggregates

---

### 7. Document Upload Fails

**Symptoms**: User gets error when uploading a file.

**Steps**:
1. Check file size (limit: 50MB)
2. Check file type (restricted types may be configured)
3. Check storage directory has write permissions
4. Check disk space on storage location
5. Check API logs for the correlation ID

---

### 8. Compliance Check Blocking Matter

**Symptoms**: User cannot close a matter, gets compliance error.

**Steps**:
1. Check compliance checks for the client: `GET /api/compliance/checks?clientId={id}`
2. Identify failed or pending checks
3. Compliance officer must resolve the check
4. Once resolved, matter closure should succeed

---

## Escalation Path

| Level | Who | When |
|-------|-----|------|
| L1 | Support User | First contact, known issues, playbook scenarios |
| L2 | Senior Developer | Unknown issues, code investigation needed |
| L3 | Technical Lead | Architecture issues, data corruption, security incidents |

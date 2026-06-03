# Secure Configuration

## Principle

Secrets must never be stored in source code or committed to version control.

## Development Environment

For local development, secrets are stored in `appsettings.json` (acceptable for development only):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=DESKTOP-VVJN96B;Database=ClarityDb;Trusted_Connection=True;..."
  },
  "Jwt": {
    "Key": "ClarityDevelopmentSecretKey12345678",
    "Issuer": "Clarity",
    "Audience": "ClarityUsers"
  }
}
```

For additional security in development, use .NET User Secrets:

```
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "my-dev-secret-key"
```

## Production Environment

In production, secrets must come from a secure source:

| Secret | Source |
|--------|--------|
| Database connection string | Azure Key Vault / Environment variable |
| JWT signing key | Azure Key Vault |
| Email API key | Azure Key Vault |
| Storage connection string | Azure Key Vault / Managed Identity |

### Azure Key Vault Integration (Future)

```csharp
builder.Configuration.AddAzureKeyVault(
    new Uri("https://clarity-vault.vault.azure.net/"),
    new DefaultAzureCredential());
```

## What Counts as a Secret

- Database passwords
- JWT signing keys
- API keys for external services
- Storage account keys
- Email service credentials
- Encryption keys

## Rules

1. **Never commit secrets** — `.gitignore` excludes `.env`, `*.user`, secret files
2. **Never log secrets** — Serilog must not write connection strings or keys
3. **Never return secrets** — API responses must never include keys/passwords
4. **Rotate regularly** — JWT keys and API keys should be rotated periodically
5. **Minimum privilege** — Database accounts use minimum required permissions

using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using Xunit;

namespace Clarity.Tests.Security;

public class PasswordHashingTests
{
    [Fact]
    public void PBKDF2_Hash_CanBeVerified()
    {
        var password = "TestPassword123!";
        var hash = HashPassword(password);
        
        hash.Should().StartWith("PBKDF2$");
        VerifyPassword(password, hash).Should().BeTrue();
    }

    [Fact]
    public void PBKDF2_WrongPassword_FailsVerification()
    {
        var hash = HashPassword("CorrectPassword");
        VerifyPassword("WrongPassword", hash).Should().BeFalse();
    }

    [Fact]
    public void PBKDF2_SamePassword_ProducesDifferentHashes()
    {
        var hash1 = HashPassword("SamePassword");
        var hash2 = HashPassword("SamePassword");
        hash1.Should().NotBe(hash2, "different salts should produce different hashes");
    }

    [Fact]
    public void LegacySHA256_CanBeVerified()
    {
        var password = "Admin123!";
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        var legacyHash = Convert.ToBase64String(bytes);

        VerifyPassword(password, legacyHash).Should().BeTrue();
    }

    [Fact]
    public void LegacySHA256_WrongPassword_FailsVerification()
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes("Admin123!"));
        var legacyHash = Convert.ToBase64String(bytes);

        VerifyPassword("WrongPassword!", legacyHash).Should().BeFalse();
    }

    // Replicated from AuthController for testing
    private static string HashPassword(string password)
    {
        var salt = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        const int iterations = 100_000;
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);
        return $"PBKDF2${Convert.ToBase64String(salt)}${iterations}${Convert.ToBase64String(hash)}";
    }

    private static bool VerifyPassword(string password, string hash)
    {
        if (hash.StartsWith("PBKDF2$"))
        {
            var parts = hash.Split('$');
            if (parts.Length != 4) return false;
            var salt = Convert.FromBase64String(parts[1]);
            var iterations = int.Parse(parts[2]);
            var storedHash = Convert.FromBase64String(parts[3]);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var computedHash = pbkdf2.GetBytes(32);
            return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
        }
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes) == hash;
    }
}

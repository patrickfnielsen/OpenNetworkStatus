using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace OpenNetworkStatus.Services.Authentication
{
    public sealed class PasswordHasher
    {
        public int IterationCount { get; } = 10000;
        public int SubkeyLength { get; } = 256 / 8; // 256 bits
        public int SaltSize { get; } = 128 / 8; // 128 bits

        public string HashPassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            var salt = GenerateSalt(SaltSize);
            var hashed = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: IterationCount,
                numBytesRequested: SubkeyLength
            );

            var inArray = new byte[SaltSize + SubkeyLength];
            Buffer.BlockCopy(salt, 0, inArray, 0, SaltSize);
            Buffer.BlockCopy(hashed, 0, inArray, 0 + SaltSize, SubkeyLength);

            return Convert.ToBase64String(inArray);
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            if (hashedPassword == null)
            {
                return PasswordVerificationResult.Failed;
            }

            byte[] numArray = Convert.FromBase64String(hashedPassword);
            if (numArray.Length < 1)
            {
                return PasswordVerificationResult.Failed;
            }


            byte[] salt = new byte[SaltSize];
            Buffer.BlockCopy(numArray, 0, salt, 0, SaltSize);

            byte[] storedHashedPassword = new byte[SubkeyLength];
            Buffer.BlockCopy(numArray, 0 + SaltSize, storedHashedPassword, 0, SubkeyLength);

            var newHashedPassword = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: IterationCount,
                numBytesRequested: SubkeyLength
            );

            if (CryptographicOperations.FixedTimeEquals(storedHashedPassword, newHashedPassword))
            {
                return PasswordVerificationResult.Success;
            }

            return PasswordVerificationResult.Failed;
        }

        private byte[] GenerateSalt(int saltSize)
        {
            byte[] salt = new byte[saltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }
    }

    public enum PasswordVerificationResult
    {
        Failed,
        Success
    }
}

using System;
using Microsoft.AspNet.Identity;
using opcode4.crypto;

namespace opcode4.web.Membership
{
    public class CustomPasswordHasher : PasswordHasher
    {
        public override string HashPassword(string password)
        {
            return Hashes.MD5(password);
            //return base.HashPassword(password);
        }

        public override PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providerPassword)
        {
            if (hashedPassword == null)
                throw new ArgumentNullException(nameof(hashedPassword));

            if (providerPassword == null)
                throw new ArgumentNullException(nameof(providerPassword));

            return hashedPassword == Hashes.MD5(providerPassword) ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }
    }
}
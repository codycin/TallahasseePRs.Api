namespace TallahasseePRs.Api.Security
{
    public static class RefreshTokenUtil
    {
        public static string GenerateToken()
        {
            // Generate a secure random token (you can use any method you prefer)
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        public static string HashToken(string token)
        {
            // Hash the token using a secure hashing algorithm (e.g., SHA256)
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hashBytes);
        }


    }
}

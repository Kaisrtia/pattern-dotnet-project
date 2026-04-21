using System.Security.Cryptography;
using System.Text;

namespace pattern_project.Services;

public static class PasswordHasher
{
  public static string Hash(string input)
  {
    var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
    return Convert.ToHexString(hashBytes);
  }
}

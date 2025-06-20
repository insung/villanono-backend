using System.Security.Cryptography;
using System.Text;

public static class IdGenerator
{
    /// <summary>
    /// 주어진 문자열들을 조합하여 결정적이고 고유한 ID를 생성합니다. (SHA-1 사용)
    /// </summary>
    /// <param name="inputs">ID를 생성할 때 사용할 문자열들</param>
    /// <returns>고유한 ID 문자열</returns>
    public static string GenerateDeterministicId(params string[] inputs)
    {
        // 입력 문자열들을 구분자(|)로 합쳐 일관된 하나의 문자열을 만듭니다.
        // Trim()을 사용하여 앞뒤 공백을 제거하여 일관성을 높입니다.
        string uniqueString = string.Join("|", inputs.Select(i => i.Trim()));

        // SHA1 해시 알고리즘을 사용하여 바이트 배열로 변환합니다.
        using (var sha1 = SHA1.Create())
        {
            var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(uniqueString));

            // 해시된 바이트 배열을 16진수 문자열로 변환하여 최종 ID로 사용합니다.
            // (예: "2e4b5b7f...")
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }
}

using System.IO;
using System.Security.Cryptography;

namespace ModThatIsNotMod.Internals
{
    internal static class MelonHashChecking
    {
        private static MD5 md5 = MD5.Create();

        public static string GetMelonHash(string path)
        {
            byte[] bytes = md5.ComputeHash(File.ReadAllBytes(path));
            string finalHash = "";
            foreach (byte b in bytes)
                finalHash += b.ToString("x2");
            return finalHash;
        }
    }
}

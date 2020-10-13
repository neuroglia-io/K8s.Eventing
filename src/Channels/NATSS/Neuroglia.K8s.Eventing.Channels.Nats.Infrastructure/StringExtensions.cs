using System;
using System.Security.Cryptography;
using System.Text;

namespace Neuroglia.K8s.Eventing.Channels.Nats.Infrastructure
{

    /// <summary>
    /// Defines extensions for <see cref="string"/>s
    /// </summary>
    public static class StringExtensions
    {

        internal static readonly char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

        /// <summary>
        /// Generates a random string of the specified length
        /// </summary>
        /// <param name="length">The length of the string to generate</param>
        /// <returns>A new random string of the specified length</returns>
        public static string GenerateRandomString(int length)
        {
            byte[] data = new byte[4 * length];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[idx]);
            }
            return result.ToString();
        }

    }

}

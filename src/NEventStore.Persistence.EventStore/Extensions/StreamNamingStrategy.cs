using System;
using System.Security.Cryptography;
using System.Text;

namespace NEventStore.Persistence.GES.Extensions
{
    public static class HashingExtensions
    {
        private static readonly SHA1 Hasher = SHA1.Create();

        public static string ToHashRepresentation(this string name)
        {
            return Convert.ToBase64String(Hasher.ComputeHash(Encoding.UTF8.GetBytes(name)));
        }
        
    }
}
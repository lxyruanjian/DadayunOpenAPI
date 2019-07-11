using Dadayun.Core.Auth;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Dadayun.Core.Util
{
    public interface ICryptoUtil
    {
        string HMACSign(string data, string key, SigningAlgorithm algorithmName);
        byte[] HMACSignBinary(byte[] data, byte[] key, SigningAlgorithm algorithmName);

        byte[] ComputeMD5Hash(byte[] data);
        byte[] ComputeMD5Hash(Stream steam);
    }

    public static partial class CryptoUtilFactory
    {
        static CryptoUtil util = new CryptoUtil();

        public static ICryptoUtil CryptoInstance
        {
            get { return util; }
        }

        partial class CryptoUtil : ICryptoUtil
        {
            public string HMACSign(string data, string key, SigningAlgorithm algorithmName)
            {
                var binaryData = Encoding.UTF8.GetBytes(data);
                return HMACSign(binaryData, key, algorithmName);
            }

            public string HMACSign(byte[] data, string key, SigningAlgorithm algorithmName)
            {
                if (String.IsNullOrEmpty(key))
                    throw new ArgumentNullException("key", "Please specify a Secret Signing Key.");

                if (data == null || data.Length == 0)
                    throw new ArgumentNullException("data", "Please specify data to sign.");

                if (algorithmName == SigningAlgorithm.HmacSHA1)
                {
                    using (HMACSHA1 mac = new HMACSHA1(Encoding.UTF8.GetBytes(key)))
                    {
                        byte[] hash = mac.ComputeHash(data);
                        return Convert.ToBase64String(hash);
                    }
                }
                else if (algorithmName == SigningAlgorithm.HmacSHA256)
                {
                    using (HMACSHA256 mac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
                    {
                        byte[] hash = mac.ComputeHash(data);
                        return Convert.ToBase64String(hash);
                    }
                }
                else
                {
                    throw new ArgumentException("不合法的参数" + nameof(algorithmName));
                }
            }

            public byte[] ComputeMD5Hash(byte[] data)
            {
                byte[] hashed = MD5.Create().ComputeHash(data);
                return hashed;
            }

            public byte[] ComputeMD5Hash(Stream steam)
            {
                byte[] hashed = MD5.Create().ComputeHash(steam);
                return hashed;
            }

            public byte[] HMACSignBinary(byte[] data, byte[] key, SigningAlgorithm algorithmName)
            {
                if (key == null || key.Length == 0)
                    throw new ArgumentNullException("key", "Please specify a Secret Signing Key.");

                if (data == null || data.Length == 0)
                    throw new ArgumentNullException("data", "Please specify data to sign.");

                if (algorithmName == SigningAlgorithm.HmacSHA1)
                {
                    using (HMACSHA1 mac = new HMACSHA1(key))
                    {
                        byte[] hash = mac.ComputeHash(data);
                        return hash;
                    }
                }
                else if (algorithmName == SigningAlgorithm.HmacSHA256)
                {
                    using (HMACSHA256 mac = new HMACSHA256(key))
                    {
                        byte[] hash = mac.ComputeHash(data);
                        return hash;
                    }
                }
                else
                {
                    throw new ArgumentException("不合法的参数" + nameof(algorithmName));
                }
            }

        }
    }
}

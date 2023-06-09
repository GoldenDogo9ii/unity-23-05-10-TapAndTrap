using System;
using UnityEngine;
// Token: 0x02000002 RID: 2
public class CheckSum
{
	static DES64 d = new DES64();

    public CheckSum()
    {
    }

#if UNITY_EDITOR

    public static string Encrypt(string message, long key)
    {
        string result = "";
        while (message.Length > 8)
        {
            result = result + d.Encrypt(message.Substring(0, 8), LongToByte(key));
            message = message.Substring(8);
        }
        result = result + d.Encrypt(message, LongToByte(key));
        return result;
    }
#endif

    public static string Decrypt(string message, long key)
    {
        string result = "";
        while (message.Length > 16)
        {
            result = result + d.Decrypt(message.Substring(0, 16), LongToByte(key));
            message = message.Substring(16);
        }
        result = result + d.Decrypt(message, LongToByte(key));
        return result;
    }

    static byte[] LongToByte(long _n)
    {
        byte[] result = new byte[8];
        for (int k = 0; k < 8; k++)
        {
            result[k] = (byte)(_n >> ((7 - k) * 8));
        }
        return result;
    }
}


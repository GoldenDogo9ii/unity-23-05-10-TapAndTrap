using System;
using System.Text;
using UnityEngine;

[Serializable]
public sealed class ObscuredString
{
	private ObscuredString(string value)
	{
		this.currentCryptoKey = ObscuredString.cryptoKey;
		this.hiddenValue = value;
		this.fakeValue = null;
		this.inited = true;
	}

	public static void SetNewCryptoKey(string newKey)
	{
		ObscuredString.cryptoKey = newKey;
	}

	public string GetEncrypted()
	{
		return this.hiddenValue;
	}

	public void SetEncrypted(string encrypted)
	{
		this.hiddenValue = encrypted;
		if (ObscuredCheatingDetector.isRunning)
		{
			this.fakeValue = this.InternalDecrypt();
		}
	}

	public static string EncryptDecrypt(string value)
	{
		return ObscuredString.EncryptDecrypt(value, string.Empty);
	}

	public static string EncryptDecrypt(string value, string key)
	{
		if (string.IsNullOrEmpty(value))
		{
			return string.Empty;
		}
		if (string.IsNullOrEmpty(key))
		{
			key = ObscuredString.cryptoKey;
		}
		StringBuilder stringBuilder = new StringBuilder();
		int lengthKey = key.Length;
		int lengthVal = value.Length;
		for (int i = 0; i < lengthVal; i++)
		{
			char cVal = value[i];
			char cKey = key[i % lengthKey];
			char c = (char)(cVal ^ cKey);
			stringBuilder.Append(c);
		}
		return stringBuilder.ToString();
	}

	public static string Encrypt(string value, string key)
	{
		if (string.IsNullOrEmpty(value))
		{
			return string.Empty;
		}
		if (string.IsNullOrEmpty(key))
		{
			key = ObscuredString.cryptoKey;
		}
		StringBuilder stringBuilder = new StringBuilder();
		int lengthKey = key.Length;
		int lengthVal = value.Length;
		for (int i = 0; i < lengthVal; i++)
		{
			char cVal = value[i];
			char cKey = key[i % lengthKey];
			char c = (char)(cVal ^ cKey);
			stringBuilder.Append(((int)c).ToString("X4"));
		}
		return stringBuilder.ToString();
	}

	public static string Decrypt(string value, string key)
	{
		if (string.IsNullOrEmpty(value))
		{
			return string.Empty;
		}
		if (string.IsNullOrEmpty(key))
		{
			key = ObscuredString.cryptoKey;
		}
		StringBuilder stringBuilder = new StringBuilder();
		int lengthKey = key.Length;
		int lengthVal = value.Length;
		for (int i = 0; i < lengthVal; i+=4)
		{

			char cVal = (char) int.Parse(value.Substring(i, 4), System.Globalization.NumberStyles.AllowHexSpecifier);
			char cKey = key[(i/4) % lengthKey];
			char c = (char)(cVal ^ cKey);
			stringBuilder.Append(c);
		}
		return stringBuilder.ToString();
	}

	private string InternalDecrypt()
	{
		if (!this.inited)
		{
			this.currentCryptoKey = ObscuredString.cryptoKey;
			this.hiddenValue = ObscuredString.EncryptDecrypt(string.Empty);
			this.fakeValue = string.Empty;
			this.inited = true;
		}
		string key = ObscuredString.cryptoKey;
		if (this.currentCryptoKey != ObscuredString.cryptoKey)
		{
			key = this.currentCryptoKey;
		}
		string text = ObscuredString.EncryptDecrypt(this.hiddenValue, key);
		if (ObscuredCheatingDetector.isRunning && this.fakeValue != null && text != this.fakeValue)
		{
			ObscuredCheatingDetector.Instance.OnCheatingDetected();
		}
		return text;
	}

	public override string ToString()
	{
		return this.InternalDecrypt();
	}

	public override bool Equals(object obj)
	{
		ObscuredString obscuredString = obj as ObscuredString;
		string b = null;
		if (obscuredString != null)
		{
			b = obscuredString.hiddenValue;
		}
		return string.Equals(this.hiddenValue, b);
	}

	public bool Equals(ObscuredString value)
	{
		string b = null;
		if (value != null)
		{
			b = value.hiddenValue;
		}
		return string.Equals(this.hiddenValue, b);
	}

	public bool Equals(ObscuredString value, StringComparison comparisonType)
	{
		string b = null;
		if (value != null)
		{
			b = value.InternalDecrypt();
		}
		return string.Equals(this.InternalDecrypt(), b, comparisonType);
	}

	public override int GetHashCode()
	{
		return this.InternalDecrypt().GetHashCode();
	}

	public static implicit operator ObscuredString(string value)
	{
		if (value == null)
		{
			return null;
		}
		ObscuredString obscuredString = new ObscuredString(ObscuredString.EncryptDecrypt(value));
		if (ObscuredCheatingDetector.isRunning)
		{
			obscuredString.fakeValue = value;
		}
		return obscuredString;
	}

	public static implicit operator string(ObscuredString value)
	{
		if (value == null)
		{
			return null;
		}
		return value.InternalDecrypt();
	}

	public static bool operator ==(ObscuredString a, ObscuredString b)
	{
		if (object.ReferenceEquals(a, b))
		{
			return true;
		}
		if (a == null || b == null)
		{
			return false;
		}
		string a2 = a.hiddenValue;
		string b2 = b.hiddenValue;
		return string.Equals(a2, b2);
	}

	public static bool operator !=(ObscuredString a, ObscuredString b)
	{
		return !(a == b);
	}

	private static string cryptoKey = "4441";

	[SerializeField]
	private string currentCryptoKey;

	[SerializeField]
	private string hiddenValue;

	private string fakeValue;

	private bool inited;
}
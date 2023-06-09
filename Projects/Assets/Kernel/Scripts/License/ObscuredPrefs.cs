using System;
using System.Text;
using UnityEngine;

public static class ObscuredPrefs
{
	private static string DeviceHash
	{
		get
		{
			if (string.IsNullOrEmpty(ObscuredPrefs.deviceHash))
			{
				ObscuredPrefs.deviceHash = ObscuredPrefs.GetDeviceID();
			}
			return ObscuredPrefs.deviceHash;
		}
	}

	public static void ForceLockToDeviceInit()
	{
		if (string.IsNullOrEmpty(ObscuredPrefs.deviceHash))
		{
			ObscuredPrefs.deviceHash = ObscuredPrefs.GetDeviceID();
		}
		else
		{
			Debug.LogWarning("[ACT] ObscuredPrefs.ForceLockToDeviceInit() is called, but device ID is already obtained!");
		}
	}

	public static void ForceDeviceID(string newDeviceID)
	{
		ObscuredPrefs.deviceHash = newDeviceID;
	}

	public static void SetNewCryptoKey(string newKey)
	{
		ObscuredPrefs.encryptionKey = newKey;
	}

	public static void SetInt(string key, int value)
	{
		ObscuredPrefs.SetStringValue(key, value.ToString());
	}

	public static int GetInt(string key)
	{
		return ObscuredPrefs.GetInt(key, 0);
	}

	public static int GetInt(string key, int defaultValue)
	{
		string key2 = ObscuredPrefs.EncryptKey(key);
		if (!PlayerPrefs.HasKey(key2) && PlayerPrefs.HasKey(key))
		{
			int i = PlayerPrefs.GetInt(key, defaultValue);
			if (!ObscuredPrefs.preservePlayerPrefs)
			{
				ObscuredPrefs.SetInt(key, i);
				PlayerPrefs.DeleteKey(key);
			}
			return i;
		}
		string data = ObscuredPrefs.GetData(key2, defaultValue.ToString());
		int result;
		int.TryParse(data, out result);
		return result;
	}

	public static void SetString(string key, string value)
	{
		ObscuredPrefs.SetStringValue(key, value);
	}

	public static string GetString(string key)
	{
		return ObscuredPrefs.GetString(key, string.Empty);
	}

	public static string GetString(string key, string defaultValue)
	{
		string key2 = ObscuredPrefs.EncryptKey(key);
		if (!PlayerPrefs.HasKey(key2) && PlayerPrefs.HasKey(key))
		{
			string str = PlayerPrefs.GetString(key, defaultValue);
			if (!ObscuredPrefs.preservePlayerPrefs)
			{
				ObscuredPrefs.SetString(key, str);
				PlayerPrefs.DeleteKey(key);
			}
			return str;
		}
		return ObscuredPrefs.GetData(key2, defaultValue);
	}

	public static void SetFloat(string key, float value)
	{
		ObscuredPrefs.SetStringValue(key, value.ToString());
	}

	public static float GetFloat(string key)
	{
		return ObscuredPrefs.GetFloat(key, 0f);
	}

	public static float GetFloat(string key, float defaultValue)
	{
		string key2 = ObscuredPrefs.EncryptKey(key);
		if (!PlayerPrefs.HasKey(key2) && PlayerPrefs.HasKey(key))
		{
			float floatVal = PlayerPrefs.GetFloat(key, defaultValue);
			if (!ObscuredPrefs.preservePlayerPrefs)
			{
				ObscuredPrefs.SetFloat(key, floatVal);
				PlayerPrefs.DeleteKey(key);
			}
			return floatVal;
		}
		string data = ObscuredPrefs.GetData(key2, defaultValue.ToString());
		float result;
		float.TryParse(data, out result);
		return result;
	}

	public static void SetDouble(string key, double value)
	{
		ObscuredPrefs.SetStringValue(key, value.ToString());
	}

	public static double GetDouble(string key)
	{
		return ObscuredPrefs.GetDouble(key, 0.0);
	}

	public static double GetDouble(string key, double defaultValue)
	{
		string data = ObscuredPrefs.GetData(ObscuredPrefs.EncryptKey(key), defaultValue.ToString());
		double result;
		double.TryParse(data, out result);
		return result;
	}

	public static void SetLong(string key, long value)
	{
		ObscuredPrefs.SetStringValue(key, value.ToString());
	}

	public static long GetLong(string key)
	{
		return ObscuredPrefs.GetLong(key, 0L);
	}

	public static long GetLong(string key, long defaultValue)
	{
		string data = ObscuredPrefs.GetData(ObscuredPrefs.EncryptKey(key), defaultValue.ToString());
		long result;
		long.TryParse(data, out result);
		return result;
	}

	public static void SetBool(string key, bool value)
	{
		ObscuredPrefs.SetInt(key, (!value) ? 224 : 721);
	}

	public static bool GetBool(string key)
	{
		return ObscuredPrefs.GetBool(key, false);
	}

	public static bool GetBool(string key, bool defaultValue)
	{
		int num = (!defaultValue) ? 224 : 721;
		string data = ObscuredPrefs.GetData(ObscuredPrefs.EncryptKey(key), num.ToString());
		int num2;
		int.TryParse(data, out num2);
		return num2 == 721;
	}

	public static void SetVector3(string key, Vector3 value)
	{
		string value2 = string.Concat(new object[]
			{
				value.x,
				"|",
				value.y,
				"|",
				value.z
			});
		ObscuredPrefs.SetStringValue(key, value2);
	}

	public static Vector3 GetVector3(string key)
	{
		return ObscuredPrefs.GetVector3(key, Vector3.zero);
	}

	public static Vector3 GetVector3(string key, Vector3 defaultValue)
	{
		string data = ObscuredPrefs.GetData(ObscuredPrefs.EncryptKey(key), "{not_found}");
		Vector3 result;
		if (data == "{not_found}")
		{
			result = defaultValue;
		}
		else
		{
			string[] array = data.Split(new char[]
				{
					'|'
				});
			float x;
			float.TryParse(array[0], out x);
			float y;
			float.TryParse(array[1], out y);
			float z;
			float.TryParse(array[2], out z);
			result = new Vector3(x, y, z);
		}
		return result;
	}

	public static void SetByteArray(string key, byte[] value)
	{
		ObscuredPrefs.SetStringValue(key, Encoding.UTF8.GetString(value, 0, value.Length));
	}

	public static byte[] GetByteArray(string key)
	{
		return ObscuredPrefs.GetByteArray(key, 0, 0);
	}

	public static byte[] GetByteArray(string key, byte defaultValue, int defaultLength)
	{
		string data = ObscuredPrefs.GetData(ObscuredPrefs.EncryptKey(key), "{not_found}");
		byte[] array;
		if (data == "{not_found}")
		{
			array = new byte[defaultLength];
			for (int i = 0; i < defaultLength; i++)
			{
				array[i] = defaultValue;
			}
		}
		else
		{
			array = Encoding.UTF8.GetBytes(data);
		}
		return array;
	}

	public static bool HasKey(string key)
	{
		return PlayerPrefs.HasKey(key) || PlayerPrefs.HasKey(ObscuredPrefs.EncryptKey(key));
	}

	public static void DeleteKey(string key)
	{
		PlayerPrefs.DeleteKey(ObscuredPrefs.EncryptKey(key));
		PlayerPrefs.DeleteKey(key);
	}

	public static void DeleteAll()
	{
		PlayerPrefs.DeleteAll();
	}

	public static void Save()
	{
		PlayerPrefs.Save();
	}

	private static void SetStringValue(string key, string value)
	{
		PlayerPrefs.SetString(ObscuredPrefs.EncryptKey(key), ObscuredPrefs.EncryptValue(value));
	}

	private static string GetData(string key, string defaultValueRaw)
	{
		string text = PlayerPrefs.GetString(key, defaultValueRaw);
		if (text != defaultValueRaw)
		{
			text = ObscuredPrefs.DecryptValue(text);
			if (text == string.Empty)
			{
				text = defaultValueRaw;
			}
		}
		else
		{
			string text2 = ObscuredPrefs.DecryptKey(key);
			string key2 = ObscuredPrefs.EncryptKeyDeprecated(text2);
			text = PlayerPrefs.GetString(key2, defaultValueRaw);
			if (text != defaultValueRaw)
			{
				text = ObscuredPrefs.DecryptValueDeprecated(text);
				PlayerPrefs.DeleteKey(key2);
				ObscuredPrefs.SetStringValue(text2, text);
			}
			else if (PlayerPrefs.HasKey(text2))
			{
				Debug.LogWarning("[ACT] Are you trying to read data saved with regular PlayerPrefs using ObscuredPrefs (key = " + text2 + ")?");
			}
		}
		return text;
	}

	private static string EncryptKey(string key)
	{
		key = ObscuredString.EncryptDecrypt(key, ObscuredPrefs.encryptionKey);
		key = Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
		return key;
	}

	private static string DecryptKey(string key)
	{
		byte[] array = Convert.FromBase64String(key);
		key = Encoding.UTF8.GetString(array, 0, array.Length);
		key = ObscuredString.EncryptDecrypt(key, ObscuredPrefs.encryptionKey);
		return key;
	}

	private static string EncryptValue(string value)
	{
		string text = ObscuredString.EncryptDecrypt(value, ObscuredPrefs.encryptionKey);
		text = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
		text = text + ':' + ObscuredPrefs.CalculateChecksum(text);
		return text;
	}

	private static string DecryptValue(string value)
	{
		string[] array = value.Split(new char[]
			{
				':'
			});
		if (array.Length < 2)
		{
			ObscuredPrefs.SavesTampered();
			return string.Empty;
		}
		string text = array[0];
		string a = array[1];
		byte[] array2;
		try
		{
			array2 = Convert.FromBase64String(text);
		}
		catch
		{
			ObscuredPrefs.SavesTampered();
			return string.Empty;
		}
		string str = Encoding.UTF8.GetString(array2, 0, array2.Length);
		string result = ObscuredString.EncryptDecrypt(str, ObscuredPrefs.encryptionKey);
		if (array.Length == 3)
		{
			if (a != ObscuredPrefs.CalculateChecksum(text + ObscuredPrefs.DeviceHash))
			{
				ObscuredPrefs.SavesTampered();
			}
		}
		else if (array.Length == 2)
		{
			if (a != ObscuredPrefs.CalculateChecksum(text))
			{
				ObscuredPrefs.SavesTampered();
			}
		}
		else
		{
			ObscuredPrefs.SavesTampered();
		}

		return result;
	}

	private static string CalculateChecksum(string input)
	{
		int num = 0;
		byte[] bytes = Encoding.UTF8.GetBytes(input + ObscuredPrefs.encryptionKey);
		int num2 = bytes.Length;
		int num3 = ObscuredPrefs.encryptionKey.Length ^ 64;
		for (int i = 0; i < num2; i++)
		{
			byte b = bytes[i];
			num += (int)b + (int)b * (i + num3) % 3;
		}
		return num.ToString("X2");
	}

	private static void SavesTampered()
	{
		if (ObscuredPrefs.onAlterationDetected != null && !ObscuredPrefs.savesAlterationReported)
		{
			ObscuredPrefs.savesAlterationReported = true;
			ObscuredPrefs.onAlterationDetected();
		}
	}
	
	private static string GetDeviceID()
	{
		string text = LicenseKey.GetDeviceID("34210458329");
		if (string.IsNullOrEmpty(text))
		{
			text = "MNe4wFV8qh4mmMFQ";
		}
		return ObscuredPrefs.CalculateChecksum(text);
	}

	private static string EncryptKeyDeprecated(string key)
	{
		key = ObscuredString.EncryptDecrypt(key);
		key = Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
		return key;
	}

	private static string DecryptValueDeprecated(string value)
	{
		byte[] array = Convert.FromBase64String(value);
		value = Encoding.UTF8.GetString(array, 0, array.Length);
		value = ObscuredString.EncryptDecrypt(value, ObscuredPrefs.encryptionKey);
		return value;
	}

	private static string encryptionKey = "e806f6";

	private static bool savesAlterationReported = false;

	public delegate void deleAlterationDetected();
	public static deleAlterationDetected onAlterationDetected;

	private static string deviceHash;

	public static bool preservePlayerPrefs = false;
}
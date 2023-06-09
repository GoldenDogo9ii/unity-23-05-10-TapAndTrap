using UnityEngine;
using System;
using System.Text;
using System.Collections;

public static class LicenseKey
{
	public static long sign = 1638912;

	static int productId = 128;		
	static bool inited = false;
#if !UNITY_EDITOR
	static private AndroidJavaClass mHelper = null;
	static private AndroidJavaClass mActivityClass = null;
	static private AndroidJavaObject mActivity = null;
#endif

	private static long stringCS = 3213244;

	public static bool Init()
	{
		if (inited)
			return true;

#if UNITY_EDITOR
		sign = 35880711134;
#else
		GameObject go = GameObject.FindGameObjectWithTag("stringCS");
		if (go == null)
			return false;

		stringCS = long.Parse(go.name);

		AndroidJavaClass activityClass = new AndroidJavaClass(CheckSum.Decrypt("4D48E3091637EE0FC8AF4A1E510475C07DCA112DC36A700A3ECBAF8261D02AD1", stringCS));
		AndroidJavaObject activity= activityClass.GetStatic<AndroidJavaObject>(CheckSum.Decrypt("CDCD04C64CCE8B42D1F3D989CEFE9ABB", stringCS));
		AndroidJavaObject pkgMgr= activity.Call<AndroidJavaObject>(CheckSum.Decrypt("B75292D617AE227312448AA618F2ABF7AEB8BCF69A962F65", stringCS));
		string sPkgName = activity.Call<string>(CheckSum.Decrypt("B75292D617AE2273C0C931E1E9F79FFC", stringCS));
		AndroidJavaObject pkgInfo = pkgMgr.Call<AndroidJavaObject>(CheckSum.Decrypt("B75292D617AE22736405EA71270847A3", stringCS), sPkgName, 64);
		AndroidJavaObject[] sigs = pkgInfo.Get<AndroidJavaObject[]>(CheckSum.Decrypt("631681F7D3C835BB634446BA06535D12", stringCS));
		string s = sigs[0].Call<string>(CheckSum.Decrypt("A20AA3F93B1440314E5B9449C8D08BF4", stringCS));
		sign = long.Parse(MakeNumericFormat(s.Substring(21, 12)));
#endif
		inited = true;

		return true;
	}

	public static string GetDeviceID(string defDeviceID)
	{
		string sResult;
        #if UNITY_EDITOR
		    sResult = "0002044073235132";
        #else
        {
	        if(Application.platform == RuntimePlatform.Android)
	        {
		        if(mHelper == null)
		        {
			        mActivityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			        mActivity = mActivityClass.GetStatic<AndroidJavaObject>("currentActivity");
			        if(mActivityClass != null && mActivity != null)
			        {
				        Debug.Log("get activity ok");
			        }

			        mHelper = new AndroidJavaClass("com.cr.biling.DeviceId");
		        }

                string sID = mHelper.CallStatic<string>("get", mActivity);
		        if(sID == null || sID == "")
			        sResult = defDeviceID;
		        else
			        sResult = sID;
	        }
	        else
	        {
		        sResult = defDeviceID;
	        }
        }
        #endif

		return MakeNumericFormat(sResult);
	}

	public static string MakeNumericFormat(string _s)
	{
		int[] nNum = new int[_s.Length];
		for (int i = 0; i < _s.Length; i ++)
		{
			int c = (int)(_s[i]);
			if ('A' <= c && c <= 'Z')
				nNum[i] = (c - 'A') % 10;
			else if ('a' <= c && c <= 'z')
				nNum[i] = (c - 'a') % 10;
			else if ('0' <= c && c <= '9')
				nNum[i] = (c - '0') % 10;
			else
				nNum[i] = c % 10;
		}

		string sResult = "";
		for (int i = 0; i < _s.Length; i ++)
		{   //make 0~9 format
			char cc = (char)(nNum[i] + '0');
			sResult += cc.ToString();
		}

		return sResult;
	}

	public static string GetSerialKey(bool _isRand, int _itemId, int _rand, string defDeviceID = "5743892405213543254")
	{
		const int MIN_LEN = 4 + 6 + 2;
		const String sZero = "00000000000000000000";
		string imei		= GetDeviceID(defDeviceID);
		imei			= defDeviceID.Substring(0, Mathf.Max(0, MIN_LEN-imei.Length)) + imei;
		string abcd		= imei.Substring(0, 4);
		string efg		= (productId % 1000).ToString("000");
		string h		= "7";//(get8MonthMultiply() % 3).ToString();
		string i		= (_itemId % 10).ToString("0");
		string jk		= _isRand ? (_rand%100).ToString("00") : imei.Substring(imei.Length-8, 2);
		string lmnopq	= imei.Substring(imei.Length-6);

		string abcdefghijklmnopq = abcd + efg + h + i + jk + lmnopq;
		BigInt r = BigInt.BiPowMod(new BigInt(abcdefghijklmnopq, false), new BigInt("11", false), new BigInt("999999999999932729", false));

		string ABCDEFGHIJKLMNOPQR = r.ToStr();
		ABCDEFGHIJKLMNOPQR = sZero.Substring(0, Mathf.Max(0, 18-ABCDEFGHIJKLMNOPQR.Length)) + ABCDEFGHIJKLMNOPQR;

		string ST = (getCheckSum(ABCDEFGHIJKLMNOPQR) % 100).ToString("00");
		string ABCDEFGHIJKLMNOPQRST = ABCDEFGHIJKLMNOPQR + ST;
		return MakeSeperatedKey(ABCDEFGHIJKLMNOPQRST);
	}

	private static int getCheckSum(string _s)
	{
		int sum = 0;	
		for(int i = 0; i < _s.Length; i += 2)
			sum += int.Parse(_s.Substring(i, Mathf.Min(2, _s.Length - i)));

		return sum;
	}

	private static int get8MonthMultiply()
	{
		DateTime dtStart	= new DateTime(2020,1,1);
		DateTime dtNow		= DateTime.Now;
		return Mathf.Max(0, (dtNow.Year - dtStart.Year) * 12 + dtNow.Month - dtStart.Month) / 8;
	}

	public static string MakeSeperatedKey(string sNum)
	{
		return sNum.Substring(0, 5) + "-" + sNum.Substring(5, 5) + "-" + sNum.Substring(10, 5) + "-" + sNum.Substring(15, 5);
	}
}

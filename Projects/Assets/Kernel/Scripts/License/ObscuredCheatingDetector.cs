using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObscuredCheatingDetector : ActDetectorBase
{
	private ObscuredCheatingDetector()
	{
	}

	public static ObscuredCheatingDetector Instance
	{
		get
		{
			if (ObscuredCheatingDetector.instance == null)
			{
				ObscuredCheatingDetector obscuredCheatingDetector = (ObscuredCheatingDetector)UnityEngine.Object.FindObjectOfType(typeof(ObscuredCheatingDetector));
				if (obscuredCheatingDetector == null)
				{
					GameObject gameObject = new GameObject("Obscured Cheating Detector");
					obscuredCheatingDetector = gameObject.AddComponent<ObscuredCheatingDetector>();
				}
				return obscuredCheatingDetector;
			}
			return ObscuredCheatingDetector.instance;
		}
	}

	public static void StartDetection(DetectionDelegate callback)
	{
		ObscuredCheatingDetector.Instance.StartDetectionInternal(callback);
	}

	public static void StopDetection()
	{
		ObscuredCheatingDetector.Instance.StopDetectionInternal();
	}

	public static void Dispose()
	{
		ObscuredCheatingDetector.Instance.DisposeInternal();
	}

	private void Awake()
	{
		if (this.Init(ObscuredCheatingDetector.instance, "Obscured Cheating Detector"))
		{
			ObscuredCheatingDetector.instance = this;
		}
	}

	private void StartDetectionInternal(DetectionDelegate callback)
	{
		if (ObscuredCheatingDetector.isRunning)
		{
			Debug.LogWarning("[ACT] Obscured Cheating Detector already running!");
			return;
		}
		this.onDetection = callback;
		ObscuredCheatingDetector.isRunning = true;
	}

	protected override void StopDetectionInternal()
	{
		if (ObscuredCheatingDetector.isRunning)
		{
			this.onDetection = null;
			ObscuredCheatingDetector.isRunning = false;
		}
	}

	protected override void DisposeInternal()
	{
		base.DisposeInternal();
		ObscuredCheatingDetector.instance = null;
	}

	internal void OnCheatingDetected()
	{
		if (this.onDetection != null)
		{
			this.onDetection();
			if (this.autoDispose)
			{
				ObscuredCheatingDetector.Dispose();
			}
			else
			{
				ObscuredCheatingDetector.StopDetection();
			}
		}
	}

	private const string COMPONENT_NAME = "Obscured Cheating Detector";

	internal static bool isRunning;

	private static ObscuredCheatingDetector instance;
}
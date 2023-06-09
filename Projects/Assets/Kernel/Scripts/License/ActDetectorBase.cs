using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActDetectorBase : MonoBehaviour
{
	protected virtual bool IsPlacedCorrectly(string componentName)
	{
		return base.name == componentName && base.GetComponentsInChildren<Component>().Length == 2 && base.transform.childCount == 0;
	}

	protected virtual bool Init(ActDetectorBase instance, string detectorName)
	{
		if (instance != null)
		{
			Debug.LogWarning("[ACT] Only one " + detectorName + " instance allowed!");
			UnityEngine.Object.Destroy(base.gameObject);
			return false;
		}
		if (!this.IsPlacedCorrectly(detectorName))
		{
			Debug.LogWarning(string.Concat(new string[]
				{
					"[ACT] ",
					detectorName,
					" is placed in scene incorrectly and will be auto-destroyed!\nPlease, use \"",
					"GameObject/Create Other/Code Stage/Anti-Cheat Toolkit/".Replace("/", "->"),
					detectorName,
					"\" menu to correct this!"
				}));
			UnityEngine.Object.Destroy(base.gameObject);
			return false;
		}
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		return true;
	}

	private void OnDisable()
	{
		this.StopDetectionInternal();
	}

	private void OnApplicationQuit()
	{
		this.DisposeInternal();
	}

	protected abstract void StopDetectionInternal();

	protected virtual void DisposeInternal()
	{
		this.StopDetectionInternal();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	protected const string MENU_PATH = "GameObject/Create Other/Code Stage/Anti-Cheat Toolkit/";

	public bool autoDispose = true;

	public bool keepAlive = true;

	public delegate void DetectionDelegate();
	protected DetectionDelegate onDetection;
}
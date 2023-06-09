using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIExpander : MonoBehaviour
{
	private	bool mExpanded = false;
	public	bool expanded {get {return mExpanded;} }
	private	bool mInited = false;
	void init()
	{
		gameObject.GetComponentInParent<UIExpanderGrp>().AddExpander(this);
	}

	void Start()
	{

	}

	void Update()
	{
		if (!mInited)
		{
			init();
			mInited = false;
		}
	}

	public void DoClick()
	{
		Cmm.SendMessageRecursively(gameObject, !mExpanded ? "PlayForward" : "PlayReverse", 2);
		mExpanded = !mExpanded;
	}

	void OnClick()
	{
		DoClick();
		gameObject.GetComponentInParent<UIExpanderGrp>().OnClickChild(this);
	}
}

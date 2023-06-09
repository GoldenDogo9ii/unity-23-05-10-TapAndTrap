using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIScrollView))]

public class UIExpanderGrp : MonoBehaviour
{
	protected List<UIExpander> mList = new List<UIExpander>();
	protected UIExpander mExpandedObj = null;

	public void AddExpander(UIExpander _child)
	{
		mList.Add(_child);
	}

	public void OnClickChild(UIExpander _obj)
	{
		if (mExpandedObj == _obj)
		{
			mExpandedObj = null;
		}
		else
		{
			if(mExpandedObj != null)
				mExpandedObj.DoClick();
			mExpandedObj = _obj;
		}

		StartCoroutine(updatePosAndScrollCoroutine());
	}
	IEnumerator updatePosAndScrollCoroutine()
	{
		yield return new WaitForSeconds(0.05f);
		//GetComponent<UIScrollView>().UpdatePosition();
		GetComponent<UIScrollView>().UpdateScrollbars();
		yield break;
	}
}

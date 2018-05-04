using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Language Selection"), RequireComponent(typeof(UIPopupList))]
public class LanguageSelection : MonoBehaviour
{
	private UIPopupList mList;

	private bool mStarted;

	private void Awake()
	{
		this.mList = base.GetComponent<UIPopupList>();
	}

	private void Start()
	{
		this.mStarted = true;
		this.Refresh();
		EventDelegate.Add(this.mList.onChange, delegate
		{
			Localization.language = UIPopupList.current.value;
		});
	}

	private void OnEnable()
	{
		if (this.mStarted)
		{
			this.Refresh();
		}
	}

	public void Refresh()
	{
		if (this.mList != null && Localization.knownLanguages != null)
		{
			this.mList.Clear();
			int i = 0;
			int num = Localization.knownLanguages.Length;
			while (i < num)
			{
				this.mList.items.Add(Localization.knownLanguages[i]);
				i++;
			}
			this.mList.value = Localization.language;
		}
	}

	private void OnLocalize()
	{
		this.Refresh();
	}
}

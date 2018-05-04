using System;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(UIWidget))]
public class AnimatedColor : MonoBehaviour
{
	public Color color = Color.white;

	private UIWidget mWidget;

	private void OnEnable()
	{
		this.mWidget = base.GetComponent<UIWidget>();
		if (this.mWidget == null)
		{
			Transform transform = base.gameObject.transform;
			string text = string.Empty;
			while (transform != null)
			{
				text = transform.gameObject.name + "." + text;
				transform = transform.parent;
			}
			Debug.LogError("STARTS-21491 AnimatedColor mWidget null: " + text);
		}
		this.LateUpdate();
	}

	private void LateUpdate()
	{
		if (this.mWidget == null)
		{
			this.mWidget = base.GetComponent<UIWidget>();
		}
		if (this.mWidget != null)
		{
			this.mWidget.color = this.color;
		}
	}
}

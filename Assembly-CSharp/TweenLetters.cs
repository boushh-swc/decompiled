using System;
using System.Collections.Generic;
using UnityEngine;

public class TweenLetters : UITweener
{
	public enum AnimationLetterOrder
	{
		Forward = 0,
		Reverse = 1,
		Random = 2
	}

	private class LetterProperties
	{
		public float start;

		public float duration;

		public Vector2 offset;
	}

	[Serializable]
	public class AnimationProperties
	{
		public TweenLetters.AnimationLetterOrder animationOrder = TweenLetters.AnimationLetterOrder.Random;

		[Range(0f, 1f)]
		public float overlap = 0.5f;

		public bool randomDurations;

		[MinMaxRange(0f, 1f)]
		public Vector2 randomness = new Vector2(0.25f, 0.75f);

		public Vector2 offsetRange = Vector2.zero;

		public Vector3 pos = Vector3.zero;

		public Vector3 rot = Vector3.zero;

		public Vector3 scale = Vector3.one;

		public float alpha = 1f;
	}

	public TweenLetters.AnimationProperties hoverOver;

	public TweenLetters.AnimationProperties hoverOut;

	private UILabel mLabel;

	private int mVertexCount = -1;

	private int[] mLetterOrder;

	private TweenLetters.LetterProperties[] mLetter;

	private TweenLetters.AnimationProperties mCurrent;

	private void OnEnable()
	{
		this.mVertexCount = -1;
		UILabel expr_0D = this.mLabel;
		expr_0D.onPostFill = (UIWidget.OnPostFillCallback)Delegate.Combine(expr_0D.onPostFill, new UIWidget.OnPostFillCallback(this.OnPostFill));
	}

	private void OnDisable()
	{
		UILabel expr_06 = this.mLabel;
		expr_06.onPostFill = (UIWidget.OnPostFillCallback)Delegate.Remove(expr_06.onPostFill, new UIWidget.OnPostFillCallback(this.OnPostFill));
	}

	private void Awake()
	{
		this.mLabel = base.GetComponent<UILabel>();
		this.mCurrent = this.hoverOver;
	}

	public override void Play(bool forward)
	{
		this.mCurrent = ((!forward) ? this.hoverOut : this.hoverOver);
		base.Play(forward);
	}

	private void OnPostFill(UIWidget widget, int bufferOffset, List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
		if (verts == null)
		{
			return;
		}
		int count = verts.Count;
		if (verts == null || count == 0)
		{
			return;
		}
		if (this.mLabel == null)
		{
			return;
		}
		try
		{
			int quadsPerCharacter = this.mLabel.quadsPerCharacter;
			int num = count / quadsPerCharacter / 4;
			string printedText = this.mLabel.printedText;
			if (this.mVertexCount != count)
			{
				this.mVertexCount = count;
				this.SetLetterOrder(num);
				this.GetLetterDuration(num);
			}
			Matrix4x4 identity = Matrix4x4.identity;
			Vector3 pos = Vector3.zero;
			Quaternion q = Quaternion.identity;
			Vector3 s = Vector3.one;
			Vector3 b = Vector3.zero;
			Quaternion a = Quaternion.Euler(this.mCurrent.rot);
			Vector3 vector = Vector3.zero;
			Color value = Color.clear;
			float num2 = base.tweenFactor * this.duration;
			for (int i = 0; i < quadsPerCharacter; i++)
			{
				for (int j = 0; j < num; j++)
				{
					int num3 = this.mLetterOrder[j];
					int num4 = i * num * 4 + num3 * 4;
					if (num4 < count)
					{
						float start = this.mLetter[num3].start;
						float num5 = Mathf.Clamp(num2 - start, 0f, this.mLetter[num3].duration) / this.mLetter[num3].duration;
						num5 = this.animationCurve.Evaluate(num5);
						b = TweenLetters.GetCenter(verts, num4, 4);
						Vector2 offset = this.mLetter[num3].offset;
						pos = Vector3.LerpUnclamped(this.mCurrent.pos + new Vector3(offset.x, offset.y, 0f), Vector3.zero, num5);
						q = Quaternion.SlerpUnclamped(a, Quaternion.identity, num5);
						s = Vector3.LerpUnclamped(this.mCurrent.scale, Vector3.one, num5);
						float num6 = Mathf.LerpUnclamped(this.mCurrent.alpha, 1f, num5);
						identity.SetTRS(pos, q, s);
						for (int k = num4; k < num4 + 4; k++)
						{
							vector = verts[k];
							vector -= b;
							vector = identity.MultiplyPoint3x4(vector);
							vector += b;
							verts[k] = vector;
							value = cols[k];
							value.a *= num6;
							cols[k] = value;
						}
					}
				}
			}
		}
		catch (Exception)
		{
			base.enabled = false;
		}
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.mLabel.MarkAsChanged();
	}

	private void SetLetterOrder(int letterCount)
	{
		if (letterCount == 0)
		{
			this.mLetter = null;
			this.mLetterOrder = null;
			return;
		}
		this.mLetterOrder = new int[letterCount];
		this.mLetter = new TweenLetters.LetterProperties[letterCount];
		for (int i = 0; i < letterCount; i++)
		{
			this.mLetterOrder[i] = ((this.mCurrent.animationOrder != TweenLetters.AnimationLetterOrder.Reverse) ? i : (letterCount - 1 - i));
			int num = this.mLetterOrder[i];
			this.mLetter[num] = new TweenLetters.LetterProperties();
			this.mLetter[num].offset = new Vector2(UnityEngine.Random.Range(-this.mCurrent.offsetRange.x, this.mCurrent.offsetRange.x), UnityEngine.Random.Range(-this.mCurrent.offsetRange.y, this.mCurrent.offsetRange.y));
		}
		if (this.mCurrent.animationOrder == TweenLetters.AnimationLetterOrder.Random)
		{
			System.Random random = new System.Random();
			int j = letterCount;
			while (j > 1)
			{
				int num2 = random.Next(--j + 1);
				int num3 = this.mLetterOrder[num2];
				this.mLetterOrder[num2] = this.mLetterOrder[j];
				this.mLetterOrder[j] = num3;
			}
		}
	}

	private void GetLetterDuration(int letterCount)
	{
		if (this.mCurrent.randomDurations)
		{
			for (int i = 0; i < this.mLetter.Length; i++)
			{
				this.mLetter[i].start = UnityEngine.Random.Range(0f, this.mCurrent.randomness.x * this.duration);
				float num = UnityEngine.Random.Range(this.mCurrent.randomness.y * this.duration, this.duration);
				this.mLetter[i].duration = num - this.mLetter[i].start;
			}
		}
		else
		{
			float num2 = this.duration / (float)letterCount;
			float num3 = 1f - this.mCurrent.overlap;
			float num4 = num2 * (float)letterCount * num3;
			float duration = this.ScaleRange(num2, num4 + num2 * this.mCurrent.overlap, this.duration);
			float num5 = 0f;
			for (int j = 0; j < this.mLetter.Length; j++)
			{
				int num6 = this.mLetterOrder[j];
				this.mLetter[num6].start = num5;
				this.mLetter[num6].duration = duration;
				num5 += this.mLetter[num6].duration * num3;
			}
		}
	}

	private float ScaleRange(float value, float baseMax, float limitMax)
	{
		return limitMax * value / baseMax;
	}

	private static Vector3 GetCenter(List<Vector3> verts, int firstVert, int length)
	{
		Vector3 a = verts[firstVert];
		for (int i = firstVert + 1; i < firstVert + length; i++)
		{
			a += verts[i];
		}
		return a / (float)length;
	}
}

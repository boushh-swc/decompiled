using StaRTS.Main.Views.UX.Elements;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers
{
	public class BackButtonHelper
	{
		public const string BTN_BACK = "BtnBack";

		protected UXButton backBtn;

		protected UXFactory uxFactory;

		public Action BackButtonCallBack;

		protected Stack<List<UXElement>> navigationVisibilityStack;

		public BackButtonHelper(UXFactory uxFactory)
		{
			this.uxFactory = uxFactory;
			this.navigationVisibilityStack = new Stack<List<UXElement>>();
		}

		public void InitWithSingleElementLayer(UXElement element)
		{
			this.Init();
			this.ResetLayers(element, false);
		}

		public void InitWithMultipleElementsLayer(List<UXElement> elements)
		{
			this.Init();
			this.ResetLayers(elements, false);
		}

		private void Init()
		{
			this.backBtn = this.uxFactory.GetElement<UXButton>("BtnBack");
			this.backBtn.OnClicked = new UXButtonClickedDelegate(this.BackButtonClicked);
		}

		public void ResetLayers(UXElement element, bool makeAllExistingElementsInvisible)
		{
			List<UXElement> elements = new List<UXElement>
			{
				element
			};
			this.ResetLayers(elements, makeAllExistingElementsInvisible);
		}

		public void ResetLayers(List<UXElement> elements, bool makeAllExistingElementsInvisible)
		{
			if (makeAllExistingElementsInvisible)
			{
				while (this.navigationVisibilityStack.Count > 0)
				{
					this.SetVisible(this.navigationVisibilityStack.Pop(), false);
				}
			}
			this.SetVisible(elements, true);
			this.navigationVisibilityStack.Push(elements);
			this.EvaluateBackButtonVisiblity();
		}

		public void AddElementToTopLayer(UXElement element)
		{
			element.Visible = true;
			this.navigationVisibilityStack.Peek().Add(element);
		}

		public void RemoveElementFromTopLayer(UXElement element)
		{
			this.navigationVisibilityStack.Peek().Remove(element);
			element.Visible = false;
		}

		public void AddLayer(UXElement element)
		{
			List<UXElement> elements = new List<UXElement>
			{
				element
			};
			this.AddLayer(elements);
		}

		public void AddLayer(List<UXElement> elements)
		{
			this.SetVisible(this.navigationVisibilityStack.Peek(), false);
			this.SetVisible(elements, true);
			this.navigationVisibilityStack.Push(elements);
			this.EvaluateBackButtonVisiblity();
		}

		public void GoBack()
		{
			this.BackButtonClicked(null);
		}

		public bool IsBackButtonEnabled()
		{
			return this.navigationVisibilityStack.Count > 1;
		}

		public UXButton GetBackButton()
		{
			return this.backBtn;
		}

		protected void EvaluateBackButtonVisiblity()
		{
			this.backBtn.Visible = (this.navigationVisibilityStack.Count > 1);
		}

		protected void BackButtonClicked(UXButton btn)
		{
			this.SetVisible(this.navigationVisibilityStack.Pop(), false);
			this.SetVisible(this.navigationVisibilityStack.Peek(), true);
			this.EvaluateBackButtonVisiblity();
			if (this.BackButtonCallBack != null)
			{
				this.BackButtonCallBack();
			}
		}

		private void SetVisible(List<UXElement> elements, bool isVisible)
		{
			for (int i = 0; i < elements.Count; i++)
			{
				elements[i].Visible = isVisible;
			}
		}
	}
}

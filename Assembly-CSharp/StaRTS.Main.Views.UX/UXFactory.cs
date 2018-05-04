using StaRTS.Assets;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Cameras;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX
{
	public class UXFactory : UXElement, IEventObserver
	{
		private const string DEGENERATE_ELEMENT_NAME = "Degenerate";

		private const float WAIT_TILL_RESIZING_DONE = 0.5f;

		private UXFactoryLoadDelegate uxOnSuccess;

		private UXFactoryLoadDelegate uxOnFailure;

		private object uxCookie;

		private Dictionary<string, UXElement> elements;

		private Dictionary<string, int> duplicates;

		private bool isCachedScreen;

		private bool visible;

		private bool hiddenInQueue;

		public override bool Visible
		{
			get
			{
				return this.visible && !this.hiddenInQueue;
			}
			set
			{
				this.visible = value;
				this.HandleVisibilityChange();
			}
		}

		public virtual bool HiddenInQueue
		{
			get
			{
				return this.hiddenInQueue;
			}
			set
			{
				this.hiddenInQueue = value;
				this.HandleVisibilityChange();
			}
		}

		public UXFactory(UXCamera camera) : base(camera, null, null)
		{
			this.uxOnSuccess = null;
			this.uxOnFailure = null;
			this.uxCookie = null;
			this.elements = new Dictionary<string, UXElement>();
			this.duplicates = new Dictionary<string, int>();
			this.isCachedScreen = false;
			this.visible = false;
		}

		protected void Load(ref AssetHandle uxAssetHandle, string uxAssetName, UXFactoryLoadDelegate onSuccess, UXFactoryLoadDelegate onFailure, object cookie)
		{
			this.uxOnSuccess = onSuccess;
			this.uxOnFailure = onFailure;
			this.uxCookie = cookie;
			this.isCachedScreen = (Service.ScreenController != null && Service.ScreenController.LoadCachedScreen(ref uxAssetHandle, uxAssetName, new AssetSuccessDelegate(this.LoadSuccessNoClone), new AssetFailureDelegate(this.LoadFailure), cookie));
			if (!this.isCachedScreen)
			{
				Service.AssetManager.Load(ref uxAssetHandle, uxAssetName, new AssetSuccessDelegate(this.LoadSuccess), new AssetFailureDelegate(this.LoadFailure), null);
			}
		}

		protected void Unload(AssetHandle uxAssetHandle, string uxAssetName)
		{
			if (Service.AssetManager == null)
			{
				return;
			}
			if (!this.isCachedScreen || (Service.ScreenController != null && !Service.ScreenController.UnloadCachedScreen(uxAssetName)))
			{
				Service.AssetManager.Unload(uxAssetHandle);
			}
		}

		public bool IsLoaded()
		{
			return this.root != null;
		}

		private void LoadSuccessNoClone(object asset, object cookie)
		{
			GameObject gameObject = asset as GameObject;
			this.FinishSetup(gameObject);
		}

		private void LoadSuccess(object asset, object cookie)
		{
			GameObject gameObject = Service.AssetManager.CloneGameObject(asset as GameObject);
			this.FinishSetup(gameObject);
		}

		private void FinishSetup(GameObject gameObject)
		{
			base.InternalSetRoot(gameObject);
			this.uxCamera.AttachToMainAnchor(this.root);
			this.CreateElements(this.root);
			Service.EventManager.SendEvent(EventId.AllUXElementsCreated, this);
			this.SetupRootCollider();
			if (this.uxOnSuccess != null)
			{
				this.uxOnSuccess(this.uxCookie);
			}
			this.Visible = this.visible;
			if (Service.ScreenController != null)
			{
				Service.ScreenController.AdjustDepths();
			}
			if (Service.EventManager != null)
			{
				Service.EventManager.SendEvent(EventId.UxFactoryLoadSuccess, this);
			}
		}

		private void LoadFailure(object cookie)
		{
			if (this.uxOnFailure != null)
			{
				this.uxOnFailure(this.uxCookie);
			}
		}

		public virtual void SetupRootCollider()
		{
		}

		private void HandleVisibilityChange()
		{
			bool flag = this.visible && !this.hiddenInQueue;
			base.Visible = flag;
			if (flag)
			{
				this.RefreshView();
			}
		}

		public virtual void RefreshView()
		{
		}

		public bool HasElement<T>(string name) where T : UXElement
		{
			return this.elements.ContainsKey(name);
		}

		public T GetOptionalElement<T>(string name) where T : UXElement
		{
			return (!this.HasElement<T>(name)) ? ((T)((object)null)) : this.GetElement<T>(name);
		}

		public T GetElement<T>(string name) where T : UXElement
		{
			T t = (T)((object)null);
			if (this.duplicates.ContainsKey(name))
			{
				Service.Logger.ErrorFormat("UX bundle {0} contains duplicate elements named {1}", new object[]
				{
					this.root.name,
					name
				});
			}
			if (this.elements.ContainsKey(name))
			{
				t = (this.elements[name] as T);
			}
			if (t == null)
			{
				Service.Logger.ErrorFormat("Could not find {0} named {1} in {2}", new object[]
				{
					typeof(T),
					name,
					(!(this.root == null)) ? this.root.name : "(null)"
				});
				t = this.CreateDegenerateElement<T>(name);
			}
			return t;
		}

		protected UXElement CreateElements(GameObject parent)
		{
			UIButton component = parent.GetComponent<UIButton>();
			UIToggle component2 = parent.GetComponent<UIToggle>();
			UIGrid component3 = parent.GetComponent<UIGrid>();
			UITable component4 = parent.GetComponent<UITable>();
			UISprite component5 = parent.GetComponent<UISprite>();
			UILabel component6 = parent.GetComponent<UILabel>();
			UIInput component7 = parent.GetComponent<UIInput>();
			UISlider component8 = parent.GetComponent<UISlider>();
			UITexture component9 = parent.GetComponent<UITexture>();
			UIPlayTween component10 = parent.GetComponent<UIPlayTween>();
			MeshRenderer component11 = parent.GetComponent<MeshRenderer>();
			UXElement result;
			if (component8 != null)
			{
				result = this.CreateSlider(parent, component8);
			}
			else if (component7 != null)
			{
				result = this.CreateInput(parent, component7);
			}
			else if (component6 != null)
			{
				result = this.CreateLabel(parent, component6);
			}
			else if (component3 != null)
			{
				result = this.CreateGrid(parent, component3);
			}
			else if (component4 != null)
			{
				result = this.CreateTable(parent, component4);
			}
			else if (component5 != null)
			{
				result = this.CreateSprite(parent, component5);
			}
			else if (component2 != null)
			{
				result = this.CreateCheckbox(parent, component2, component, component10);
			}
			else if (component != null)
			{
				result = this.CreateButton(parent, component);
			}
			else if (component9 != null)
			{
				result = this.CreateTexture(parent, component9);
			}
			else if (component11 != null)
			{
				result = this.CreateMeshRenderer(parent, component11);
			}
			else
			{
				result = this.CreateElement(parent);
			}
			int i = 0;
			int childCount = parent.transform.childCount;
			while (i < childCount)
			{
				this.CreateElements(parent.transform.GetChild(i).gameObject);
				i++;
			}
			return result;
		}

		private UXElement CreateElement(GameObject gameObject)
		{
			UXElement uXElement = new UXElement(this.uxCamera, gameObject, null);
			this.AddElement(gameObject.name, uXElement);
			return uXElement;
		}

		private void AddElement(string name, UXElement element)
		{
			if (this.elements.ContainsKey(name))
			{
				this.AddDuplicateName(name);
			}
			else
			{
				this.elements.Add(name, element);
			}
		}

		private T CreateDegenerateElement<T>(string name) where T : UXElement
		{
			T t = (T)((object)null);
			if (this.root == null)
			{
				throw new Exception("Cannot create degenerate UX element with null root");
			}
			GameObject gameObject = new GameObject();
			gameObject.name = string.Format("{0} ({1})", "Degenerate", name);
			gameObject.transform.parent = this.root.transform;
			gameObject.transform.position = Vector3.zero;
			gameObject.layer = this.root.layer;
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(UXElement))
			{
				t = (this.CreateElement(gameObject) as T);
			}
			else if (typeFromHandle == typeof(UXButton))
			{
				t = (this.CreateButton(gameObject, null) as T);
			}
			else if (typeFromHandle == typeof(UXCheckbox))
			{
				t = (this.CreateCheckbox(gameObject, null, null, null) as T);
			}
			else if (typeFromHandle == typeof(UXGrid))
			{
				t = (this.CreateGrid(gameObject, null) as T);
			}
			else if (typeFromHandle == typeof(UXSprite))
			{
				t = (this.CreateSprite(gameObject, null) as T);
			}
			else if (typeFromHandle == typeof(UXInput))
			{
				t = (this.CreateInput(gameObject, null) as T);
			}
			else if (typeFromHandle == typeof(UXLabel))
			{
				t = (this.CreateLabel(gameObject, null) as T);
			}
			else if (typeFromHandle == typeof(UXSlider))
			{
				t = (this.CreateSlider(gameObject, null) as T);
			}
			else if (typeFromHandle == typeof(UXTable))
			{
				t = (this.CreateTable(gameObject, null) as T);
			}
			else if (typeFromHandle == typeof(UXTexture))
			{
				t = (this.CreateTexture(gameObject, null) as T);
			}
			else if (typeFromHandle == typeof(UXMeshRenderer))
			{
				t = (this.CreateMeshRenderer(gameObject, null) as T);
			}
			if (t == null)
			{
				throw new Exception("Could not create degenerate type " + typeFromHandle);
			}
			return t;
		}

		private UXSlider CreateSlider(GameObject gameObject, UISlider nguiSlider)
		{
			UXSliderComponent uXSliderComponent = gameObject.GetComponent<UXSliderComponent>();
			if (uXSliderComponent != null)
			{
				uXSliderComponent.Slider = null;
				UnityEngine.Object.Destroy(uXSliderComponent);
			}
			uXSliderComponent = gameObject.AddComponent<UXSliderComponent>();
			uXSliderComponent.NGUISlider = nguiSlider;
			UXSlider uXSlider = new UXSlider(this.uxCamera, uXSliderComponent);
			uXSliderComponent.Slider = uXSlider;
			uXSlider.Value = uXSliderComponent.Value;
			this.AddElement(gameObject.name, uXSlider);
			return uXSlider;
		}

		private UXLabel CreateLabel(GameObject gameObject, UILabel nguiLabel)
		{
			UXLabelComponent uXLabelComponent = gameObject.GetComponent<UXLabelComponent>();
			if (uXLabelComponent != null)
			{
				uXLabelComponent.Label = null;
				UnityEngine.Object.Destroy(uXLabelComponent);
			}
			uXLabelComponent = gameObject.AddComponent<UXLabelComponent>();
			uXLabelComponent.NGUILabel = nguiLabel;
			UXLabel uXLabel = new UXLabel(this.uxCamera, uXLabelComponent);
			uXLabelComponent.Label = uXLabel;
			string text = uXLabelComponent.Text;
			if (text != null && text.StartsWith("s_"))
			{
				text = Service.Lang.Get(text, new object[0]);
			}
			uXLabel.Text = text;
			this.AddElement(gameObject.name, uXLabel);
			return uXLabel;
		}

		private UXInput CreateInput(GameObject gameObject, UIInput nguiInput)
		{
			UXInputComponent uXInputComponent = gameObject.GetComponent<UXInputComponent>();
			if (uXInputComponent != null)
			{
				uXInputComponent.Input = null;
				UnityEngine.Object.Destroy(uXInputComponent);
			}
			uXInputComponent = gameObject.AddComponent<UXInputComponent>();
			uXInputComponent.NGUIInput = nguiInput;
			UXInput uXInput = new UXInput(this.uxCamera, uXInputComponent);
			uXInputComponent.Input = uXInput;
			this.AddElement(gameObject.name, uXInput);
			return uXInput;
		}

		private UXGrid CreateGrid(GameObject gameObject, UIGrid nguiGrid)
		{
			UXGridComponent uXGridComponent = gameObject.GetComponent<UXGridComponent>();
			if (uXGridComponent != null)
			{
				uXGridComponent.Grid = null;
				UnityEngine.Object.Destroy(uXGridComponent);
			}
			uXGridComponent = gameObject.AddComponent<UXGridComponent>();
			uXGridComponent.NGUIGrid = nguiGrid;
			uXGridComponent.NGUIPanel = gameObject.transform.parent.GetComponent<UIPanel>();
			uXGridComponent.NGUIScrollView = gameObject.transform.parent.GetComponent<UIScrollView>();
			uXGridComponent.NGUICenterOnChild = gameObject.GetComponent<UICenterOnChild>();
			if (uXGridComponent.NGUICenterOnChild == null)
			{
				uXGridComponent.NGUICenterOnChild = gameObject.transform.parent.GetComponent<UICenterOnChild>();
			}
			UXGrid uXGrid = new UXGrid(this, this.uxCamera, uXGridComponent);
			uXGridComponent.Grid = uXGrid;
			this.AddElement(gameObject.name, uXGrid);
			return uXGrid;
		}

		private UXTable CreateTable(GameObject gameObject, UITable nguiTable)
		{
			UXTableComponent uXTableComponent = gameObject.GetComponent<UXTableComponent>();
			if (uXTableComponent != null)
			{
				uXTableComponent.Table = null;
				UnityEngine.Object.Destroy(uXTableComponent);
			}
			uXTableComponent = gameObject.AddComponent<UXTableComponent>();
			uXTableComponent.NGUITable = nguiTable;
			uXTableComponent.NGUIPanel = gameObject.transform.parent.GetComponent<UIPanel>();
			uXTableComponent.NGUIScrollView = gameObject.transform.parent.GetComponent<UIScrollView>();
			uXTableComponent.NGUICenterOnChild = gameObject.GetComponent<UICenterOnChild>();
			if (uXTableComponent.NGUICenterOnChild == null)
			{
				uXTableComponent.NGUICenterOnChild = gameObject.transform.parent.GetComponent<UICenterOnChild>();
			}
			UXTable uXTable = new UXTable(this, this.uxCamera, uXTableComponent);
			uXTableComponent.Table = uXTable;
			this.AddElement(gameObject.name, uXTable);
			return uXTable;
		}

		private UXSprite CreateSprite(GameObject gameObject, UISprite nguiSprite)
		{
			UXSpriteComponent uXSpriteComponent = gameObject.GetComponent<UXSpriteComponent>();
			if (uXSpriteComponent != null)
			{
				uXSpriteComponent.Sprite = null;
				UnityEngine.Object.Destroy(uXSpriteComponent);
			}
			uXSpriteComponent = gameObject.AddComponent<UXSpriteComponent>();
			uXSpriteComponent.NGUISprite = nguiSprite;
			UXSprite uXSprite = new UXSprite(this.uxCamera, uXSpriteComponent);
			uXSpriteComponent.Sprite = uXSprite;
			uXSprite.SpriteName = uXSpriteComponent.SpriteName;
			this.AddElement(gameObject.name, uXSprite);
			return uXSprite;
		}

		private UXCheckbox CreateCheckbox(GameObject gameObject, UIToggle nguiCheckbox, UIButton nguiButton, UIPlayTween nguiTween)
		{
			UXCheckboxComponent uXCheckboxComponent = gameObject.GetComponent<UXCheckboxComponent>();
			if (uXCheckboxComponent != null)
			{
				uXCheckboxComponent.Checkbox = null;
				UnityEngine.Object.Destroy(uXCheckboxComponent);
			}
			uXCheckboxComponent = gameObject.AddComponent<UXCheckboxComponent>();
			uXCheckboxComponent.NGUICheckbox = nguiCheckbox;
			uXCheckboxComponent.NGUIButton = nguiButton;
			uXCheckboxComponent.NGUITween = nguiTween;
			UXCheckbox uXCheckbox = new UXCheckbox(this.uxCamera, uXCheckboxComponent);
			uXCheckboxComponent.Checkbox = uXCheckbox;
			uXCheckbox.Selected = uXCheckboxComponent.Selected;
			uXCheckbox.RadioGroup = uXCheckboxComponent.RadioGroup;
			this.AddElement(gameObject.name, uXCheckbox);
			return uXCheckbox;
		}

		private UXTexture CreateTexture(GameObject gameObject, UITexture nguiTexture)
		{
			UXTextureComponent uXTextureComponent = gameObject.GetComponent<UXTextureComponent>();
			if (uXTextureComponent != null)
			{
				uXTextureComponent.Texture = null;
				UnityEngine.Object.Destroy(uXTextureComponent);
			}
			uXTextureComponent = gameObject.AddComponent<UXTextureComponent>();
			uXTextureComponent.NGUITexture = nguiTexture;
			UXTexture uXTexture = new UXTexture(this.uxCamera, uXTextureComponent);
			uXTextureComponent.Texture = uXTexture;
			this.AddElement(gameObject.name, uXTexture);
			return uXTexture;
		}

		private UXMeshRenderer CreateMeshRenderer(GameObject gameObject, MeshRenderer meshRenderer)
		{
			UXMeshRenderer uXMeshRenderer = new UXMeshRenderer(this.uxCamera, meshRenderer);
			this.AddElement(gameObject.name, uXMeshRenderer);
			return uXMeshRenderer;
		}

		private UXButton CreateButton(GameObject gameObject, UIButton nguiButton)
		{
			UXButtonComponent uXButtonComponent = gameObject.GetComponent<UXButtonComponent>();
			if (uXButtonComponent != null)
			{
				uXButtonComponent.Button = null;
				UnityEngine.Object.Destroy(uXButtonComponent);
			}
			uXButtonComponent = gameObject.AddComponent<UXButtonComponent>();
			uXButtonComponent.NGUIButton = nguiButton;
			UXButton uXButton = new UXButton(this.uxCamera, uXButtonComponent);
			uXButtonComponent.Button = uXButton;
			this.AddElement(gameObject.name, uXButton);
			return uXButton;
		}

		public T CloneElement<T>(UXElement template, string name, GameObject parent) where T : UXElement
		{
			if (template == null || parent == null || name == null || name == string.Empty)
			{
				return (T)((object)null);
			}
			GameObject gameObject = template.CloneRoot(name, parent);
			if (gameObject == null)
			{
				return (T)((object)null);
			}
			return this.CreateElements(gameObject) as T;
		}

		private void RestoreElementsToOrig()
		{
			foreach (UXElement current in this.elements.Values)
			{
				current.Visible = current.OrigVisible;
			}
		}

		public void DestroyFactory()
		{
			this.DestroyElement(this, !this.isCachedScreen);
			if (this.isCachedScreen)
			{
				this.RestoreElementsToOrig();
			}
		}

		public void DestroyElement(UXElement element)
		{
			this.DestroyElement(element, true);
		}

		private void DestroyElement(UXElement element, bool destroyGameObjects)
		{
			if (element != null)
			{
				element.OnDestroyElement();
				GameObject root = element.Root;
				if (root != null)
				{
					this.RemoveElementsRecursively(root, true, true);
					if (destroyGameObjects)
					{
						UnityEngine.Object.Destroy(root);
					}
				}
			}
		}

		private void AddDuplicateName(string name)
		{
			if (this.duplicates.ContainsKey(name))
			{
				Dictionary<string, int> dictionary;
				(dictionary = this.duplicates)[name] = dictionary[name] + 1;
			}
			else
			{
				this.duplicates.Add(name, 1);
			}
		}

		private void RemoveDuplicateNameIfPresent(string name)
		{
			if (this.duplicates.ContainsKey(name))
			{
				int num = this.duplicates[name] - 1;
				if (num == 0)
				{
					this.duplicates.Remove(name);
				}
				else
				{
					this.duplicates[name] = num;
				}
			}
		}

		public void RenameElement(GameObject gameObject, string newName)
		{
			string name = gameObject.name;
			if (name == newName)
			{
				return;
			}
			gameObject.name = newName;
			this.RemoveDuplicateNameIfPresent(name);
			bool flag = this.elements.ContainsKey(newName);
			if (flag)
			{
				this.AddDuplicateName(newName);
			}
			if (this.elements.ContainsKey(name))
			{
				UXElement value = this.elements[name];
				this.elements.Remove(name);
				if (!flag)
				{
					this.elements.Add(newName, value);
				}
			}
		}

		public void RevertToOriginalNameRecursively(GameObject gameObject, string appendedName)
		{
			this.RenameElement(gameObject, UXUtils.FormatNameToOriginalName(gameObject.name, appendedName));
			Transform transform = gameObject.transform;
			int i = 0;
			int childCount = transform.childCount;
			while (i < childCount)
			{
				this.RevertToOriginalNameRecursively(transform.GetChild(i).gameObject, appendedName);
				i++;
			}
		}

		protected void RemoveElementsRecursively(GameObject gameObject, bool isRootObject, bool callOnDestroy)
		{
			string name = gameObject.name;
			if (this.elements.ContainsKey(name))
			{
				UXElement uXElement = this.elements[name];
				uXElement.InternalDestroyComponent();
				if (callOnDestroy && !isRootObject)
				{
					uXElement.OnDestroyElement();
					uXElement.Visible = uXElement.OrigVisible;
				}
				this.elements.Remove(name);
			}
			else
			{
				this.RemoveDuplicateNameIfPresent(name);
			}
			Transform transform = gameObject.transform;
			int i = 0;
			int childCount = transform.childCount;
			while (i < childCount)
			{
				this.RemoveElementsRecursively(transform.GetChild(i).gameObject, false, callOnDestroy);
				i++;
			}
		}

		public virtual EatResponse OnEvent(EventId id, object cookie)
		{
			return EatResponse.NotEaten;
		}
	}
}

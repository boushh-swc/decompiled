using System;
using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class NavigationActionAnalytics : GameAnalytics
	{
		private string _buttonPressed;

		private string _fromLocation;

		private string _toLocation;

		private string _module;

		private int _order = -1;

		private IDictionary<string, object> _custom;

		public string ButtonPressed
		{
			get
			{
				return this._buttonPressed;
			}
		}

		public string FromLocation
		{
			get
			{
				return this._fromLocation;
			}
		}

		public string ToLocation
		{
			get
			{
				return this._toLocation;
			}
		}

		public string Module
		{
			get
			{
				return this._module;
			}
		}

		public int Order
		{
			get
			{
				return this._order;
			}
		}

		public IDictionary<string, object> Custom
		{
			get
			{
				return this._custom;
			}
		}

		public NavigationActionAnalytics(string buttonPressed)
		{
			this.InitNavigationActionAnalytics(buttonPressed, null, null, null, -1, null);
		}

		public NavigationActionAnalytics(string buttonPressed, string fromLocation)
		{
			this.InitNavigationActionAnalytics(buttonPressed, fromLocation, null, null, -1, null);
		}

		public NavigationActionAnalytics(string buttonPressed, string fromLocation, string toLocation)
		{
			this.InitNavigationActionAnalytics(buttonPressed, fromLocation, toLocation, null, -1, null);
		}

		public NavigationActionAnalytics(string buttonPressed, string fromLocation, string toLocation, string module)
		{
			this.InitNavigationActionAnalytics(buttonPressed, fromLocation, toLocation, module, -1, null);
		}

		public NavigationActionAnalytics(string buttonPressed, string fromLocation, string toLocation, string module, int order)
		{
			this.InitNavigationActionAnalytics(buttonPressed, fromLocation, toLocation, module, order, null);
		}

		public NavigationActionAnalytics(string buttonPressed, string fromLocation, string toLocation, string module, int order, IDictionary<string, object> custom)
		{
			this.InitNavigationActionAnalytics(buttonPressed, fromLocation, toLocation, module, order, custom);
		}

		private void InitNavigationActionAnalytics(string buttonPressed, string fromLocation, string toLocation, string module, int order, IDictionary<string, object> custom)
		{
			this._buttonPressed = buttonPressed;
			this._fromLocation = fromLocation;
			this._toLocation = toLocation;
			this._module = module;
			this._order = order;
			if (custom != null)
			{
				this._custom = new Dictionary<string, object>(custom);
			}
		}

		public override Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["button_pressed"] = this._buttonPressed;
			if (!string.IsNullOrEmpty(this._fromLocation))
			{
				dictionary["from_location"] = this._fromLocation;
			}
			if (!string.IsNullOrEmpty(this._toLocation))
			{
				dictionary["to_location"] = this._toLocation;
			}
			if (!string.IsNullOrEmpty(this._module))
			{
				dictionary["module"] = this._module;
			}
			if (this._order > -1)
			{
				dictionary["order"] = this._order;
			}
			if (this._custom != null)
			{
				foreach (KeyValuePair<string, object> current in this._custom)
				{
					dictionary[current.Key] = current.Value;
				}
			}
			return dictionary;
		}

		public override string GetSwrveEvent()
		{
			return "navigation_action";
		}
	}
}

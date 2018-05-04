using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class UIUserNotificationCategory
{
	private const string IDENTIFIER_KEY = "identifier";

	private const string CONTEXT_ACTIONS_KEY = "contextActions";

	public string identifier;

	public List<UIUserNotificationAction> defaultContextActions;

	public List<UIUserNotificationAction> minimalContextActions;

	public Dictionary<string, object> toDict()
	{
		Dictionary<UIUserNotificationActionContext, List<UIUserNotificationAction>> dictionary = new Dictionary<UIUserNotificationActionContext, List<UIUserNotificationAction>>();
		if (0 < this.defaultContextActions.Count)
		{
			dictionary[UIUserNotificationActionContext.UIUserNotificationActionContextDefault] = this.defaultContextActions;
		}
		if (0 < this.minimalContextActions.Count)
		{
			dictionary[UIUserNotificationActionContext.UIUserNotificationActionContextMinimal] = this.minimalContextActions;
		}
		Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
		if (0 < dictionary.Keys.Count)
		{
			dictionary2["identifier"] = this.identifier;
			dictionary2["contextActions"] = dictionary.ToDictionary((KeyValuePair<UIUserNotificationActionContext, List<UIUserNotificationAction>> x) => (int)x.Key, (KeyValuePair<UIUserNotificationActionContext, List<UIUserNotificationAction>> y) => (from a in y.Value
			select a.toDict()).ToList<Dictionary<string, object>>());
		}
		return dictionary2;
	}
}

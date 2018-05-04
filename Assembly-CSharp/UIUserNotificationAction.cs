using System;
using System.Collections.Generic;

[Serializable]
public class UIUserNotificationAction
{
	private const string IDENTIFIER_KEY = "identifier";

	private const string TITLE_KEY = "title";

	private const string BEHAVIOUR_KEY = "behaviour";

	private const string ACTIVATION_MODE_KEY = "activationMode";

	private const string AUTHENTICATION_REQUIRED_KEY = "authenticationRequired";

	private const string DESTRUCTIVE_KEY = "destructive";

	public string identifier;

	public string title;

	public UIUserNotificationActionBehavior behaviour;

	public UIUserNotificationActivationMode activationMode;

	public bool authenticationRequired;

	public bool destructive;

	public Dictionary<string, object> toDict()
	{
		return new Dictionary<string, object>
		{
			{
				"identifier",
				this.identifier
			},
			{
				"title",
				this.title
			},
			{
				"behaviour",
				(int)this.behaviour
			},
			{
				"activationMode",
				(int)this.activationMode
			},
			{
				"authenticationRequired",
				this.authenticationRequired
			},
			{
				"destructive",
				this.destructive
			}
		};
	}
}

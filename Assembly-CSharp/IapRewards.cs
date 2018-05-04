using System;
using System.Collections.Generic;

public class IapRewards
{
	protected Dictionary<string, Dictionary<string, string>> rewards;

	public IapRewards()
	{
		this.rewards = new Dictionary<string, Dictionary<string, string>>();
	}

	public IapRewards(string currencyName, long amount)
	{
		this.rewards = new Dictionary<string, Dictionary<string, string>>();
		this.AddCurrency(currencyName, amount);
	}

	public void AddItem(string resourceName, long quantity)
	{
		this._AddObject(resourceName, quantity, "item");
	}

	public void AddCurrency(string currencyName, long amount)
	{
		this._AddObject(currencyName, amount, "currency");
	}

	public Dictionary<string, Dictionary<string, string>> getRewards()
	{
		return this.rewards;
	}

	protected void _AddObject(string name, long quantity, string type)
	{
		if (!this._CheckArguments(name, quantity, type))
		{
			SwrveLog.LogError("ERROR: IapRewards reward has not been added because it received an illegal argument");
			return;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("amount", quantity.ToString());
		dictionary.Add("type", type);
		this.rewards.Add(name, dictionary);
	}

	protected bool _CheckArguments(string name, long quantity, string type)
	{
		if (string.IsNullOrEmpty(name))
		{
			SwrveLog.LogError("IapRewards illegal argument: reward name cannot be empty");
			return false;
		}
		if (quantity <= 0L)
		{
			SwrveLog.LogError("IapRewards illegal argument: reward amount must be greater than zero");
			return false;
		}
		if (string.IsNullOrEmpty(type))
		{
			SwrveLog.LogError("IapRewards illegal argument: type cannot be empty");
			return false;
		}
		return true;
	}
}

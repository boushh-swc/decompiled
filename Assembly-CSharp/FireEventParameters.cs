using System;
using System.Collections.Generic;

public class FireEventParameters
{
	internal Dictionary<string, object> valuePayload;

	internal string eventName;

	internal Dictionary<FireEventType, string> eventNameList;

	public string checkoutAsGuest
	{
		set
		{
			this.valuePayload.Add("checkout_as_guest", value ?? string.Empty);
		}
	}

	public string contentId
	{
		set
		{
			this.valuePayload.Add("content_id", value ?? string.Empty);
		}
	}

	public string contentType
	{
		set
		{
			this.valuePayload.Add("content_type", value ?? string.Empty);
		}
	}

	public string currency
	{
		set
		{
			this.valuePayload.Add("currency", value ?? string.Empty);
		}
	}

	public string dateString
	{
		set
		{
			if (!this.valuePayload.ContainsKey("now_date"))
			{
				this.valuePayload.Add("now_date", value ?? string.Empty);
			}
		}
	}

	public DateTime date
	{
		set
		{
			if (!this.valuePayload.ContainsKey("now_date"))
			{
				this.valuePayload.Add("now_date", value.ToUniversalTime().ToString() ?? string.Empty);
			}
		}
	}

	public string description
	{
		set
		{
			this.valuePayload.Add("description", value ?? string.Empty);
		}
	}

	public string destination
	{
		set
		{
			this.valuePayload.Add("destination", value ?? string.Empty);
		}
	}

	public float duration
	{
		set
		{
			this.valuePayload.Add("duration", value);
		}
	}

	public string endDateString
	{
		set
		{
			if (!this.valuePayload.ContainsKey("end_date"))
			{
				this.valuePayload.Add("end_date", value ?? string.Empty);
			}
		}
	}

	public DateTime endDate
	{
		set
		{
			if (!this.valuePayload.ContainsKey("end_date"))
			{
				this.valuePayload.Add("end_date", value.ToUniversalTime().ToString() ?? string.Empty);
			}
		}
	}

	public string itemAddedFrom
	{
		set
		{
			this.valuePayload.Add("item_added_from", value ?? string.Empty);
		}
	}

	public string level
	{
		set
		{
			this.valuePayload.Add("level", value ?? string.Empty);
		}
	}

	public float maxRatingValue
	{
		set
		{
			this.valuePayload.Add("max_rating_value", value);
		}
	}

	public string name
	{
		set
		{
			this.valuePayload.Add("name", value ?? string.Empty);
		}
	}

	public string orderId
	{
		set
		{
			this.valuePayload.Add("order_id", value ?? string.Empty);
		}
	}

	public string origin
	{
		set
		{
			this.valuePayload.Add("origin", value ?? string.Empty);
		}
	}

	public float price
	{
		set
		{
			this.valuePayload.Add("price", value);
		}
	}

	public string quantity
	{
		set
		{
			this.valuePayload.Add("quantity", value ?? string.Empty);
		}
	}

	public float ratingValue
	{
		set
		{
			this.valuePayload.Add("rating_value", value);
		}
	}

	public string receiptId
	{
		set
		{
			this.valuePayload.Add("receipt_id", value ?? string.Empty);
		}
	}

	public string referralFrom
	{
		set
		{
			this.valuePayload.Add("referral_from", value ?? string.Empty);
		}
	}

	public string registrationMethod
	{
		set
		{
			this.valuePayload.Add("registration_method", value ?? string.Empty);
		}
	}

	public string results
	{
		set
		{
			this.valuePayload.Add("results", value ?? string.Empty);
		}
	}

	public string score
	{
		set
		{
			this.valuePayload.Add("score", value ?? string.Empty);
		}
	}

	public string searchTerm
	{
		set
		{
			this.valuePayload.Add("search_term", value ?? string.Empty);
		}
	}

	public string startDateString
	{
		set
		{
			if (!this.valuePayload.ContainsKey("start_date"))
			{
				this.valuePayload.Add("start_date", value ?? string.Empty);
			}
		}
	}

	public DateTime startDate
	{
		set
		{
			if (!this.valuePayload.ContainsKey("start_date"))
			{
				this.valuePayload.Add("start_date", value.ToUniversalTime().ToString() ?? string.Empty);
			}
		}
	}

	public string success
	{
		set
		{
			this.valuePayload.Add("success", value ?? string.Empty);
		}
	}

	public string userId
	{
		set
		{
			this.valuePayload.Add("user_id", value ?? string.Empty);
		}
	}

	public string userName
	{
		set
		{
			this.valuePayload.Add("user_name", value ?? string.Empty);
		}
	}

	public string validated
	{
		set
		{
			this.valuePayload.Add("validated", value ?? string.Empty);
		}
	}

	public FireEventParameters(FireEventType fireEventType)
	{
		this.valuePayload = new Dictionary<string, object>();
		this.eventNameList = new Dictionary<FireEventType, string>
		{
			{
				FireEventType.Achievement,
				"Achievement"
			},
			{
				FireEventType.AddToCart,
				"Add to Cart"
			},
			{
				FireEventType.AddToWishList,
				"Add to Wish List"
			},
			{
				FireEventType.CheckoutStart,
				"Checkout Start"
			},
			{
				FireEventType.LevelComplete,
				"Level Complete"
			},
			{
				FireEventType.Purchase,
				"Purchase"
			},
			{
				FireEventType.Rating,
				"Rating"
			},
			{
				FireEventType.RegistrationComplete,
				"Registration Complete"
			},
			{
				FireEventType.Search,
				"Search"
			},
			{
				FireEventType.TutorialComplete,
				"Tutorial Complete"
			},
			{
				FireEventType.View,
				"View"
			}
		};
		if (!this.eventNameList.TryGetValue(fireEventType, out this.eventName))
		{
			this.eventName = string.Empty;
		}
	}
}

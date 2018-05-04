using System;
using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class FailedReceiptAnalytics : GameAnalytics
	{
		private string _productId;

		private string _error;

		public string ProductId
		{
			get
			{
				return this._productId;
			}
		}

		public string Error
		{
			get
			{
				return this._error;
			}
		}

		public string Context
		{
			get
			{
				return "IAP";
			}
		}

		public FailedReceiptAnalytics(string productId, string error)
		{
			this._productId = productId;
			this._error = error;
		}

		public override Dictionary<string, object> Serialize()
		{
			return new Dictionary<string, object>
			{
				{
					"context",
					"IAP"
				},
				{
					"product_id",
					this._productId
				},
				{
					"error",
					this._error
				}
			};
		}

		public override string GetSwrveEvent()
		{
			return "failed_receipt";
		}
	}
}

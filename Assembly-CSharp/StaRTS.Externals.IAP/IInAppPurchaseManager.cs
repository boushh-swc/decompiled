using System;

namespace StaRTS.Externals.IAP
{
	public interface IInAppPurchaseManager
	{
		void Init();

		void GetProducts();

		void Purchase(string productID);

		void Consume(string consumeId);

		void RestorePurchases();

		void OnApplicationResume();
	}
}

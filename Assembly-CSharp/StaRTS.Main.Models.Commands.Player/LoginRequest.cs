using StaRTS.Externals.EnvironmentManager;
using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class LoginRequest : PlayerIdRequest
	{
		public string LocalePreference
		{
			get;
			set;
		}

		public string DeviceToken
		{
			get;
			set;
		}

		public double TimeZoneOffset
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			if (this.LocalePreference != null)
			{
				serializer.AddString("locale", this.LocalePreference);
			}
			if (this.DeviceToken != null)
			{
				serializer.AddString("deviceToken", this.DeviceToken);
			}
			serializer.AddString("deviceType", "a");
			serializer.Add<double>("timeZoneOffset", this.TimeZoneOffset);
			EnvironmentController environmentController = Service.EnvironmentController;
			serializer.AddString("clientVersion", "6.0.0.10394");
			serializer.AddString("model", environmentController.GetModel());
			serializer.AddString("os", environmentController.GetOS());
			serializer.AddString("osVersion", environmentController.GetOSVersion());
			serializer.AddString("platform", environmentController.GetPlatform());
			serializer.AddString("sessionId", Service.ServerAPI.SessionId);
			serializer.AddString("deviceId", environmentController.GetDeviceIDForEvent2());
			serializer.AddString("deviceIdType", environmentController.GetDeviceIdType());
			return serializer.End().ToString();
		}
	}
}

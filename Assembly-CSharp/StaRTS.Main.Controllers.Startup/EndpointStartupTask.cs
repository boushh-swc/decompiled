using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models.Commands;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Startup
{
	public class EndpointStartupTask : StartupTask
	{
		public EndpointStartupTask(float startPercentage) : base(startPercentage)
		{
		}

		public override void Start()
		{
			GetEndpointsCommand getEndpointsCommand = new GetEndpointsCommand(new DefaultRequest());
			getEndpointsCommand.AddSuccessCallback(new AbstractCommand<DefaultRequest, GetEndpointsResponse>.OnSuccessCallback(this.OnCommandComplete));
			getEndpointsCommand.AddFailureCallback(new AbstractCommand<DefaultRequest, GetEndpointsResponse>.OnFailureCallback(this.OnCommandFailure));
			Service.ServerAPI.Async(getEndpointsCommand);
		}

		private void OnCommandComplete(GetEndpointsResponse response, object cookie)
		{
			if (!string.IsNullOrEmpty(response.Event2BiLogging))
			{
				Service.Logger.DebugFormat("Updating Event2.0 endpoint to {0}", new object[]
				{
					response.Event2BiLogging
				});
				Service.BILoggingController.SetBIUrl(response.Event2BiLogging, response.Event2NoProxyBiLogging);
			}
			base.Complete();
		}

		private void OnCommandFailure(uint status, object cookie)
		{
			base.Complete();
		}
	}
}

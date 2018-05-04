using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.Projectors
{
	public class ProjectorForceReloadHelper : IEventObserver
	{
		private GeometryProjector Projector;

		public ProjectorForceReloadHelper(GeometryProjector projector)
		{
			this.Projector = projector;
			Service.EventManager.RegisterObserver(this, EventId.ForceGeometryReload, EventPriority.Default);
		}

		public void Destroy()
		{
			Service.EventManager.UnregisterObserver(this, EventId.ForceGeometryReload);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.ForceGeometryReload)
			{
				return EatResponse.NotEaten;
			}
			if (this.Projector.Renderer != null)
			{
				bool flag = this.Projector.Renderer.DoesRenderTextureNeedReload();
				if (flag)
				{
					this.Projector.Renderer.Render(this.Projector.Config);
				}
			}
			return EatResponse.NotEaten;
		}
	}
}

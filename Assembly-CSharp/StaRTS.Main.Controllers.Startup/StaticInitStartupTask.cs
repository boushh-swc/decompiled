using StaRTS.Assets;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.Startup
{
	public class StaticInitStartupTask : StartupTask
	{
		public StaticInitStartupTask(float startPercentage) : base(startPercentage)
		{
		}

		public override void Start()
		{
			new AppServerEnvironmentController();
			MainController.StaticInit();
			AssetManager assetManager = Service.AssetManager;
			List<AssetHandle> list = new List<AssetHandle>();
			List<string> list2 = new List<string>();
			List<object> list3 = new List<object>();
			list.Add(AssetHandle.Invalid);
			list2.Add("gui_shared");
			list3.Add(null);
			int i = 0;
			int count = list2.Count;
			while (i < count)
			{
				assetManager.RegisterDependencyBundle(list2[i]);
				i++;
			}
			assetManager.MultiLoad(list, list2, null, null, list3, new AssetsCompleteDelegate(this.OnMultiLoadComplete), null);
		}

		private void OnMultiLoadComplete(object cookie)
		{
			base.Complete();
		}
	}
}

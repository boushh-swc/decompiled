using System;

namespace StaRTS.Main.Utils.Events
{
	public enum EventPriority
	{
		ServerAfterOthers = 0,
		MeshCombineAfterOthers = 1,
		AfterDefault = 2,
		Default = 3,
		BeforeDefault = 4,
		Notification = 5
	}
}

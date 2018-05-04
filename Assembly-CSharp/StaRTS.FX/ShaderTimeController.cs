using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.FX
{
	public class ShaderTimeController : IViewFrameTimeObserver
	{
		private const string SHADER_TIME = "_ShaderTime";

		private float shaderClockFloat;

		public ShaderTimeController()
		{
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
		}

		public void OnViewFrameTime(float dt)
		{
			this.shaderClockFloat += 0.01f * dt;
			while (this.shaderClockFloat >= 1f)
			{
				this.shaderClockFloat -= 1f;
			}
			Shader.SetGlobalFloat("_ShaderTime", this.shaderClockFloat);
		}
	}
}

using System;

namespace StaRTS.FX
{
	public class EmitterPoolCookie
	{
		public object Cookie
		{
			get;
			private set;
		}

		public float DelayPostEmitterStop
		{
			get;
			private set;
		}

		public EmitterStopDelegate PostEmitterStop
		{
			get;
			private set;
		}

		public EmitterStopDelegate EmitterStop
		{
			get;
			private set;
		}

		public EmitterPoolCookie(object cookie, EmitterStopDelegate emitterStop, float delayPostEmitterStop, EmitterStopDelegate postEmitterStop)
		{
			this.Cookie = cookie;
			this.EmitterStop = emitterStop;
			this.DelayPostEmitterStop = delayPostEmitterStop;
			this.PostEmitterStop = postEmitterStop;
		}
	}
}

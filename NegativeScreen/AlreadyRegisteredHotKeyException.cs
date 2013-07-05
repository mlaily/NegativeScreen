using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NegativeScreen
{
	class AlreadyRegisteredHotKeyException : Exception
	{

		public HotKey HotKey { get; protected set; }

		public AlreadyRegisteredHotKeyException(HotKey hotkey, Exception innerException = null)
			: base(string.Format("Unable to register \"{0}\", this hot key is already registered.", hotkey), innerException)
		{
			this.HotKey = hotkey;
		}

	}
}

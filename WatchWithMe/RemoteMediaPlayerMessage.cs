using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchWithMe
{
	public class RemoteMediaPlayerMessage
	{
		public enum MessageType : byte
		{
			Connect,
			Sync,
			StateChange,
			Seek,
		}

		private RemoteMediaPlayerMessage()
		{
			
		}

		public static RemoteMediaPlayerMessage Decode(byte[] message)
		{
			
		}

		public byte[] Encode()
		{
			throw new NotImplementedException();
		}
	}
}

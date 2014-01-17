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
			Connect,		//Sent whenever a new client is connected (to all connected clients)
			Sync,			//Syncs playback position
			StateChange,	//State Change (Play, Pause, Stop)
			Seek,
		}

		public MessageType Type { get; private set; }

		private RemoteMediaPlayerMessage(MessageType messageType)
		{
			Type = messageType;
		}

		public static RemoteMediaPlayerMessage Decode(byte[] messageBytes)
		{
			var message = new RemoteMediaPlayerMessage((MessageType) messageBytes[0]);

			return message;
		}

		public byte[] Encode()
		{
			throw new NotImplementedException();
		}
	}
}

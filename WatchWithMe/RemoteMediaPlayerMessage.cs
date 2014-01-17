using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WatchWithMe
{
	public abstract class RemoteMediaPlayerMessage
	{
		public enum MessageType : byte
		{
			Connect,		//Sent whenever a new client is connected (to all connected clients)
			Sync,			//Syncs playback position
			StateChange,	//State Change (Play, Pause, Stop)
			Seek,
		}

		public MessageType Type { get; private set; }

		protected RemoteMediaPlayerMessage(MessageType messageType)
		{
			Type = messageType;
		}

		public static RemoteMediaPlayerMessage Decode(byte[] messageBytes)
		{
			var messageType = (MessageType) messageBytes[0];

			switch (messageType)
			{
				case MessageType.Connect:
					return new ConnectMessage(messageBytes);
					
				default:
					throw new ArgumentException();
			}
		}

		public abstract byte[] Encode();
	}

	public class ConnectMessage : RemoteMediaPlayerMessage
	{		
		public long Length { get; private set; }
		public long Size { get; private set; }

		internal ConnectMessage(byte[] messageBytes) : base(MessageType.Connect)
		{
			using (var messageStream = new MemoryStream(messageBytes))
			using (var reader = new BinaryReader(messageStream))
			{
				var messageType = (MessageType) reader.ReadByte();

				if(messageType != MessageType.Connect)
					throw new ArgumentException();

				Length = reader.ReadInt64();
				Size = reader.ReadInt64();
			}
		}

		public override byte[] Encode()
		{
			using(var memoryStream = new MemoryStream())
			using (var writer = new BinaryWriter(memoryStream))
			{
				writer.Write((byte) Type);
				writer.Write(Length);
				writer.Write(Size);

				return memoryStream.ToArray();
			}
		}
	}
}

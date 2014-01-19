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

				case MessageType.Sync:
					return new SyncMessage(messageBytes);

				case MessageType.StateChange:
					return new StateChangeMessage(messageBytes);

				case MessageType.Seek:
					return new SeekMessage(messageBytes);
					
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

		public ConnectMessage(long length, long size) : base(MessageType.Connect)
		{
			Length = length;
			Size = size;
		}

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

	public class SyncMessage : RemoteMediaPlayerMessage
	{
		public long Position { get; private set; }
		public PlayState State { get; private set; }

		public SyncMessage(long position, PlayState playState) : base(MessageType.Sync)
		{
			Position = position;
			State = playState;
		}

		internal SyncMessage(byte[] messageBytes) : base(MessageType.Sync)
		{
			using (var messageStream = new MemoryStream(messageBytes))
			using (var reader = new BinaryReader(messageStream))
			{
				var messageType = (MessageType) reader.ReadByte();

				if (messageType != MessageType.Sync)
					throw new ArgumentException();

				Position = reader.ReadInt64();
				State = (PlayState)reader.ReadByte();
			}
		}

		public override byte[] Encode()
		{
			using (var memoryStream = new MemoryStream())
			using (var writer = new BinaryWriter(memoryStream))
			{
				writer.Write((byte) Type);

				writer.Write(Position);
				writer.Write((byte) State);

				return memoryStream.ToArray();
			}
		}
	}

	public class StateChangeMessage : RemoteMediaPlayerMessage
	{
		public PlayState State { get; private set; }

		public StateChangeMessage(PlayState playState) : base(MessageType.StateChange)
		{
			State = playState;
		}

		internal StateChangeMessage(byte[] messageBytes) : base(MessageType.StateChange)
		{
			using (var messageStream = new MemoryStream(messageBytes))
			using (var reader = new BinaryReader(messageStream))
			{
				var messageType = (MessageType) reader.ReadByte();

				if (messageType != MessageType.StateChange)
					throw new ArgumentException();

				State = (PlayState) reader.ReadByte();
			}
		}

		public override byte[] Encode()
		{
			using (var memoryStream = new MemoryStream())
			using (var writer = new BinaryWriter(memoryStream))
			{
				writer.Write((byte) Type);

				writer.Write((byte) State);

				return memoryStream.ToArray();
			}
		}
	}

	public class SeekMessage : RemoteMediaPlayerMessage
	{
		public long Position { get; private set; }

		public SeekMessage(long position) : base(MessageType.Seek)
		{
			Position = position;
		}

		internal SeekMessage(byte[] messageBytes) : base(MessageType.Seek)
		{
			using (var messageStream = new MemoryStream(messageBytes))
			using (var reader = new BinaryReader(messageStream))
			{
				var messageType = (MessageType) reader.ReadByte();

				if (messageType != MessageType.Seek)
					throw new ArgumentException();

				Position = reader.ReadInt64();
			}
		}
		
		public override byte[] Encode()
		{
			using (var memoryStream = new MemoryStream())
			using (var writer = new BinaryWriter(memoryStream))
			{
				writer.Write((byte) Type);

				writer.Write(Position);

				return memoryStream.ToArray();
			}
		}
	}
}

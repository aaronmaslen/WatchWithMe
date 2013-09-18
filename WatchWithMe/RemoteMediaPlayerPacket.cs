using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchWithMe
{
	internal struct RemoteMediaPlayerPacket
	{
		public enum PacketType
		{
			Sync,               // Sync event, causes both sides to report current position and state
			Play,
			Pause,
			Stop,
			Update,             // Periodic update
			Acknowledgement,    // Echoes the received packet
			Error,              // Unrecoverable error, contains error message only
		}

		private readonly PacketType _pType;
		private readonly string _errorMessage;

		public PacketType PType
		{
			get { return _pType; }
		}

		public string ErrorMessage
		{
			get { return _errorMessage; }
		}

		public RemoteMediaPlayerPacket(PacketType packetType)
		{
			if(packetType == PacketType.Error) throw new ArgumentException();

			_pType = packetType;
			_errorMessage = null;
		}

		public RemoteMediaPlayerPacket(string errorMessage)
		{
			_pType = PacketType.Error;
			_errorMessage = errorMessage;
		}
	}
}

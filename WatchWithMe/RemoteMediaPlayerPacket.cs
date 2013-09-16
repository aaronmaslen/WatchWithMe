using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchWithMe
{
	struct RemoteMediaPlayerPacket
	{
		public enum PacketType
		{
			Sync,               // Initial sync event, must be first packet recieved after desync
			Play,
			Pause,
			Stop,
			Update,             // Periodic update
			Acknowledgement,    // Echoes the received packet
			Error,              // Unrecoverable error, contains error message only
		}

		
		private IMediaPlayer _mediaPlayer;
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

		public RemoteMediaPlayerPacket(PacketType packetType, IMediaPlayer mediaPlayer)
		{
			if(packetType == PacketType.Error) throw new ArgumentException();

			if(mediaPlayer is IRemoteMediaPlayer && packetType != PacketType.Acknowledgement)
				throw new ArgumentException();

			_pType = packetType;
			_mediaPlayer = mediaPlayer;
			_errorMessage = null;
		}

		public RemoteMediaPlayerPacket(string errorMessage)
		{
			_pType = PacketType.Error;
			_errorMessage = errorMessage;
			_mediaPlayer = null;
		}
	}
}

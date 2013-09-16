using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WatchWithMe
{
	class RemoteMediaPlayer : IRemoteMediaPlayer
	{
		private readonly Socket _socket;

		public RemoteMediaPlayer()
		{
			_socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
		}

		public RemoteMediaPlayer(IPEndPoint localEndPoint) : this()
		{
			_socket.Bind(localEndPoint);
		}

		public void Play()
		{
			throw new NotImplementedException();
		}

		public void Pause()
		{
			throw new NotImplementedException();
		}

		public void Stop()
		{
			throw new NotImplementedException();
		}

		public PlayState State
		{
			get { throw new NotImplementedException(); }
		}

		public TimeSpan Position
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public string FileId
		{
			get { throw new NotImplementedException(); }
		}

		public void OpenConnection(IPEndPoint remoteEndPoint)
		{
			_socket.Connect(remoteEndPoint);
		}
	}
}

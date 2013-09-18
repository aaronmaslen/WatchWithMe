using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WatchWithMe
{
	class RemoteMediaPlayer : IRemoteMediaPlayer, IDisposable
	{
		private readonly TcpListener _tcpListener;
		private readonly TcpClient _tcpClient;

		public RemoteMediaPlayer(IPEndPoint bindEndPoint)
		{
			_tcpListener = new TcpListener(bindEndPoint);
			_tcpClient = new TcpClient();
		}

		public async void Start()
		{
			_tcpListener.Start();

			while(_tcpListener.Server.IsBound)
				AcceptClient(await _tcpListener.AcceptTcpClientAsync());
		}

		private async void AcceptClient(TcpClient client)
		{
			byte[] buffer = new byte[256];
			int i = await client.GetStream().ReadAsync(buffer, 0, buffer.Length);
		}

		public PlayState State { get; private set; }

		public TimeSpan Position { get; private set; }

		public string FileId { get; private set; }

		public void OpenConnection(IPEndPoint remoteEndPoint)
		{
			_tcpClient.Connect(remoteEndPoint);
		}

		public void Dispose()
		{
			_tcpListener.Stop();
			_tcpClient.Close();
		}
	}
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WatchWithMe
{
	class TcpRemoteMediaPlayerServer
	{
		public class Client : MediaPlayer
		{
			private readonly TcpRemoteMediaPlayerServer _server;
			private readonly TcpClient _tcpClient;
			private readonly List<IPEndPoint> _clients;

			public Client(TcpClient client, TcpRemoteMediaPlayerServer server)
			{
				EndPoint = (IPEndPoint)client.Client.RemoteEndPoint;
				_tcpClient = client;
				_clients = new List<IPEndPoint>();
				_server = server;

				Task.Run(() => ReceiveClientData());
			}

			public IPEndPoint EndPoint { get; private set; }
			public IEnumerable<IPEndPoint> ConnectedClients { get { return _clients.AsReadOnly(); } }

			private async void ReceiveClientData()
			{
				while (_tcpClient.Connected)
				{
					//Receive the size of the message. If the size is less than 255 bytes (it should rarely, 
					//if ever, be larger than a handful of bytes) then only one byte is needed, but this allows for
					//arbitrarily-large messages (up to the memory limit of the application, in theory)

					var buf = new byte[1];
					var messageSize = 0;

					do
					{
						await _tcpClient.GetStream().ReadAsync(buf, 0, 1);
						messageSize += buf[0];
					} while (buf[0] == 255);

					//Receive the message. Because TCP doesn't guarantee that messages won't be fragmented or that the messages
					//will be delivered atomically, this method ensures that the entire message (and only that many bytes)
					//are written into the buffer, regardless of how many times the stream is accessed to do so.

					buf = new byte[messageSize];
					var bytesRead = 0;

					while (bytesRead < messageSize)
					{
						bytesRead += await _tcpClient.GetStream().ReadAsync(buf, bytesRead, messageSize - bytesRead);
					}

					await HandleMessage(buf);
				}

				_tcpClient.Close();
				_server.NotifyClientDisconnected(EndPoint);
			}

			private async Task HandleMessage(byte[] messageBytes)
			{
				var message = RemoteMediaPlayerMessage.Decode(messageBytes);

				throw new NotImplementedException();
			}

			public override void Play()
			{
				throw new NotImplementedException();
			}

			public override void Pause()
			{
				throw new NotImplementedException();
			}

			public override void Stop()
			{
				throw new NotImplementedException();
			}

			public override TimeSpan Position
			{
				get { throw new NotImplementedException(); }
				set { throw new NotImplementedException(); }
			}
		}

		private readonly TcpListener _tcpListener;
		private readonly ConcurrentDictionary<IPEndPoint, Client> _clients;

		public IEnumerable<Client> Clients { get { return _clients.Values.ToList().AsReadOnly(); } }

		public TcpRemoteMediaPlayerServer()
		{
			_tcpListener = new TcpListener(IPAddress.Any, 0) {ExclusiveAddressUse = false};
			_clients = new ConcurrentDictionary<IPEndPoint, Client>();

			_tcpListener.Start();

			AcceptClients();
		}

		private async void AcceptClients()
		{
			while (_tcpListener.Active())
			{
				var tcpClient = await _tcpListener.AcceptTcpClientAsync();
				var client = new Client(tcpClient, this);
				if (_clients.TryAdd(client.EndPoint, client))
				{
					
				}
			}
		}

		private void NotifyClientDisconnected(IPEndPoint ep)
		{
			throw new NotImplementedException();
		}
	}

	static class TcpListenerExtensions
	{
		public static bool Active(this TcpListener l)
		{
			return (bool) (typeof (TcpListener)).GetProperty("Active",
				BindingFlags.NonPublic | BindingFlags.Instance)
				.GetValue(l);
		}
	}
}

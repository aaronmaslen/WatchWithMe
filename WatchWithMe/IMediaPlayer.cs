using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WatchWithMe
{
	enum PlayState
	{
		Unknown,
		Playing,
		Paused,
		Stopped,
	};

	interface IMediaPlayer
	{
		void Play();
		void Pause();
		void Stop();

		PlayState State { get; }

		TimeSpan Position { get; set; }

		string FileId { get; }
	}

	interface ILocalMediaPlayer : IMediaPlayer {}

	interface IRemoteMediaPlayer : IMediaPlayer
	{
		void OpenConnection(IPEndPoint remoteEndPoint);
	}
}

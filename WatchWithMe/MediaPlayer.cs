using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appccelerate.EventBroker;
using Appccelerate.EventBroker.Handlers;

namespace WatchWithMe
{
	public enum PlayState : byte
	{
		Unknown,
		Playing,
		Paused,
		Stopped,
	};

	public interface IMediaPlayer
	{
		void Play();

		void Pause();

		void Stop();

		PlayState State { get; }

		event EventHandler StateChanged;

		TimeSpan Position { get; set; }

		event EventHandler SeekEvent;

		long FileSize { get; }
		long FileLength { get; }
	}

	public interface IRemoteMediaPlayer : IMediaPlayer
	{
		void Connect();

		event EventHandler Connected;

		void Sync();

		event EventHandler SyncEvent;
	}

	public abstract class MediaPlayer : IMediaPlayer
	{
		protected class StateChangeEventArgs : EventArgs
		{
			public StateChangeEventArgs(PlayState newState, PlayState oldState)
			{
				NewState = newState;
				OldState = oldState;
			}

			public PlayState NewState { get; private set; }
			public PlayState OldState { get; private set; }
		}

		protected MediaPlayer()
		{
			EventBroker = new EventBroker();

			_state = PlayState.Unknown;
		}

		public readonly EventBroker EventBroker;

		public abstract long FileSize { get; protected set; }
		public abstract long FileLength { get; protected set; }

		public abstract void Play();

		public abstract void Pause();

		public abstract void Stop();

		public abstract TimeSpan Position { get; set; }

		[EventPublication(@"topic://SeekEvent")]
		public event EventHandler SeekEvent;
		protected virtual void OnSeek(object sender, EventArgs e)
		{
			if (SeekEvent != null)
				SeekEvent(sender, e);
		}

		protected void ChangeState(PlayState newState, object sender = null)
		{
			if (newState == State) return;

			State = newState;

			OnStateChange(sender ?? this, new StateChangeEventArgs(newState, State));
		}

		private PlayState _state;
		public virtual PlayState State
		{
			get { return _state; }
			private set { _state = value; }
		}

		[EventPublication(@"topic://StateChangeEvent")]
		public event EventHandler StateChanged;
		protected virtual void OnStateChange(object sender, StateChangeEventArgs e)
		{
			if (StateChanged != null)
				StateChanged(sender, e);
		}
	}

	public abstract class RemoteMediaPlayer : MediaPlayer, IRemoteMediaPlayer
	{
		public abstract void Connect();

		[EventPublication(@"topic://ConnectEvent")]
		public event EventHandler Connected;

		protected virtual void OnConnect(object sender, EventArgs e)
		{
			if (Connected != null)
				Connected(sender, e);
		}

		public abstract void Sync();

		[EventPublication(@"topic://SyncEvent")]
		public event EventHandler SyncEvent;

		protected virtual void OnSync(object sender, EventArgs e)
		{
			if (SyncEvent != null)
				SyncEvent(sender, e);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchWithMe
{
	public enum PlayState
	{
		Unknown,
		Playing,
		Paused,
		Stopped,
	};

	public interface IMediaPlayer
	{
		void Play();

		event EventHandler PlayEvent;

		void Pause();

		event EventHandler PauseEvent;

		void Stop();

		event EventHandler StopEvent;

		PlayState State { get; }

		event EventHandler StateChanged;

		TimeSpan Position { get; set; }

		event EventHandler SeekEvent;

		string FileId { get; }
	}

	public interface IRemoteMediaPlayer : IMediaPlayer
	{
		void Connect();

		event EventHandler Connected;
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
			PlayEvent += (o, e) => State = PlayState.Playing;
			PauseEvent += (o, e) => State = PlayState.Paused;
			StopEvent += (o, e) => State = PlayState.Stopped;

			State = PlayState.Unknown;
		}

		public abstract void Play();
		public event EventHandler PlayEvent;
		protected virtual void OnPlay(object sender, EventArgs e)
		{
			if (PlayEvent != null)
				PlayEvent(sender, e);
		}

		public abstract void Pause();
		public event EventHandler PauseEvent;
		protected virtual void OnPause(object sender, EventArgs e)
		{
			if (PauseEvent != null)
				PauseEvent(sender, e);
		}

		public abstract void Stop();
		public event EventHandler StopEvent;
		protected virtual void OnStop(object sender, EventArgs e)
		{
			if (StopEvent != null)
				StopEvent(sender, e);
		}

		public abstract TimeSpan Position { get; set; }
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

		public virtual PlayState State { get; private set; }

		public event EventHandler StateChanged;
		protected virtual void OnStateChange(object sender, StateChangeEventArgs e)
		{
			if (StateChanged != null)
				StateChanged(sender, e);
		}

		public virtual string FileId { get; protected set; }
	}

	internal abstract class RemoteMediaPlayer : MediaPlayer, IRemoteMediaPlayer
	{
		public abstract void Connect();
		public event EventHandler Connected;

		protected virtual void OnConnect(object sender, EventArgs e)
		{
			if (Connected != null)
				Connected(sender, e);
		}
	}
}

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
		void OnPlay(object sender, EventArgs e);

		void Pause();

		event EventHandler PauseEvent;
		void OnPause(object sender, EventArgs e);

		void Stop();

		event EventHandler StopEvent;
		void OnStop(object sender, EventArgs e);

		PlayState State { get; }

		event EventHandler StateChanged;
		void OnStateChange(object sender, EventArgs e);

		TimeSpan Position { get; set; }

		event EventHandler SeekEvent;
		void OnSeek(object sender, EventArgs e);

		string FileId { get; }
	}

	public class StateChangeEventArgs : EventArgs
	{
		public StateChangeEventArgs(PlayState newState, PlayState oldState)
		{
			NewState = newState;
			OldState = oldState;
		}

		public PlayState NewState { get; private set; }
		public PlayState OldState { get; private set; }
	}

	public abstract class MediaPlayer : IMediaPlayer
	{
		protected MediaPlayer()
		{
			PlayEvent += (o, e) => State = PlayState.Playing;
			PauseEvent += (o, e) => State = PlayState.Paused;
			StopEvent += (o, e) => State = PlayState.Stopped;

			State = PlayState.Unknown;
		}

		public abstract void Play();
		public event EventHandler PlayEvent;
		public virtual void OnPlay(object sender, EventArgs e)
		{
			if (PlayEvent != null)
				PlayEvent(sender, e);
		}

		public abstract void Pause();
		public event EventHandler PauseEvent;
		public virtual void OnPause(object sender, EventArgs e)
		{
			if (PauseEvent != null)
				PauseEvent(sender, e);
		}

		public abstract void Stop();
		public event EventHandler StopEvent;
		public virtual void OnStop(object sender, EventArgs e)
		{
			if (StopEvent != null)
				StopEvent(sender, e);
		}

		public abstract TimeSpan Position { get; set; }
		public event EventHandler SeekEvent;
		public virtual void OnSeek(object sender, EventArgs e)
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
		public virtual void OnStateChange(object sender, EventArgs e)
		{
			if (StateChanged != null)
				StateChanged(sender, e);
		}

		public virtual string FileId { get; protected set; }
	}

	internal abstract class RemoteMediaPlayer : MediaPlayer {}
}

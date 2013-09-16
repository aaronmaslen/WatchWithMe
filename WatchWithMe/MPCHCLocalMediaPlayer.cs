using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WatchWithMe
{
	class MpchcLocalMediaPlayer : ILocalMediaPlayer
	{
		private void SendCommand(MPCAPI_COMMAND command, string p)
		{
			IntPtr lpData = Marshal.StringToCoTaskMemAuto(p);

			COPYDATASTRUCT cds;

			cds.dwData = new UIntPtr((uint) command);
			cds.lpData = lpData;
			cds.cbData = p.Length * Marshal.SystemDefaultCharSize;

			if (Handle == null)
				throw new InvalidOperationException("Unexpected Command: MPC not connected");

			User32Dll.SendMessage((IntPtr) Handle, (uint) WM.COPYDATA, _wHandle, ref cds);
		}

		public void Play()
		{
			SendCommand(MPCAPI_COMMAND.CMD_PLAY, "");
		}

		public void Pause()
		{
			SendCommand(MPCAPI_COMMAND.CMD_PAUSE, "");
		}

		public void Stop()
		{
			SendCommand(MPCAPI_COMMAND.CMD_STOP, "");
		}

		public PlayState State { get; private set; }

		public TimeSpan Position
		{
			get
			{
				TimeSpan? position = null;

				var wh = new EventWaitHandle(false, EventResetMode.ManualReset);

				MpcMessageHandler handler = null;
				handler = m =>
					{
						if (m.dwData == MPCAPI_COMMAND.CMD_CURRENTPOSITION)
						{
							position = TimeSpan.FromSeconds(int.Parse(m.lpData));
						}

						MpcMessageReceived -= handler;

						wh.Set();
					};

				MpcMessageReceived += handler;
				
				SendCommand(MPCAPI_COMMAND.CMD_GETCURRENTPOSITION, "");

				wh.WaitOne();

				if (position == null)
					throw new Exception("Something broke");

				return (TimeSpan) position;
			}
			set
			{
				SendCommand(MPCAPI_COMMAND.CMD_SETPOSITION, value.Seconds
					.ToString(CultureInfo.InvariantCulture));
			}
		}

		public string FileId { get; private set; }
		
		internal delegate void MpcMessageHandler(MpcMessage m);
		internal event MpcMessageHandler MpcMessageReceived;

		internal IntPtr? Handle { get; set; }

		private readonly IntPtr _wHandle;

		public MpchcLocalMediaPlayer(IntPtr? handle, IntPtr wHandle)
		{
			Handle = handle;
			_wHandle = wHandle;
		}

		public MpchcLocalMediaPlayer(IntPtr wHandle) : this(null, wHandle) {}

		internal void MpcMessageReceivedHandler(Message m)
		{
			MpcMessage message;

			try
			{
				 message = new MpcMessage((COPYDATASTRUCT) m.GetLParam(typeof (COPYDATASTRUCT)));
			}
			catch (ArgumentException ae)
			{
				Debug.Print("Argument Exception: " + ae);
				Debug.Print("Probably not a MPC COPYDATASTRUCT");
				return;
			}

			if(message.dwData == MPCAPI_COMMAND.CMD_CONNECT)
				Handle = new IntPtr(int.Parse(message.lpData));

			if(Handle == null)
				throw new Exception("Unexpected message received: MPC not connected.");

			switch (message.dwData)
			{
				case MPCAPI_COMMAND.CMD_PLAYMODE:
					switch (((MPC_PLAYSTATE) int.Parse(message.lpData)))
					{
						case MPC_PLAYSTATE.PS_PLAY:
							State = PlayState.Playing;
							Debug.Print("Playing");
							break;
						case MPC_PLAYSTATE.PS_PAUSE:
							State = PlayState.Paused;
							Debug.Print("Paused");
							break;
						case MPC_PLAYSTATE.PS_STOP:
							State = PlayState.Stopped;
							Debug.Print("Stopped");
							break;
					}
					break;
			
				case MPCAPI_COMMAND.CMD_NOTIFYSEEK:
					Debug.Print("Do something with this");
					break;
			
				case MPCAPI_COMMAND.CMD_NOWPLAYING:
					FileId = Path.GetFileName(message.lpData.Split(new [] {'|'})[3]);
					Debug.Print("File ID: " + (FileId ?? ""));
					break;
			}

			if(MpcMessageReceived != null)
				MpcMessageReceived(message);
		}

		internal class MpcMessage
		{
			// ReSharper disable InconsistentNaming
			public MPCAPI_COMMAND dwData { get; set; }
			public string lpData { get; set; }
			// ReSharper restore InconsistentNaming

			public MpcMessage(COPYDATASTRUCT data)
			{
				try
				{
					dwData = (MPCAPI_COMMAND) data.dwData.ToUInt32();
					lpData = Marshal.PtrToStringUni(data.lpData);
				}
				catch (OverflowException oe)
				{
					throw new ArgumentException("Error decoding message", oe);
				}
				catch (InvalidCastException ice)
				{
					throw new ArgumentException("Error decoding message", ice);
				}

				Debug.Print(dwData.ToString());
				Debug.Print(lpData ?? "NULL");
			}
		}

		internal enum MPCAPI_COMMAND : uint {
			// ==== Commands from MPC to host

			// Send after connection
			// Parameter 1: MPC window handle (command should be sent to this HWnd)
			CMD_CONNECT             = 0x50000000,

			// Send when opening or closing file
			// Parameter 1: current state (see MPC_LOADSTATE enum)
			CMD_STATE               = 0x50000001,

			// Send when playing, pausing or closing file
			// Parameter 1: current play mode (see MPC_PLAYSTATE enum)
			CMD_PLAYMODE            = 0x50000002,

			// Send after opening a new file
			// Parameter 1: title
			// Parameter 2: author
			// Parameter 3: description
			// Parameter 4: complete filename (path included)
			// Parameter 5: duration in seconds
			CMD_NOWPLAYING          = 0x50000003,

			// List of subtitle tracks
			// Parameter 1: Subtitle track name 0
			// Parameter 2: Subtitle track name 1
			// ...
			// Parameter n: Active subtitle track, -1 if subtitles are disabled
			//
			// if no subtitle track present, returns -1
			// if no file loaded, returns -2
			CMD_LISTSUBTITLETRACKS  = 0x50000004,

			// List of audio tracks
			// Parameter 1: Audio track name 0
			// Parameter 2: Audio track name 1
			// ...
			// Parameter n: Active audio track
			//
			// if no audio track is present, returns -1
			// if no file is loaded, returns -2
			CMD_LISTAUDIOTRACKS     = 0x50000005,

			// Send current playback position in responce
			// of CMD_GETCURRENTPOSITION.
			// Parameter 1: current position in seconds
			CMD_CURRENTPOSITION     = 0x50000007,

			// Send the current playback position after a jump.
			// (Automatically sent after a seek event).
			// Parameter 1: new playback position (in seconds).
			CMD_NOTIFYSEEK          = 0x50000008,

			// Notify the end of current playback
			// (Automatically sent).
			// Parameter 1: none.
			CMD_NOTIFYENDOFSTREAM   = 0x50000009,

			// Send version str
			// Parameter 1: MPC-HC's version
			CMD_VERSION             = 0x5000000A,

			// List of files in the playlist
			// Parameter 1: file path 0
			// Parameter 2: file path 1
			// ...
			// Parameter n: active file, -1 if no active file
			CMD_PLAYLIST            = 0x50000006,

			// Send information about mpc closing
			CMD_DISCONNECT          = 0x5000000B,

			// ==== Commands from host to MPC

			// Open new file
			// Parameter 1: file path
			CMD_OPENFILE            = 0xA0000000,

			// Stop playback, but keep file / playlist
			CMD_STOP                = 0xA0000001,

			// Stop playback and close file / playlist
			CMD_CLOSEFILE           = 0xA0000002,

			// Pause or restart playback
			CMD_PLAYPAUSE           = 0xA0000003,

			// Unpause playback
			CMD_PLAY = 0xA0000004,

			// Pause playback
			CMD_PAUSE = 0xA0000005,

			// Add a new file to playlist (did not start playing)
			// Parameter 1: file path
			CMD_ADDTOPLAYLIST       = 0xA0001000,

			// Remove all files from playlist
			CMD_CLEARPLAYLIST       = 0xA0001001,

			// Start playing playlist
			CMD_STARTPLAYLIST       = 0xA0001002,

			CMD_REMOVEFROMPLAYLIST  = 0xA0001003,   // TODO

			// Cue current file to specific position
			// Parameter 1: new position in seconds
			CMD_SETPOSITION         = 0xA0002000,

			// Set the audio delay
			// Parameter 1: new audio delay in ms
			CMD_SETAUDIODELAY       = 0xA0002001,

			// Set the subtitle delay
			// Parameter 1: new subtitle delay in ms
			CMD_SETSUBTITLEDELAY    = 0xA0002002,

			// Set the active file in the playlist
			// Parameter 1: index of the active file, -1 for no file selected
			// DOESN'T WORK
			CMD_SETINDEXPLAYLIST    = 0xA0002003,

			// Set the audio track
			// Parameter 1: index of the audio track
			CMD_SETAUDIOTRACK       = 0xA0002004,

			// Set the subtitle track
			// Parameter 1: index of the subtitle track, -1 for disabling subtitles
			CMD_SETSUBTITLETRACK    = 0xA0002005,

			// Ask for a list of the subtitles tracks of the file
			// return a CMD_LISTSUBTITLETRACKS
			CMD_GETSUBTITLETRACKS   = 0xA0003000,

			// Ask for the current playback position,
			// see CMD_CURRENTPOSITION.
			// Parameter 1: current position in seconds
			CMD_GETCURRENTPOSITION  = 0xA0003004,

			// Jump forward/backward of N seconds,
			// Parameter 1: seconds (negative values for backward)
			CMD_JUMPOFNSECONDS      = 0xA0003005,

			// Ask slave for version
			CMD_GETVERSION          = 0xA0003006,

			// Ask for a list of the audio tracks of the file
			// return a CMD_LISTAUDIOTRACKS
			CMD_GETAUDIOTRACKS      = 0xA0003001,

			// Ask for the properties of the current loaded file
			// return a CMD_NOWPLAYING
			CMD_GETNOWPLAYING       = 0xA0003002,

			// Ask for the current playlist
			// return a CMD_PLAYLIST
			CMD_GETPLAYLIST         = 0xA0003003,

			// Toggle FullScreen
			CMD_TOGGLEFULLSCREEN    = 0xA0004000,

			// Jump forward(medium)
			CMD_JUMPFORWARDMED      = 0xA0004001,

			// Jump backward(medium)
			CMD_JUMPBACKWARDMED     = 0xA0004002,

			// Increase Volume
			CMD_INCREASEVOLUME      = 0xA0004003,

			// Decrease volume
			CMD_DECREASEVOLUME      = 0xA0004004,

			// Toggle shader
			CMD_SHADER_TOGGLE       = 0xA0004005,

			// Close App
			CMD_CLOSEAPP            = 0xA0004006,

			// Set playing rate
			CMD_SETSPEED            = 0xA0004008,

			// Show host defined OSD message string
			CMD_OSDSHOWMESSAGE      = 0xA0005000

		};

		internal enum MPC_PLAYSTATE
		{
			PS_PLAY = 0,
			PS_PAUSE = 1,
			PS_STOP = 2,
			PS_UNUSED = 3
		};

		internal enum MPC_LOADSTATE
		{
			MLS_CLOSED,
			MLS_LOADING,
			MLS_LOADED,
			MLS_CLOSING
		};

		//private static string ByteToHexBitFiddle(byte[] bytes)
		//{
		//	char[] c = new char[bytes.Length * 2];
		//	int b;
		//	for (int i = 0; i < bytes.Length; i++)
		//	{
		//		b = bytes[i] >> 4;
		//		c[i * 2] = (char)(55 + b + (((b - 10) >> 31) & -7));
		//		b = bytes[i] & 0xF;
		//		c[i * 2 + 1] = (char)(55 + b + (((b - 10) >> 31) & -7));
		//	}
		//	return new string(c);
		//}
	}
}

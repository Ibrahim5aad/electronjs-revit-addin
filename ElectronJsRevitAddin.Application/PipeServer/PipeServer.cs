using ElectronJsRevitAddin.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;

namespace ElectronJsRevitAddin.Pipes
{

	public class PipeServer
	{

		#region Fields

		private NamedPipeServerStream _pipe;
		private PipeMessage _pipeMessage;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="PipeServer"/> class.
		/// </summary>
		/// <param name="pipeName">Name of the pipe.</param>
		public PipeServer(string pipeName = "Pipe.Server")
		{
			PipeName = pipeName;
			IsConnected = false;
			Init();
		}

		#endregion

		#region Events

		public event ConnectEvent OnConnect;

		public event DisconnectEvent OnDisconnect;

		public event MessageEvent MessageReceived;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the name of the pipe.
		/// </summary>
		/// <value>
		/// The name of the pipe.
		/// </value>
		public string PipeName { get; set; }


		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PipeServer"/> is connected.
		/// </summary>
		/// <value>
		///   <c>true</c> if connected; otherwise, <c>false</c>.
		/// </value>
		public bool IsConnected { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Opens the connection.
		/// </summary>
		public void OpenConnection()
		{
			WaitForClientsConnection();
		}


		/// <summary>
		/// Closes the connection.
		/// </summary>
		public void CloseConnection()
		{
			if (_pipe.IsConnected)
			{
				_pipe.WaitForPipeDrain();
				_pipe.Disconnect();
			}

			Init();
			IsConnected = false;
			OnDisconnect?.Invoke(this, EventArgs.Empty);
		}


		/// <summary>
		/// Sends the message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <exception cref="System.Exception">No client connected to pipe.</exception>
		public void SendMessage(string message)
		{
			if (_pipe.IsConnected)
			{
				IsConnected = true;
				var msg = new PipeMessage(message);
				_pipe.BeginWrite(msg.MessageBytes, 0, msg.MessageBytes.Length, (res) => _pipe.EndWrite(res), _pipe);
			}
			else
			{
				IsConnected = false;
				throw new Exception("No client connected to pipe.");
			}
		}


		/// <summary>
		/// Initializes this instance.
		/// </summary>
		private void Init()
		{
			_pipe = new NamedPipeServerStream(PipeName, PipeDirection.InOut, -1, PipeTransmissionMode.Message,
				PipeOptions.Asynchronous | PipeOptions.WriteThrough, PipeMessage.MessageBufferSize,
				PipeMessage.MessageBufferSize);
		}


		/// <summary>
		/// Waits for clients connection.
		/// </summary>
		private void WaitForClientsConnection()
		{
			try
			{
				if (!_pipe.IsConnected)
					_pipe.BeginWaitForConnection(OnClientConnected, _pipe);
			}
			catch (IOException e)
			{
				Debug.WriteLine(e.Message);
				if (e.Message.Contains("Pipe is broken"))
				{
					CloseConnection();
					OpenConnection();
				}
				else
				{
					throw;
				}
			}
		}


		/// <summary>
		/// Called when [client connected].
		/// </summary>
		/// <param name="result">The result.</param>
		private void OnClientConnected(IAsyncResult result)
		{
			try
			{
				_pipe.EndWaitForConnection(result);
				IsConnected = true;
				OnConnect?.Invoke(this, EventArgs.Empty);
				ListenToClients();
			}
			catch (IOException ioe)
			{
				Debug.WriteLine(ioe.Message);
				if (ioe.Message.Contains("No process"))
				{
					CloseConnection();
					OpenConnection();
				}
				else
				{
					throw;
				}
			}
		}


		/// <summary>
		/// Listens to clients connected to this server.
		/// </summary>
		private void ListenToClients()
		{
			_pipeMessage = new PipeMessage();
			try
			{
				_pipe.BeginRead(_pipeMessage.MessageBytes, 0, PipeMessage.MessageBufferSize, OnMessageReceived, _pipe);
			}
			catch (IOException ioe)
			{
				Debug.WriteLine(ioe.Message);
				if (ioe.Message.Contains("Waiting for"))
				{
					CloseConnection();
					OpenConnection();
				}
				else
				{
					throw;
				}
			}
		}


		/// <summary>
		/// Called when [read message].
		/// </summary>
		/// <param name="result">The result.</param>
		private void OnMessageReceived(IAsyncResult result)
		{
			_pipe.EndRead(result);
			if (!_pipeMessage.IsNullOrEmpty())
			{
				var args = new MessageEventArgs { Message = _pipeMessage.Message };
				MessageReceived?.Invoke(this, args);
			}

			if (_pipe.IsConnected)
			{
				ListenToClients();
			}
			else
			{
				CloseConnection();
				OpenConnection();
			}
		}

		#endregion


	}
}

using ElectronJsRevitAddin.TestServer.Utils.EventsArgs;

namespace ElectronJsRevitAddin.TestServer
{

	internal class Program
	{

		private static PipeServer _server;


		/// <summary>
		/// Defines the entry point of the application.
		/// </summary>
		/// <param name="args">The arguments.</param>
		static void Main(string[] args)
		{
			InitServer();
			Console.WriteLine("Server Initialized.. [send 'end' to close]");

			string? msg = "";

			while ((msg = Console.ReadLine()) != "end")
			{
				Console.WriteLine("Message sent to clients: " + msg);
				_server.SendMessage(msg ?? "");
			}

			CloseServer();
			Console.WriteLine("Server Closed..");

		}


		/// <summary>
		/// Initializes the server.
		/// </summary>
		private static void InitServer()
		{
			_server = new PipeServer();
			_server.OnConnect += OnServerConnect;
			_server.OnDisconnect += OnServerOnDisconnect; 
			_server.MessageReceived += OnMessageReceived; 
			_server.OpenConnection();
		}


		/// <summary>
		/// Closes the server.
		/// </summary>
		private static void CloseServer()
		{
			_server.CloseConnection();
			_server.OnConnect -= OnServerConnect;
			_server.OnDisconnect -= OnServerOnDisconnect;
			_server.MessageReceived -= OnMessageReceived;
			_server = null;
		}


		/// <summary>
		/// Called when [server on disconnect].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
		private static void OnServerOnDisconnect(object sender, EventArgs args)
		{
			Console.WriteLine("A Client Disconnected");
		}


		/// <summary>
		/// Called when [server connect].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
		private static void OnServerConnect(object sender, EventArgs args)
		{
			Console.WriteLine("A Client Connected");
		}


		/// <summary>
		/// Called when [message received].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="MessageEventArgs"/> instance containing the event data.</param>
		private static void OnMessageReceived(object sender, MessageEventArgs args)
		{
			if (args != null)
			{
				var message = args.Message;
				Console.WriteLine($"Message received from client: {message}");
			}
		}

	}



}
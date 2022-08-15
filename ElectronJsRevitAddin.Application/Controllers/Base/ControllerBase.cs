using ElectronJsRevitAddin.Controllers.Attributes;
using ElectronJsRevitAddin.Controllers.Base;
using ElectronJsRevitAddin.DI;
using ElectronJsRevitAddin.Pipes;
using ElectronJsRevitAddin.Utils;
using ElectronJsRevitAddin.Utils.Exceptions;
using Newtonsoft.Json.Linq;
using ProtoBuf;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ElectronJsRevitAddin.Controllers
{
	public class ControllerBase
	{

		private static PipeServer _server;


		/// <summary>
		/// Initializes the <see cref="ControllerBase"/> class.
		/// </summary>
		static ControllerBase()
		{
		}


		/// <summary>
		/// Finalizes an instance of the <see cref="ControllerBase"/> class.
		/// </summary>
		~ControllerBase()
		{
			CloseServer();
		}


		/// <summary>
		/// Initializes the server.
		/// </summary>
		public static void InitServer()
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
		public static void CloseServer()
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

		}


		/// <summary>
		/// Called when [server connect].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="EventArgs"/> instance containing the event data.</param>
		private static void OnServerConnect(object sender, EventArgs args)
		{

		}


		/// <summary>
		/// Called when [message received].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="MessageEventArgs"/> instance containing the event data.</param>
		protected static async void OnMessageReceived(object sender, MessageEventArgs args)
		{
			try
			{
				var serializedRequest = args.Message;
				byte[] data = Convert.FromBase64String(serializedRequest);

				Request request = null;
				using (MemoryStream stream = new MemoryStream(data))
				{
					request = Serializer.Deserialize<Request>(stream);
				}

				if (request != null)
				{
					var splits = request.Endpoint.Trim(Environment.NewLine.ToCharArray()).Split('/');
					var controllerName = splits[0].ToLower();
					var endpoint = splits.Length > 1 ? splits[1].ToLower() : String.Empty;
					var payload = JObject.Parse(request.Payload);

					var service = IoC.Default.GetService(controllerName);

					if (service == null)
						throw new NoControllerFoundException(controllerName);

					var endpointInfo = service.GetType().GetMethods()
								.FirstOrDefault(m =>
								{
									var attr = m.GetCustomAttribute(typeof(Endpoint)) as Endpoint;
									if (attr.Path.ToLower() == endpoint)
										return true;
									else return false;
								});

					if (endpointInfo != null)
					{
						var result = endpointInfo.Invoke(service, new object[] { payload });
						if (result is Task task)
							await task;
					}
					else
						throw new NoEndpointFoundException(endpoint);
				}
			}
			catch (Exception)
			{
			}

		}


		/// <summary>
		/// Sends the message to client.
		/// </summary>
		/// <param name="msg">The MSG.</param>
		public void SendMessageToClient(string msg)
		{
			if (_server is null)
				InitServer();
			_server.SendMessage(msg ?? "");
		}
	}

}

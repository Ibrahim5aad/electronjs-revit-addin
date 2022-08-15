using ElectronJsRevitAddin.TestServer.Utils.EventsArgs;

namespace ElectronJsRevitAddin.TestServer.Utils
{

	public delegate void ConnectEvent(object sender, EventArgs args);

	public delegate void DisconnectEvent(object sender, EventArgs args);

	public delegate void MessageEvent(object sender, MessageEventArgs args);

}

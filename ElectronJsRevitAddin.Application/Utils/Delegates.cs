using System;

namespace ElectronJsRevitAddin.Utils
{

	public delegate void ConnectEvent(object sender, EventArgs args);

	public delegate void DisconnectEvent(object sender, EventArgs args);

	public delegate void MessageEvent(object sender, MessageEventArgs args);

}

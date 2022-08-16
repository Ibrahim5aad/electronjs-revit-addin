using ElectronJsRevitAddin.Application.Services;
using ElectronJsRevitAddin.Application.Utils;
using ElectronJsRevitAddin.Controllers.Attributes;
using ElectronJsRevitAddin.ExternalCommands;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace ElectronJsRevitAddin.Controllers
{
	[Controller("Window")]
	public class WindowController : ControllerBase
	{


		[Endpoint("SetWindowOwner")]
		public async Task SetWindowOwner(JObject payload)
		{
			await Task.Run(() =>
			{
				WindowHandler.SetWindowOwner
					(DocumentManager.Instance.Application,
					new IntPtr(int.Parse(payload["WindowHandle"].ToString())));
			});
		}


		[Endpoint("Close")]
		public async Task Close(JObject payload)
		{
			var isClose = payload["isclose"].ToString();
			if (bool.Parse(isClose))
			{
				MainCommand.UIProcess.Close();
				MainCommand.UIProcess = null;
				ControllerBase.CloseServer();
			}
		}


	}
}

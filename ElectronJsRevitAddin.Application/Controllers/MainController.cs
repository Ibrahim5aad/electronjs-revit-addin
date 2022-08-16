using ElectronJsRevitAddin.Application.ExternalEvents;
using ElectronJsRevitAddin.Application.Services;
using ElectronJsRevitAddin.Application.Utils;
using ElectronJsRevitAddin.Controllers.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace ElectronJsRevitAddin.Controllers
{
	[Controller("Main")]
	public class MainController : ControllerBase
	{

		[Endpoint("ProcessTasks")]
		public async Task ProcessTasks(JObject payload)
		{

			var command = payload["command"].ToString();
			switch (command)
			{
				case "deleteSelected":
					{
						try
						{
							var task = new DeleteSelectedElements();
							await ExternalExecutor
								.ExecuteInRevitContextAsync(task);

							SendMessageToClient($"toaster-msg::{task.DeleteElements?.Count ?? 0} elements had been deleted.");
						}
						catch (System.Exception)
						{
						}
						break;
					}
				default:
					break;
			}

		}
		 

	}
}

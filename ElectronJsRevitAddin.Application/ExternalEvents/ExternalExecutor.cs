using Autodesk.Revit.UI;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ElectronJsRevitAddin.Application.ExternalEvents
{
	/// <summary>
	/// Class ExternalExecutor.
	/// </summary>
	public static class ExternalExecutor
	{

		#region Fields

		private static ExternalEvent _externalEvent;

		#endregion

		#region Methods

		/// <summary>
		/// Creates the external event.
		/// </summary>
		public static void CreateExternalEvent()
		{
			_externalEvent = ExternalEvent.Create(new ExternalEventHandler());
		}


		/// <summary>
		/// Executes the in revit context asynchronous.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <returns>Task.</returns>
		public static Task ExecuteInRevitContextAsync(Action<UIApplication> command)
		{
			var request = new Request(command);
			ExternalEventHandler.Queue.Enqueue(request);
			var result = _externalEvent.Raise();
			return request.Tcs.Task;
		}

		/// <summary>
		/// Executes the in revit context asynchronous.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <returns>Task.</returns>
		public static Task ExecuteInRevitContextAsync(IRevitContextEvent @event)
		{
			var request = new Request(@event);
			ExternalEventHandler.Queue.Enqueue(request);
			var result = _externalEvent.Raise();
			return request.Tcs.Task;
		}

		#endregion

		#region Private Classes

		/// <summary>
		/// Class Request.
		/// </summary>
		private class Request
		{
			public readonly Action<UIApplication> Command;
			public readonly TaskCompletionSource<object> Tcs = new TaskCompletionSource<object>();

			/// <summary>
			/// Initializes a new instance of the <see cref="Request"/> class.
			/// </summary>
			/// <param name="command">The command.</param>
			public Request(Action<UIApplication> command)
			{
				Command = command;
			}


			/// <summary>
			/// Initializes a new instance of the <see cref="Request"/> class.
			/// </summary>
			/// <param name="event">The event.</param>
			public Request(IRevitContextEvent @event)
			{
				Command = @event.Execute;
			}
		}


		/// <summary>
		/// Class ExternalEventHandler.
		/// Implements the <see cref="Autodesk.Revit.UI.IExternalEventHandler" />
		/// </summary>
		/// <seealso cref="Autodesk.Revit.UI.IExternalEventHandler" />
		private class ExternalEventHandler : IExternalEventHandler
		{
			public static readonly ConcurrentQueue<Request> Queue
																		= new ConcurrentQueue<Request>();

			/// <summary>
			/// This method is called to handle the external event.
			/// </summary>
			/// <param name="app">The application.</param>
			/// <since>
			/// 2013
			/// </since>
			public void Execute(UIApplication app)
			{
				while (Queue.TryDequeue(out var request))
					try
					{
						request.Command(app);
						request.Tcs.SetResult(null);
					}
					catch (Exception e)
					{
						request.Tcs.SetException(e);
					}
			}

			/// <summary>
			/// String identification of the event handler.
			/// </summary>
			/// <returns>The event's name</returns>
			/// <since>
			/// 2013
			/// </since>
			public string GetName()
			{
				return "RoomX::ExternalExecutor::ExternalEventHandler";
			}
		}

		#endregion
	}
}

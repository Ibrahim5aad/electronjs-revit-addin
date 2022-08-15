using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ElectronJsRevitAddin.Application.Services;
using System.Collections.Generic;

namespace ElectronJsRevitAddin.Application.ExternalEvents
{
	internal class DeleteSelectedElements : IRevitContextEvent
	{
		public ICollection<ElementId> DeleteElements { get; private set; }

		public void Execute(UIApplication application)
		{
			using (Transaction t = new Transaction(DocumentManager.Instance.CurrentDocument))
			{
				t.Start("Delete Selected Elements");

				var elements = DocumentManager.Instance.CurrentUIDocument.Selection.GetElementIds();

				DeleteElements = DocumentManager.Instance.CurrentDocument.Delete(elements);

				t.Commit();
			}
		}
	}
}

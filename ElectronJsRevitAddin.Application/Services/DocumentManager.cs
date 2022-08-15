using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;


namespace ElectronJsRevitAddin.Application.Services
{
    /// <summary>
    /// Class DocumentManager.
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class DocumentManager : IDisposable
    {

        #region Fields

        private Document _doc;
        private UIDocument _uidoc;
        private static DocumentManager _instance;
        private UIApplication _uiapp;

        #endregion

        #region Constructor

        /// <summary>
        /// Prevents a default instance of the <see cref="DocumentManager"/> class from being created.
        /// </summary>
        private DocumentManager()
        {
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        /// <value>The instance.</value>
        public static DocumentManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DocumentManager();
                return _instance;
            }
        }


        /// <summary>
        /// Gets or sets the current document.
        /// </summary>
        /// <value>The current document.</value>
        public Document CurrentDocument
        {
            get { return _doc; }
            set
            {
                _doc = value;
            }
        }


        /// <summary>
        /// Gets the active view.
        /// </summary>
        /// <value>The active view.</value>
        public View ActiveView => CurrentDocument.ActiveView;


        /// <summary>
        /// Gets or sets the current UI document.
        /// </summary>
        /// <value>The current UI document.</value>
        public UIDocument CurrentUIDocument
        {
            get { return _uidoc; }
            set
            {
                _uidoc = value;
            }
        }


        /// <summary>
        /// Gets or sets the application.
        /// </summary>
        /// <value>The application.</value>
        public UIApplication Application
        {
            get { return _uiapp; }
            set
            {
                _uiapp = value;
            }
        }

        /// <summary>
        /// Gets or sets the revit version.
        /// </summary>
        /// <value>
        /// The revit version.
        /// </value>
        public int RevitVersion { get; set; }


        #endregion

        #region Methods

        /// <summary>
        /// Initializes the specified application.
        /// </summary>
        /// <param name="application">The application.</param>
        public void Init(UIApplication application)
        {
            _uiapp = application;
            _uidoc = application.ActiveUIDocument;
            _doc = _uidoc.Document;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
        {
            _uiapp = null;
            _uidoc = null;
            _doc = null;
            _instance = null;
        }

        #endregion
    }
}

using System;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Unitverse.Editor
{
    [Guid(UniqueId)]
    public class ConfigEditorFactory : IVsEditorFactory
    {
        public const string UniqueId = "17dabd6e-84cb-46b9-93ce-caea43da1b6d";
        
        private readonly IUnitTestGeneratorPackage _package;

        public ConfigEditorFactory(IUnitTestGeneratorPackage package)
        {
            _package = package;
        }

        public int SetSite(IOleServiceProvider psp)
        {
            return VSConstants.S_OK;
        }

		public int MapLogicalView(ref Guid rguidLogicalView, out string pbstrPhysicalView)
		{
			pbstrPhysicalView = null;

            return rguidLogicalView == VSConstants.LOGVIEWID_Primary ?
                VSConstants.S_OK :
                VSConstants.E_NOTIMPL;
		}

        public int Close()
        {
            return VSConstants.S_OK;
        }

        [EnvironmentPermission(SecurityAction.Demand, Unrestricted = true)]
        public int CreateEditorInstance(
                        uint grfCreateDoc,
                        string pszMkDocument,
                        string pszPhysicalView,
                        IVsHierarchy pvHier,
                        uint itemid,
                        IntPtr punkDocDataExisting,
                        out IntPtr ppunkDocView,
                        out IntPtr ppunkDocData,
                        out string pbstrEditorCaption,
                        out Guid pguidCmdUI,
                        out int pgrfCDW)
        {
            // Initialize to null
            pguidCmdUI = new Guid(UniqueId);
            pgrfCDW = 0;

            // Create the Document (editor)
            ConfigEditorPane newEditor = new ConfigEditorPane(_package, pszMkDocument);
            ppunkDocView = Marshal.GetIUnknownForObject(newEditor);
            ppunkDocData = Marshal.GetIUnknownForObject(newEditor);
            pbstrEditorCaption = "";

            return VSConstants.S_OK;
        }
    }
}

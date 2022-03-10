using System;
using System.Globalization;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

using Constants = Microsoft.VisualStudio.OLE.Interop.Constants;
using VSConstants = Microsoft.VisualStudio.VSConstants;
using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;
using IOleDataObject = Microsoft.VisualStudio.OLE.Interop.IDataObject;
using Unitverse.Views;
using Unitverse.Core;

namespace Unitverse.Editor
{
    public sealed class ConfigEditorPane : WindowPane, IVsPersistDocData, IPersistFileFormat

    {
        private const uint fileFormat = 0;
        private const string fileExtension = CoreConstants.ConfigFileName;
        private const char endLine = (char)10;
        private string fileName;
        private bool isDirty;
        private bool noScribbleMode;
        private ConfigEditorControl editorControl;

        public ConfigEditorPane(IUnitTestGeneratorPackage package, string filename)
            : base(null)
        {
            noScribbleMode = false;

            Content = editorControl = new ConfigEditorControl(package, filename, OnModified);
        }

        private void OnModified()
        {
            isDirty = true;
            NotifyDocChanged();
        }

        protected override void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);
            base.Dispose(disposing);
        }

        int IPersist.GetClassID(out Guid pClassID)
        {
            pClassID = new Guid(ConfigEditorFactory.UniqueId);
            return VSConstants.S_OK;
        }

        int IPersistFileFormat.GetClassID(out Guid pClassID)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return ((IPersist)this).GetClassID(out pClassID);
        }

        int IPersistFileFormat.GetCurFile(out string ppszFilename, out uint pnFormatIndex)
        {
            pnFormatIndex = fileFormat;
            ppszFilename = fileName;
            return VSConstants.S_OK;
        }

        int IPersistFileFormat.GetFormatList(out string ppszFormatList)
        {
            string formatList = string.Format(CultureInfo.CurrentCulture, "Unitverse config file (*{0}){1}*{0}{1}{1}", fileExtension, endLine);
            ppszFormatList = formatList;
            return VSConstants.S_OK;
        }

        int IPersistFileFormat.SaveCompleted(string pszFilename)
        {
            return noScribbleMode ? VSConstants.S_FALSE : VSConstants.S_OK;
        }

        int IPersistFileFormat.InitNew(uint nFormatIndex)
        {
            isDirty = false;
            return VSConstants.S_OK;
        }

        int IPersistFileFormat.IsDirty(out int pfIsDirty)
        {
            pfIsDirty = isDirty ? 1 : 0;
            return VSConstants.S_OK;
        }

        int IPersistFileFormat.Load(string pszFilename, uint grfMode, int fReadOnly)
        {
            return VSConstants.S_OK;
        }

        int IPersistFileFormat.Save(string pszFilename, int fRemember, uint nFormatIndex)
        {
            noScribbleMode = true;
            try
            {
                if (pszFilename == null || pszFilename == fileName)
                {
                    editorControl.SaveFile(fileName);
                    isDirty = false;
                }
                else
                {
                    if (fRemember != 0)
                    {
                        fileName = pszFilename;
                        editorControl.SaveFile(fileName);
                        isDirty = false;
                    }
                    else
                    {
                        editorControl.SaveFile(pszFilename);
                    }
                }
            }
            finally
            {
                noScribbleMode = false;
            }
            return VSConstants.S_OK;
        }

        int IVsPersistDocData.Close()
        {
            return VSConstants.S_OK;
        }

        int IVsPersistDocData.GetGuidEditorType(out Guid pClassID)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return ((IPersistFileFormat)this).GetClassID(out pClassID);
        }

        int IVsPersistDocData.IsDocDataDirty(out int pfDirty)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return ((IPersistFileFormat)this).IsDirty(out pfDirty);
        }

        int IVsPersistDocData.IsDocDataReloadable(out int pfReloadable)
        {
            pfReloadable = 0;
            return VSConstants.S_OK;
        }

        int IVsPersistDocData.LoadDocData(string pszMkDocument)
        {
            fileName = pszMkDocument;
            return VSConstants.S_OK;
        }

        int IVsPersistDocData.OnRegisterDocData(uint docCookie, IVsHierarchy pHierNew, uint itemidNew)
        {
            return VSConstants.S_OK;
        }

        int IVsPersistDocData.ReloadDocData(uint grfFlags)
        {
            return VSConstants.S_OK;
        }

        int IVsPersistDocData.RenameDocData(uint grfAttribs, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
        {
            return VSConstants.S_OK;
        }

        int IVsPersistDocData.SaveDocData(VSSAVEFLAGS dwSave, out string pbstrMkDocumentNew, out int pfSaveCanceled)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            pbstrMkDocumentNew = null;
            pfSaveCanceled = 0;
            int hr;

            switch (dwSave)
            {
                case VSSAVEFLAGS.VSSAVE_Save:
                case VSSAVEFLAGS.VSSAVE_SilentSave:
                    {
                        IVsQueryEditQuerySave2 queryEditQuerySave = (IVsQueryEditQuerySave2)GetService(typeof(SVsQueryEditQuerySave));

                        hr = queryEditQuerySave.QuerySaveFile(fileName, 0, null, out var result);

                        if (ErrorHandler.Failed(hr))
                        {
                            return hr;
                        }

                        // Process according to result from QuerySave
                        switch ((tagVSQuerySaveResult)result)
                        {
                            case tagVSQuerySaveResult.QSR_NoSave_Cancel:
                                pfSaveCanceled = ~0;
                                break;

                            case tagVSQuerySaveResult.QSR_SaveOK:
                            case tagVSQuerySaveResult.QSR_ForceSaveAs:
                                {
                                    var flags = (tagVSQuerySaveResult)result == tagVSQuerySaveResult.QSR_SaveOK ?
                                        dwSave : VSSAVEFLAGS.VSSAVE_SaveAs;
                                    // Call the shell to do the save for us
                                    IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
                                    hr = uiShell.SaveDocDataToFile(flags, this, fileName, out pbstrMkDocumentNew, out pfSaveCanceled);
                                    if (ErrorHandler.Failed(hr))
                                    {
                                        return hr;
                                    }
                                }
                                break;

                            case tagVSQuerySaveResult.QSR_NoSave_Continue:
                                // In this case there is nothing to do.
                                break;
                        }
                        break;
                    }
                case VSSAVEFLAGS.VSSAVE_SaveAs:
                case VSSAVEFLAGS.VSSAVE_SaveCopyAs:
                    {
                        // Make sure the file name as the right extension
                        if (string.Compare(fileExtension, System.IO.Path.GetExtension(fileName), true, CultureInfo.CurrentCulture) != 0)
                        {
                            fileName += fileExtension;
                        }
                        // Call the shell to do the save for us
                        IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
                        hr = uiShell.SaveDocDataToFile(dwSave, this, fileName, out pbstrMkDocumentNew, out pfSaveCanceled);
                        if (ErrorHandler.Failed(hr))
                        {
                            return hr;
                        }
                        break;
                    }
            };

            return VSConstants.S_OK;
        }

        int IVsPersistDocData.SetUntitledDocPath(string pszDocDataPath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return ((IPersistFileFormat)this).InitNew(fileFormat);
        }

        private void NotifyDocChanged()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            IVsRunningDocumentTable runningDocTable = (IVsRunningDocumentTable)GetService(typeof(SVsRunningDocumentTable));
            runningDocTable.FindAndLockDocument((uint)_VSRDTFLAGS.RDT_ReadLock, fileName, out _, out _, out _, out var docCookie);
            runningDocTable.NotifyDocumentChanged(docCookie, (uint)__VSRDTATTRIB.RDTA_DocDataReloaded);
            runningDocTable.UnlockDocument((uint)_VSRDTFLAGS.RDT_ReadLock, docCookie);
        }
    }
}

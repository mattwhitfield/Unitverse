using System;
using System.Globalization;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

using VSConstants = Microsoft.VisualStudio.VSConstants;
using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;
using Unitverse.Views;
using Unitverse.Core;

namespace Unitverse.Editor
{
    public sealed class ConfigEditorPane : WindowPane, IVsPersistDocData, IPersistFileFormat

    {
        private const uint _fileFormat = 0;
        private const string _fileExtension = CoreConstants.ConfigFileName;
        private const char _endLine = (char)10;
        private string _fileName;
        private bool _isDirty;
        private bool _isSaving;
        private ConfigEditorControl _editorControl;

        public ConfigEditorPane(IUnitTestGeneratorPackage package, string filename)
            : base(null)
        {
            Content = _editorControl = new ConfigEditorControl(package, filename, OnModified);
        }

        private void OnModified()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _isDirty = true;
            NotifyDocChanged();
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
            pnFormatIndex = _fileFormat;
            ppszFilename = _fileName;
            return VSConstants.S_OK;
        }

        int IPersistFileFormat.GetFormatList(out string ppszFormatList)
        {
            string formatList = string.Format(CultureInfo.CurrentCulture, "Unitverse config file (*{0}){1}*{0}{1}{1}", _fileExtension, _endLine);
            ppszFormatList = formatList;
            return VSConstants.S_OK;
        }

        int IPersistFileFormat.SaveCompleted(string pszFilename)
        {
            return _isSaving ? VSConstants.S_FALSE : VSConstants.S_OK;
        }

        int IPersistFileFormat.InitNew(uint nFormatIndex)
        {
            _isDirty = false;
            return VSConstants.S_OK;
        }

        int IPersistFileFormat.IsDirty(out int pfIsDirty)
        {
            pfIsDirty = _isDirty ? 1 : 0;
            return VSConstants.S_OK;
        }

        int IPersistFileFormat.Load(string pszFilename, uint grfMode, int fReadOnly)
        {
            return VSConstants.S_OK;
        }

        int IPersistFileFormat.Save(string pszFilename, int fRemember, uint nFormatIndex)
        {
            _isSaving = true;
            try
            {
                if (pszFilename != null)
                {
                    _fileName = pszFilename;
                }
                _editorControl.SaveFile(_fileName);
                _isDirty = false;
            }
            finally
            {
                _isSaving = false;
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
            _fileName = pszMkDocument;
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

            if (dwSave == VSSAVEFLAGS.VSSAVE_Save || dwSave == VSSAVEFLAGS.VSSAVE_SilentSave)
            {
                IVsQueryEditQuerySave2 queryEditQuerySave = (IVsQueryEditQuerySave2)GetService(typeof(SVsQueryEditQuerySave));
                hr = queryEditQuerySave.QuerySaveFile(_fileName, 0, null, out var result);
                var taggedResult = (tagVSQuerySaveResult)result;

                if (ErrorHandler.Failed(hr))
                {
                    return hr;
                }

                if (taggedResult == tagVSQuerySaveResult.QSR_NoSave_Cancel)
                {
                    pfSaveCanceled = ~0;
                }
                else if (taggedResult == tagVSQuerySaveResult.QSR_SaveOK || taggedResult == tagVSQuerySaveResult.QSR_ForceSaveAs)
                {
                    var flags = (tagVSQuerySaveResult)result == tagVSQuerySaveResult.QSR_SaveOK ? dwSave : VSSAVEFLAGS.VSSAVE_SaveAs;

                    IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
                    hr = uiShell.SaveDocDataToFile(flags, this, _fileName, out pbstrMkDocumentNew, out pfSaveCanceled);
                    if (ErrorHandler.Failed(hr))
                    {
                        return hr;
                    }
                }
            }
            else if (dwSave == VSSAVEFLAGS.VSSAVE_SaveAs || dwSave == VSSAVEFLAGS.VSSAVE_SaveCopyAs)
            {
                if (string.Compare(_fileExtension, System.IO.Path.GetExtension(_fileName), true, CultureInfo.CurrentCulture) != 0)
                {
                    _fileName += _fileExtension;
                }

                IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
                hr = uiShell.SaveDocDataToFile(dwSave, this, _fileName, out pbstrMkDocumentNew, out pfSaveCanceled);
                if (ErrorHandler.Failed(hr))
                {
                    return hr;
                }
            }

            return VSConstants.S_OK;
        }

        int IVsPersistDocData.SetUntitledDocPath(string pszDocDataPath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            return ((IPersistFileFormat)this).InitNew(_fileFormat);
        }

        private void NotifyDocChanged()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (string.IsNullOrWhiteSpace(_fileName))
            {
                return;
            }

            IVsRunningDocumentTable runningDocTable = (IVsRunningDocumentTable)GetService(typeof(SVsRunningDocumentTable));
            runningDocTable.FindAndLockDocument((uint)_VSRDTFLAGS.RDT_ReadLock, _fileName, out _, out _, out _, out var docCookie);
            runningDocTable.NotifyDocumentChanged(docCookie, (uint)__VSRDTATTRIB.RDTA_DocDataReloaded);
            runningDocTable.UnlockDocument((uint)_VSRDTFLAGS.RDT_ReadLock, docCookie);
        }
    }
}

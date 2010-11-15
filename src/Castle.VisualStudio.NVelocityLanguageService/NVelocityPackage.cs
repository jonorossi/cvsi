// Copyright 2007-2008 Jonathon Rossi - http://www.jonorossi.com/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.VisualStudio.NVelocityLanguageService
{
	using System;
	using System.ComponentModel.Design;
	using System.Runtime.InteropServices;
	using Microsoft.VisualStudio;
	using Microsoft.VisualStudio.OLE.Interop;
	using Microsoft.VisualStudio.Shell;
	using Microsoft.VisualStudio.Shell.Interop;

	/// <summary>
	/// This is the class that implements the package exposed by this assembly.
	///
	/// The minimum requirement for a class to be considered a valid package for Visual Studio
	/// is to implement the IVsPackage interface and register itself with the shell.
	/// This package uses the helper classes defined inside the Managed Package Framework (MPF)
	/// to do it: it derives from the Package class that provides the implementation of the
	/// IVsPackage interface and uses the registration attributes defined in the framework to
	/// register itself and its components with the shell.
	/// </summary>

	// This attribute tells the registration utility (regpkg.exe) that this class needs
	// to be registered as package.
	[PackageRegistration(UseManagedResourcesOnly = true)]

	[ProvideService(typeof(NVelocityLanguage), ServiceName = "NVelocity")]

	[ProvideLanguageService(typeof(NVelocityLanguage), "NVelocity", 100,
		AutoOutlining = true,
		//CodeSense = true,
		//CodeSenseDelay = 0,
		//DefaultToInsertSpaces = true,
		//EnableAsyncCompletion = true,
		//EnableCommenting = true,
		MatchBraces = true,
		MatchBracesAtCaret = true,
		RequestStockColors = false
		//ShowCompletion = true,
		//ShowMatchingBrace = true
	)]

	// This attribute is used to associate the ".vm" and ".njs" file extensions with a language service
	[ProvideLanguageExtension(typeof(NVelocityLanguage), NVelocityConstants.NVelocityFileExtension)]
	[ProvideLanguageExtension(typeof(NVelocityLanguage), NVelocityConstants.NVelocityJSFileExtension)]

	// A Visual Studio component can be registered under different registry roots; for instance
	// when you debug your package you want to register it in the experimental hive. This
	// attribute specifies the registry root to use if one is not provided to regpkg.exe with
	// the /root switch.

#if VS2005
    [DefaultRegistryRoot(@"Software\Microsoft\VisualStudio\8.0")]
#elif VS2008
    [DefaultRegistryRoot(@"Software\Microsoft\VisualStudio\9.0")]
#else
    [DefaultRegistryRoot(@"Software\Microsoft\VisualStudio\10.0Exp")]
#endif

    // This attribute is used to register the informations needed to show the this package
	// in the Help/About dialog of Visual Studio.
	[InstalledProductRegistration(true, "#ProductName", "#ProductDetails", "0.4", IconResourceID = 100,
		LanguageIndependentName = "Castle Visual Studio Integration")]

	// In order to be loaded inside Visual Studio in a machine that does not have the VS SDK
	// installed, the package needs to have a valid load key (it can be requested at 
	// http://msdn.microsoft.com/vstudio/extend/). This attributes tells the shell that this
	// package has a load key embedded in its resources.

#if VS2005
    [ProvideLoadKey("Standard", "0.3", "Castle Visual Studio Integration", "Jonathon Rossi", 2005)]
#elif VS2008
    [ProvideLoadKey("Standard", "0.3", "Castle Visual Studio Integration", "Jonathon Rossi", 2008)]
#else
    //TODO
#endif

    [Guid(NVelocityConstants.PackageGuidString)]
    [ProvideAutoLoad("{8fe2df1d-e0da-4ebe-9d5c-415d40e487b5}")]
    public sealed class NVelocityPackage : Package, IOleComponent, IVsInstalledProduct
    {
        private uint componentID;

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public NVelocityPackage()
        {
            IServiceContainer container = this;
            ServiceCreatorCallback callback = CreateService;
            container.AddService(typeof(NVelocityLanguage), callback, true);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (componentID != 0)
                {
                    IOleComponentManager mgr = GetService(typeof(SOleComponentManager)) as IOleComponentManager;
                    if (mgr != null)
                        mgr.FRevokeComponent(componentID);
                    componentID = 0;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        private object CreateService(IServiceContainer container, Type serviceType)
        {
            if (serviceType == typeof(NVelocityLanguage))
            {
                NVelocityLanguage language = new NVelocityLanguage();
                language.SetSite(this);
                RegisterForIdleTime();
                return language;
            }
            return null;
        }

		private void RegisterForIdleTime()
		{
			IOleComponentManager mgr = GetService(typeof(SOleComponentManager)) as IOleComponentManager;
			if (componentID == 0 && mgr != null)
			{
				OLECRINFO[] crinfo = new OLECRINFO[1];
				crinfo[0].cbSize = (uint)Marshal.SizeOf(typeof(OLECRINFO));
				crinfo[0].grfcrf = (uint)_OLECRF.olecrfNeedIdleTime |
					(uint)_OLECRF.olecrfNeedPeriodicIdleTime;
				crinfo[0].grfcadvf = (uint)_OLECADVF.olecadvfModal |
					(uint)_OLECADVF.olecadvfRedrawOff |
					(uint)_OLECADVF.olecadvfWarningsOff;
				crinfo[0].uIdleTimeInterval = 1000;
				/*int hr = */mgr.FRegisterComponent(this, crinfo, out componentID);
			}
		}

        #region IOleComponent Members

        public int FContinueMessageLoop(uint uReason, IntPtr pvLoopData, MSG[] pMsgPeeked)
        {
            return 1;
        }

        public int FDoIdle(uint grfidlef)
        {
            NVelocityLanguage langService = GetService(typeof(NVelocityLanguage)) as NVelocityLanguage;
            if (langService != null)
            {
                langService.OnIdle((grfidlef & (uint)_OLEIDLEF.oleidlefPeriodic) != 0);
            }
            return 0;
        }

        public int FPreTranslateMessage(MSG[] pMsg)
        {
            return 0;
        }

        public int FQueryTerminate(int fPromptUser)
        {
            return 1;
        }

        public int FReserved1(uint dwReserved, uint message, IntPtr wParam, IntPtr lParam)
        {
            return 1;
        }

        public IntPtr HwndGetWindow(uint dwWhich, uint dwReserved)
        {
            return IntPtr.Zero;
        }

        public void OnActivationChange(IOleComponent pic, int fSameComponent, OLECRINFO[] pcrinfo, int fHostIsActivating, OLECHOSTINFO[] pchostinfo, uint dwReserved)
        {
        }

        public void OnAppActivate(int fActive, uint dwOtherThreadID)
        {
        }

        public void OnEnterState(uint uStateID, int fEnter)
        {
        }

        public void OnLoseActivation()
        {
        }

        public void Terminate()
        {
        }

        #endregion

        #region IVsInstalledProduct Members

        int IVsInstalledProduct.IdBmpSplash(out uint pIdBmp)
        {
            pIdBmp = 100;
            return VSConstants.S_OK;
        }

        int IVsInstalledProduct.IdIcoLogoForAboutbox(out uint pIdIco)
        {
            pIdIco = 100;
            return VSConstants.S_OK;
        }

        int IVsInstalledProduct.OfficialName(out string pbstrName)
        {
            pbstrName = GetResourceString("ProductName");
            return VSConstants.S_OK;
        }

        int IVsInstalledProduct.ProductDetails(out string pbstrProductDetails)
        {
            pbstrProductDetails = GetResourceString("ProductDetails");
            return VSConstants.S_OK;
        }

        int IVsInstalledProduct.ProductID(out string pbstrPID)
        {
            pbstrPID = GetResourceString("ProductVersion");
            return VSConstants.S_OK;
        }

        #endregion

        private string GetResourceString(string resourceName)
        {
            string resourceValue;

            IVsResourceManager resourceManager = (IVsResourceManager)GetService(typeof(SVsResourceManager));

			if (resourceManager == null)
			{
				throw new InvalidOperationException("Could not get SVsResourceManager service. Make " +
					"sure that the package is sited before calling this method");
			}

            Guid packageGuid = GetType().GUID;
            int hr = resourceManager.LoadResourceString(ref packageGuid, -1, resourceName, out resourceValue);

            ErrorHandler.ThrowOnFailure(hr);

            return resourceValue;
        }
    }
}
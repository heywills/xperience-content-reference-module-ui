using CMS;
using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using KenticoCommunity.ContentReferenceUi.Core;
using KenticoCommunity.ContentReferenceUi.Installers;
using KenticoCommunity.ContentReferenceUi.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: RegisterModule(typeof(ContentReferenceUiInstallerModule))]


namespace KenticoCommunity.ContentReferenceUi.Modules
{
    public class ContentReferenceUiInstallerModule : Module
    {
        public ContentReferenceUiInstallerModule() : base(nameof(ContentReferenceUiInstallerModule))
        {
        }
        protected override void OnPreInit()
        {
            base.OnPreInit();
            Service.Use<IContentReferenceUiInstaller, ContentReferenceUiInstaller>();
        }

        /// <summary>
        /// Initialize the module by creating the IContentReferenceIndexService
        /// </summary>
        /// <remarks>
        /// The first dependency is created using Service.Resolve, which uses the DI container.
        /// However, all other dependencies in the chain will be created automatically using
        /// constructor-based injection.
        /// </remarks>
        protected override void OnInit()
        {
            if (IsRunningInCmsApp())
            {
                var contentReferenceUiInstaller = Service.Resolve<IContentReferenceUiInstaller>();
                contentReferenceUiInstaller.Install();
            }
            base.OnInit();
        }

        private static bool IsRunningInCmsApp()
        {
            return (SystemContext.IsCMSRunningAsMainApplication && SystemContext.IsWebSite);
        }

    }
}

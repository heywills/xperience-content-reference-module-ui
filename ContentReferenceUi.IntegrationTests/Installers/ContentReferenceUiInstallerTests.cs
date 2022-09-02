using CMS.Core;
using CMS.DataEngine;
using CMS.Modules;
using CMS.SiteProvider;
using KenticoCommunity.ContentReferenceUi.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ContentReferenceUi.IntegrationTests
{
    [TestClass]
    public class ContentReferenceUiInstallerTests
    {
        [TestMethod]
        public void Create_Installs_Kentico_Module_Objects()
        {
            IContentReferenceUiInstaller contentReferenceUiInstaller = Service.Resolve<IContentReferenceUiInstaller>();
            contentReferenceUiInstaller.Install();
        }
    }
}

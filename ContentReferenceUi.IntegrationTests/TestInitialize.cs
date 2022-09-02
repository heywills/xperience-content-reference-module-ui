using CMS.Core;
using CMS.DataEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KenticoCommunity.ContentReferenceUi.Installers;
using KenticoCommunity.ContentReferenceUi.Core;

namespace KenticoCommunity.ContentReferenceUi.IntegrationTests
{
    [TestClass]
    public class TestInitialize
    {
        [AssemblyInitialize]
        public void Initialize()
        {
            Service.Use<IContentReferenceUiInstaller, ContentReferenceUiInstaller>();
            CMSApplication.Init();
        }
    }
}

using CMS.Core;
using CMS.DataEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KenticoCommunity.ContentReferenceUi.Installers;
using KenticoCommunity.ContentReferenceUi.Core;

namespace KenticoCommunity.ContentReferenceUi.IntegrationTests
{
    [TestClass]
    public static class TestInitialize
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            Service.Use<IContentReferenceUiInstaller, ContentReferenceUiInstaller>();
            CMSApplication.Init();
        }
    }
}

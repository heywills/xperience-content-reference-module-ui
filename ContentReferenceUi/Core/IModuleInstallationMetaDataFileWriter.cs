using CMS.Modules;

namespace KenticoCommunity.ContentReferenceUi.Core
{
    internal interface IModuleInstallationMetaDataFileWriter
    {
        void EnsureModuleMetaDataFiles(ResourceInfo resourceInfo);
    }
}

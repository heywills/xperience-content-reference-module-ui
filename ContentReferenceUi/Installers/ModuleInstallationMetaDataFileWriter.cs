using CMS.Base;
using CMS.Core;
using CMS.Modules;
using CmsIo = CMS.IO;
using KenticoCommunity.ContentReferenceUi.Core;
using System.IO;
using System.Xml.Serialization;

namespace KenticoCommunity.ContentReferenceUi.Installers
{
    /// <summary>
    /// This class writes two meta files into the Kentico CMS tree.
    /// 
    /// ***Packages file***
    /// The file written to ~\App_Data\CMSModules\CMSInstallation\Packages\
    /// is used for Kentico to detect when the NuGet package is uninstalled
    /// or installed. Therefore, it must be deployed by the NuGet package 
    /// definition.
    /// However, this module will insure it exists, to prevent the module
    /// from being uninstalled because the file was missed in a deployment
    /// to upper environments.
    /// 
    /// ***Installed file***
    /// The file writtne to ~\App_Data\CMSModules\CMSInstallation\Packages\Installed
    /// is not included in the NuGet package, so that it is left behind when the 
    /// NuGet package is uninstalled. If Kentico starts and sees this file, but cannot
    /// find the same file in the "Packages" folder, it will delete the Kentico
    /// database objects assocated to the module.    /// 
    /// </summary>
    internal class ModuleInstallationMetaDataFileWriter : IModuleInstallationMetaDataFileWriter
    {
        const string _relativeInstalledModulesMetaFilesPath = @"App_Data\CMSModules\CMSInstallation\Packages\Installed";
        const string _relativPackagesModulesMetaFilesPath = @"App_Data\CMSModules\CMSInstallation\Packages\";

        /// <summary>
        /// Ensure both the packages metafile and the installed metafile exist.
        /// The NuGet package should deploy the "packagtes" metafile, but not the "installed"
        /// metafile.
        /// </summary>
        /// <param name="resourceInfo"></param>
        public void EnsureModuleMetaDataFiles(ResourceInfo resourceInfo)
        {
            var moduleInstallationMetaData = new ModuleInstallationMetaData()
                                                    {
                                                        Name = resourceInfo.ResourceName,
                                                        Version = resourceInfo.ResourceInstalledVersion
                                                    };
            string tokenMetaFileName = GetModuleInstallationMetaDataFileName(moduleInstallationMetaData);
            EnsureInstalledMetaDataFile(moduleInstallationMetaData, tokenMetaFileName);
        }

        /// <summary>
        /// Ensure the module metafile exists in the "Installed" folder. This is the metafile
        /// that Kentico looks for to see if there is a module to uninstall. If it can't find a
        /// matching one in the "Packages" folder, it will delete the modules database objects.
        /// </summary>
        /// <param name="moduleInstallationMetaData"></param>
        /// <param name="tokenMetaFileName"></param>
        private void EnsureInstalledMetaDataFile(ModuleInstallationMetaData moduleInstallationMetaData, string tokenMetaFileName)
        {
            string installedNuGetModulesMetaFilesPath = GetInstalledNuGetModulesMetaFilesPath();
            if (!Directory.Exists(installedNuGetModulesMetaFilesPath))
            {
                CmsIo.DirectoryHelper.EnsureDiskPath(installedNuGetModulesMetaFilesPath, GetRootPath());
            }
            string tokenMetaFilePath = Path.Combine(installedNuGetModulesMetaFilesPath, tokenMetaFileName);
            WriteModuleMetaDataFile(tokenMetaFilePath, moduleInstallationMetaData);
        }

        /// <summary>
        /// Ensure the module metafile exists in the "Packages" folder. This is the metafile
        /// that Kentico looks for to see if the module is installed. If it finds this file, it
        /// will know that the Nuget package is installed and will not delete the modules database objects.
        /// </summary>
        /// <param name="moduleInstallationMetaData"></param>
        /// <param name="tokenMetaFileName"></param>
        private void EnsurePackagesMetaDataFile(ModuleInstallationMetaData moduleInstallationMetaData, string tokenMetaFileName)
        {
            string packagesNuGetModulesMetaFilesPath = GetPackageNuGetModulesMetaFilesPath();
            if (!Directory.Exists(packagesNuGetModulesMetaFilesPath))
            {
                CmsIo.DirectoryHelper.EnsureDiskPath(packagesNuGetModulesMetaFilesPath, GetRootPath());
            }
            string tokenMetaFilePath = Path.Combine(packagesNuGetModulesMetaFilesPath, tokenMetaFileName);
            WriteModuleMetaDataFile(tokenMetaFilePath, moduleInstallationMetaData);
        }

        private void WriteModuleMetaDataFile(string targetFilePath, ModuleInstallationMetaData moduleInstallationMetaData)
        {
            using (var stream = CmsIo.FileStream.New(targetFilePath, CmsIo.FileMode.Create, CmsIo.FileAccess.Write))
            {
                new XmlSerializer(typeof(ModuleInstallationMetaData)).Serialize(stream, moduleInstallationMetaData);
            }
        }

        /// <summary>
        /// Rooted path to directory containing meta files of installed modules (with trailing slash).
        /// </summary>
        private string GetInstalledNuGetModulesMetaFilesPath()
        {
            return CmsIo.Path.EnsureEndSlash(Path.Combine(GetRootPath(), _relativeInstalledModulesMetaFilesPath));
        }

        /// <summary>
        /// Rooted path to the "packages" directory containing meta files of installed modules (with trailing slash).
        /// </summary>
        private string GetPackageNuGetModulesMetaFilesPath()
        {
            return CmsIo.Path.EnsureEndSlash(Path.Combine(GetRootPath(), _relativPackagesModulesMetaFilesPath));
        }

        /// <summary>
        /// Root path for resolving relative paths.
        /// </summary>
        private string GetRootPath()
        {
            return SystemContext.WebApplicationPhysicalPath;
        }

        /// <summary>
        /// Gets module installation meta file name for module meta data.
        /// </summary>
        /// <param name="moduleInstallationMetaData">Basic module installation meta data.</param>
        /// <returns>Token meta file name.</returns>
        private string GetModuleInstallationMetaDataFileName(ModuleInstallationMetaData moduleInstallationMetaData)
        {
            return $"{moduleInstallationMetaData.Name}_{moduleInstallationMetaData.Version}.xml";
        }

    }
}

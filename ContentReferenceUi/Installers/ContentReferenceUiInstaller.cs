using CMS.Base;
using CMS.Core;
using CMS.Modules;
using CMS.PortalEngine;
using CMS.SiteProvider;
using KenticoCommunity.ContentReferenceUi.Constants;
using KenticoCommunity.ContentReferenceUi.Core;
using System;
using System.Linq;

namespace KenticoCommunity.ContentReferenceUi.Installers
{
    internal class ContentReferenceUiInstaller : IContentReferenceUiInstaller
    {
        private readonly IEventLogService _eventLogService;
        private readonly IResourceInfoProvider _resourceInfoProvider;
        private readonly IUIElementInfoProvider _uIElementInfoProvider;
        private readonly IResourceSiteInfoProvider _resourceSiteInfoProvider;
        private readonly ISiteInfoProvider _siteInfoProvider;


        public ContentReferenceUiInstaller(IEventLogService eventLogService,
                                           IResourceInfoProvider resourceInfoProvider,
                                           IUIElementInfoProvider uIElementInfoProvider,
                                           IResourceSiteInfoProvider resourceSiteInfoProvider,
                                           ISiteInfoProvider siteInfoProvider)
        {
            _eventLogService = eventLogService;
            _resourceInfoProvider = resourceInfoProvider;
            _uIElementInfoProvider = uIElementInfoProvider;
            _resourceSiteInfoProvider = resourceSiteInfoProvider;
            _siteInfoProvider = siteInfoProvider;
        }

        public void Install()
        {
            try
            {
                var resourceInfo = InstallResourceInfo();
                AssignModuleToSites(resourceInfo);
            }
            catch(Exception ex)
            {
                _eventLogService.LogException(nameof(ContentReferenceUiInstaller),
                                              "ERROR",
                                              ex);
            }
        }

        private void AssignModuleToSites(ResourceInfo resourceInfo)
        {
            using (new CMSActionContext
            {
                LogSynchronization = false,
                ContinuousIntegrationAllowObjectSerialization = false
            })
            {

                var unassignedSites = _siteInfoProvider
                                    .Get()
                                    .WhereNotIn("SiteID",
                                    _resourceSiteInfoProvider
                                        .Get()
                                        .Column("SiteID")
                                        .WhereEquals("ResourceID", resourceInfo.ResourceID)
                                    )
                                    .Select(siteInfo => siteInfo.SiteID)
                                    .ToList();

                unassignedSites.ForEach(siteId => _resourceSiteInfoProvider
                                                    .Add(resourceInfo.ResourceID, siteId));
                var unassignedSiteCount = unassignedSites.Count;
                if (unassignedSiteCount > 0)
                {
                    LogInformation("ASSIGNED", $"Assigned the module '{ResourceConstants.ResourceDisplayName}' to {unassignedSiteCount} sites.");
                }
            }
        }

        private ResourceInfo InstallResourceInfo()
        {
            using (new CMSActionContext
            {
                LogSynchronization = false,
                ContinuousIntegrationAllowObjectSerialization = false
            })
            {
                var resourceInfo = _resourceInfoProvider.Get(ResourceConstants.ResourceName);
                if (InstalledModuleIsCurrent(resourceInfo))
                {
                    LogInformation("CURRENT", $"The '{ResourceConstants.ResourceDisplayName}' module is already installed and current.");
                    return resourceInfo;
                }
                LogInformation("START", $"{(resourceInfo == null ? "Installing" : "Updating")} the module '{ResourceConstants.ResourceDisplayName}'.");
                if (resourceInfo == null)
                {
                    resourceInfo = new ResourceInfo();
                }

                resourceInfo.ResourceDisplayName = ResourceConstants.ResourceDisplayName;
                resourceInfo.ResourceName = ResourceConstants.ResourceName;
                resourceInfo.ResourceDescription = ResourceConstants.ResourceDescription;
                resourceInfo.ResourceAuthor = ResourceConstants.ResourceAuthor;
                resourceInfo.ResourceIsInDevelopment = ResourceConstants.ResourceIsInDevelopment;
                // Setting ResourceInstallationState to 'installed' will cause Kentico to uninstall related objects if it
                // finds a module meta file in ~\App_Data\CMSModules\CMSInstallation\Packages\Installed
                resourceInfo.ResourceInstallationState = ResourceConstants.ResourceInstallationState;
                _resourceInfoProvider.Set(resourceInfo);

                InstallUiElement(resourceInfo);
                StoreInstalledVersion(resourceInfo);
                LogInformation("COMPLETE", $"{(resourceInfo == null ? "Install" : "Update")} of the module '{ResourceConstants.ResourceDisplayName}' version {resourceInfo.ResourceVersion} is complete.");
                return resourceInfo;
            }
        }

        /// <summary>
        /// Store the version number of the installed module. This should
        /// be done after the ResourceInfo and UiElementInfo are successfully
        /// updated and saved.
        /// </summary>
        /// <param name="resourceInfo"></param>
        private void StoreInstalledVersion(ResourceInfo resourceInfo)
        {
            using (new CMSActionContext
            {
                LogSynchronization = false,
                ContinuousIntegrationAllowObjectSerialization = false
            })
            {
                string newVersion = GetModuleVersionFromAssembly();
                resourceInfo.ResourceInstalledVersion = newVersion;
                resourceInfo.ResourceVersion = newVersion;
                _resourceInfoProvider.Set(resourceInfo);
            }
        }

        private void InstallUiElement(ResourceInfo resourceInfo)
        {
            using (new CMSActionContext
            {
                LogSynchronization = false,
                ContinuousIntegrationAllowObjectSerialization = false
            })
            {
                // NOTE: The static method GetUIElementInfo has significant optimizations for this
                // query that are not provided by the injected implementation.
                var uiElement = UIElementInfoProvider.GetUIElementInfo(resourceInfo.ResourceName, UiElementConstants.ElementName)
                            ??
                            new UIElementInfo();
                uiElement.ElementResourceID = resourceInfo.ResourceID;
                uiElement.ElementName = UiElementConstants.ElementName;
                uiElement.ElementDisplayName = UiElementConstants.ElementDisplayName;
                uiElement.ElementCaption = UiElementConstants.ElementCaption;
                uiElement.ElementIsCustom = UiElementConstants.ElementIsCustom;
                uiElement.ElementRequiresGlobalAdminPriviligeLevel = UiElementConstants.ElementRequiresGlobalAdminPriviligeLevel;
                uiElement.ElementOrder = UiElementConstants.ElementOrder;
                uiElement.ElementType = UIElementTypeEnum.PageTemplate;
                uiElement.ElementPageTemplateID = GetPageTemplateInfo(UiElementConstants.ElementPageTemplateCodeName)
                                                  .PageTemplateId;

                uiElement.ElementParentID = GetSystemUiElementInfo(UiElementConstants.ParentElementResourceName, UiElementConstants.ParentElementName)
                                            .ElementID;

                uiElement.ElementProperties = UiElementConstants.ElementProperties;
                _uIElementInfoProvider.Set(uiElement);
            }
        }

        private UIElementInfo GetSystemUiElementInfo(string resourceName, string uiElementName)
        {
            var uiElement = UIElementInfoProvider.GetUIElementInfo(resourceName, uiElementName);
            if (uiElement == null)
            {
                throw new ArgumentOutOfRangeException($"Kentico's system UI Element, {uiElementName}, in the {resourceName} module is missing!");
            }
            return uiElement;
        }

        private PageTemplateInfo GetPageTemplateInfo(string pageTemplateCodeName)
        {
            var pageTemplateInfo = PageTemplateInfoProvider.GetPageTemplateInfo(pageTemplateCodeName);
            if (pageTemplateInfo == null)
            {
                throw new ArgumentOutOfRangeException($"Kentico's system page template, {pageTemplateCodeName} is missing!");
            }
            return pageTemplateInfo;
        }

        private bool InstalledModuleIsCurrent(ResourceInfo resourceInfo)
        {
            return (resourceInfo != null) &&
                   (GetModuleVersionFromAssembly() == resourceInfo.ResourceInstalledVersion);
        }

        /// <summary>
        /// Create a Module version number from the assembly version.
        /// The module version must be in 3 parts (e.g. 1.0.13).
        /// </summary>
        /// <returns></returns>
        private string GetModuleVersionFromAssembly()
        {
            return this.GetType().Assembly.GetName().Version.ToString(3);
        }

        private void LogInformation(string eventCode, string eventMessage)
        {
            _eventLogService.LogEvent(EventTypeEnum.Information,
                                      nameof(ContentReferenceUiInstaller),
                                      eventCode,
                                      eventMessage);
        }
    }
}

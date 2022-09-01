using CMS.Core;
using CMS.Modules;
using CMS.PortalEngine;
using CMS.SiteProvider;
using KenticoCommunity.ContentReferenceUi.Constants;
using System;
using System.Linq;

namespace KenticoCommunity.ContentReferenceUi.Installers
{
    internal class ContentReferenceUiInstaller
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

        public void Installer()
        {
            var resourceInfo = InstallResourceInfo();

            AssignModuleToSites(resourceInfo);
        }

        private void AssignModuleToSites(ResourceInfo resourceInfo)
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
        }

        private ResourceInfo InstallResourceInfo()
        {
            var resourceInfo = _resourceInfoProvider.Get(ResourceConstants.ResourceName);
            if(InstalledModuleIsCurrent(resourceInfo))
            {
                LogInformation("CURRENT", $"The '{ResourceConstants.ResourceName}' module is already installed and current.");
                return resourceInfo;
            }

            if(resourceInfo == null)
            {
                resourceInfo = new ResourceInfo();
            }

            resourceInfo.ResourceDisplayName = ResourceConstants.ResourceDisplayname;
            resourceInfo.ResourceName = ResourceConstants.ResourceName;
            resourceInfo.ResourceDescription = ResourceConstants.ResourceDescription;
            resourceInfo.ResourceAuthor = ResourceConstants.ResourceAuthor;
            resourceInfo.ResourceIsInDevelopment = ResourceConstants.ResourceIsInDevelopment;
            _resourceInfoProvider.Set(resourceInfo);

            InstallUiElement(resourceInfo);
            StoreInstalledVersion(resourceInfo);
            return resourceInfo;
        }


        /// <summary>
        /// Store the version number of the installed module. This should
        /// be done after the ResourceInfo and UiElementInfo are successfully
        /// updated and saved.
        /// </summary>
        /// <param name="resourceInfo"></param>
        private void StoreInstalledVersion(ResourceInfo resourceInfo)
        {
            string newVersion = GetAssemblyVersion();
            resourceInfo.ResourceInstalledVersion = newVersion;
            resourceInfo.ResourceVersion = newVersion;
            _resourceInfoProvider.Set(resourceInfo);
        }

        private void InstallUiElement(ResourceInfo resourceInfo)
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
            uiElement.ElementType = UIElementTypeEnum.PageTemplate;
            uiElement.ElementPageTemplateID = GetPageTemplateInfo(UiElementConstants.ElementPageTemplateCodeName)
                                              .PageTemplateId;

            uiElement.ElementParentID = GetSystemUiElementInfo(UiElementConstants.ParentElementResourceName, UiElementConstants.ParentElementName)
                                        .ElementID;

            uiElement.ElementProperties = UiElementConstants.ElementProperties;
            _uIElementInfoProvider.Set(uiElement);
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
            if(pageTemplateInfo == null)
            {
                throw new ArgumentOutOfRangeException($"Kentico's system page template, {pageTemplateCodeName} is missing!");
            }
            return pageTemplateInfo;
        }

        private bool InstalledModuleIsCurrent(ResourceInfo resourceInfo)
        {
            return (resourceInfo != null) &&
                   (GetAssemblyVersion() == resourceInfo.ResourceInstalledVersion);
        }

        private string GetAssemblyVersion()
        {
            return this.GetType().Assembly.GetName().Version.ToString();
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

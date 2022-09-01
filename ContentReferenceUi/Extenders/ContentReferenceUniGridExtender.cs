using CMS;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.UIControls;
using KenticoCommunity.ContentReferenceUi.Extenders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using XperienceCommunity.ContentReferenceModule.ContentReferences.Core;
using XperienceCommunity.ContentReferenceModule.ContentReferences.Models;

[assembly: RegisterCustomClass(nameof(ContentReferenceUniGridExtender), typeof(ContentReferenceUniGridExtender))]

namespace KenticoCommunity.ContentReferenceUi.Extenders
{
    public class ContentReferenceUniGridExtender: ControlExtender<UniGrid>
    {
        private const string CMS_VIEW_LISTING_SCRIPT = "~/CMSModules/Content/CMSDesk/View/Listing.js";

        public override void OnInit()
        {
            Control.OnDataReload += Control_OnDataReload;
            LoadScriptsRequiredForEditAction();
        }

        private void LoadScriptsRequiredForEditAction()
        {
            var page = Control.Page;
            ScriptHelper.RegisterLoader(page);
            ScriptHelper.RegisterDialogScript(page);
            ScriptHelper.RegisterJQuery(page);
            ScriptHelper.RegisterScriptFile(Control, CMS_VIEW_LISTING_SCRIPT);
        }

        private DataSet Control_OnDataReload(string completeWhere,
                                                   string currentOrder,
                                                   int currentTopN,
                                                   string columns,
                                                   int currentOffset,
                                                   int currentPageSize,
                                                   ref int totalRecords)
        {
            try
            {
                var currentNode = GetCurrentNode();
                var contentReferences = GetReferences(currentNode);
                var contentReferenceGuids = contentReferences.Where(c => c.NodeGuid.HasValue)
                                                             .Select(c => c.NodeGuid.Value);

                var docs = DocumentHelper.GetDocuments()
                    .WhereIn("NodeGuid", contentReferenceGuids.ToList())
                    .OrderBy("NodeAliasPath")
                    .Culture(currentNode.DocumentCulture)
                    .Published(false)
                    .LatestVersion(true);
                return docs.Result;
            }
            catch(Exception ex)
            {
                Control.AddMessage(MessageTypeEnum.Error, $"An unexpected error occured: {ex.Message}");
            }
            return null;
        }

        private IEnumerable<ContentReference> GetReferences(ITreeNode currentNode)
        {
            var contentReferenceService = Service.Resolve<IContentReferenceService>();
            return contentReferenceService.GetParentReferencesByNode(currentNode);
        }

        private CMSUIPage GetCmsUiPage()
        {
            var page = Control.Page;
            if(!(page is CMSUIPage))
            {
                throw new InvalidOperationException($"The {nameof(ContentReferenceUniGridExtender)} can only be used on a Kentico CMS UI page.");
            }
            return (CMSUIPage)page;
        }

        private ITreeNode GetCurrentNode()
        {
            var cmsUiPage = GetCmsUiPage();
            var nodeId = cmsUiPage.NodeID;
            var cultureCode = cmsUiPage.CultureCode;
            if (!(nodeId > 0))
            {
                throw new ArgumentException($"The nodeId parameter is invalid.");
            }
            if (string.IsNullOrWhiteSpace(cultureCode))
            {
                throw new ArgumentException($"The culture parameter is invalid.");
            }
            var currentNode = DocumentHelper.GetDocument(nodeId, cultureCode, cmsUiPage.Tree);
            if(currentNode == null)
            {
                throw new ArgumentException($"Could not identify the current TreeNode. This is usually because the nodeId or culture parameter is invalid.");
            }
            return currentNode;
        }
    }
}
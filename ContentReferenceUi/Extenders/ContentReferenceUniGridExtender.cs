using CMS;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.UIControls;
using KenticoCommunity.ContentReferenceUi.Extenders;
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
        public override void OnInit()
        {
            Control.OnDataReload += Control_OnDataReload;
        }

        private DataSet Control_OnDataReload(string completeWhere,
                                                   string currentOrder,
                                                   int currentTopN,
                                                   string columns,
                                                   int currentOffset,
                                                   int currentPageSize,
                                                   ref int totalRecords)
        {
            var currentNode = GetCurrentTreeNode();
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

        private IEnumerable<ContentReference> GetReferences(ITreeNode currentNode)
        {
            var contentReferenceService = Service.Resolve<IContentReferenceService>();
            return contentReferenceService.GetParentReferencesByNode(currentNode);
        }

        private ITreeNode GetCurrentTreeNode()
        {
            var page = base.Control.Page;
            var cmsPropertyPage = (page is CMSPropertiesPage) ? (CMSPropertiesPage)page : null;
            if (cmsPropertyPage == null)
            {
                throw new System.Exception("This extender can only be used on a CMSPropertyPage"); // TODO: Improve this.
            }
            return cmsPropertyPage.Node;
        }

    }
}
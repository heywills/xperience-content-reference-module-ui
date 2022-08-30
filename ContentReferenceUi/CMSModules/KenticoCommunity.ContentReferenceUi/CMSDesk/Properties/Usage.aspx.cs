using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.UIControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using XperienceCommunity.ContentReferenceModule.ContentReferences.Core;
using XperienceCommunity.ContentReferenceModule.ContentReferences.Models;

public partial class KenticoCommunity_ContentReferenceUi_CMSDesk_Properties_Usage : CMSPropertiesPage
{
    private const string CMS_VIEW_LISTING_SCRIPT = "~/CMSModules/Content/CMSDesk/View/Listing.js";
    private const string MESSAGE_PAGE_CONTEXT_UNAVAILABLE = "The current page context is not available.";
    private const string MESSAGE_CONTENT_NOT_REFERENCED = "This content is not referenced.";

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Node == null)
        {
            lblMessage.Visible = true;
            lblMessage.Text = MESSAGE_PAGE_CONTEXT_UNAVAILABLE;
            return;
        }
        ScriptHelper.RegisterLoader(Page);
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterScriptFile(this, CMS_VIEW_LISTING_SCRIPT);

        gridDocuments.ZeroRowsText = GetString(MESSAGE_CONTENT_NOT_REFERENCED);
        gridDocuments.OnDataReload += gridDocuments_OnDataReload;
    }

    private IEnumerable<ContentReference> GetReferences()
    {
        var node = base.Node;
        var contentReferenceService = Service.Resolve<IContentReferenceService>();
        var listOfWhereNodeIsUsed = contentReferenceService.GetParentReferencesByNode(node);
        return listOfWhereNodeIsUsed;
    }

    private DataSet gridDocuments_OnDataReload(string completeWhere,
                                               string currentOrder,
                                               int currentTopN,
                                               string columns,
                                               int currentOffset,
                                               int currentPageSize,
                                               ref int totalRecords)
    {
        var contentReferences = GetReferences();
        var contentReferenceGuids = contentReferences.Where(c => c.NodeGuid.HasValue)
                                                     .Select(c => c.NodeGuid.Value);

        var docs = DocumentHelper.GetDocuments()
            .WhereIn("NodeGuid", contentReferenceGuids.ToList())
            .OrderBy("NodeAliasPath")
            .Culture("en-US")
            .Published(false)
            .LatestVersion(true);
        return docs.Result;
    }

    protected override void OnPreRender(EventArgs e)
    {
        gridDocuments.ReloadData();
    }
}
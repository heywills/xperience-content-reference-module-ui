namespace KenticoCommunity.ContentReferenceUi.Constants
{
    public static class UiElementConstants
    {
        public const string ElementName = "KenticoCommunity.ContentUsage";
        public const string ElementDisplayName = "Usage";
        public const string ElementCaption = "Usage";
		public const bool ElementIsCustom = false;
		public const string ElementPageTemplateCodeName = "Listing";
		public const string ParentElementResourceName = "CMS.Content";
        public const string ParentElementName = "Edit";
        public const string ElementProperties =
@"<Data>
	<category_name_CodeBehind>False</category_name_CodeBehind>
	<category_name_Custom>False</category_name_Custom>
	<category_name_Javascript>False</category_name_Javascript>
	<category_name_SettingKeys>False</category_name_SettingKeys>
	<DisplayBreadcrumbs>False</DisplayBreadcrumbs>
	<EditInDialog>False</EditInDialog>
	<ExtenderClassName>KenticoCommunity.ContentReferenceUi.Extenders.ContentReferenceUniGridExtender</ExtenderClassName>
	<GridExtender>KenticoCommunity.ContentReferenceUi</GridExtender>
	<GridName>~/CMSModules/KenticoCommunity.ContentReferenceUi/Grid/Usage.xml</GridName>
	<OpenInDialog>False</OpenInDialog>
	<Text>Content usage</Text>
	<TitleText>Content usage</TitleText>
	<ZeroRowsText>This content is not referenced.</ZeroRowsText>
</Data>";

    }
}

namespace Elision.Rules
{
    public class RulesSettings
    {
        public string LocalDatasourceFolderPath
        {
            get
            {
                return Sitecore.Configuration.Settings.GetSetting(
                    "Elision.LocalDatasourceFolderQuery",
                    "./_components");
            }
        }

        public string LocalDatasourceTemplateId
        {
            get
            {
                return Sitecore.Configuration.Settings.GetSetting(
                    "Elision.LocalDatasourceFolderTemplateID",
                    "{122AE27A-D84F-4C5E-8367-0F42C764976E}");
            }
        }

        public bool LocalDatasourceFolderNesting
        {
            get
            {
                return Sitecore.Configuration.Settings.GetBoolSetting(
                    "Elision.LocalDatasourceFolderNesting", true);
            }
        }

        public string WebsiteDatasourceFolderPath
        {
            get
            {
                return Sitecore.Configuration.Settings.GetSetting(
                    "Elision.WebsiteDatasourceFolderQuery",
                    "./_components");
            }
        }

        public string WebsiteDatasourceTemplateId
        {
            get
            {
                return Sitecore.Configuration.Settings.GetSetting(
                    "Elision.WebsiteDatasourceFolderTemplateID",
                    "{122AE27A-D84F-4C5E-8367-0F42C764976E}");
            }
        }

        public bool WebsiteDatasourceFolderNesting
        {
            get
            {
                return Sitecore.Configuration.Settings.GetBoolSetting(
                    "Elision.WebsiteDatasourceFolderNesting", true);
            }
        }

        public string GlobalDatasourceFolderPath
        {
            get
            {
                return Sitecore.Configuration.Settings.GetSetting(
                    "Elision.GlobalDatasourceFolderQuery",
                    "./_components");
            }
        }

        public string GlobalDatasourceTemplateId
        {
            get
            {
                return Sitecore.Configuration.Settings.GetSetting(
                    "Elision.GlobalDatasourceFolderTemplateID",
                    "{122AE27A-D84F-4C5E-8367-0F42C764976E}");
            }
        }

        public bool GlobalDatasourceFolderNesting
        {
            get
            {
                return Sitecore.Configuration.Settings.GetBoolSetting(
                    "Elision.GlobalDatasourceFolderNesting", true);
            }
        }

        public string QueryTokenPrefix
        {
            get
            {
                return Sitecore.Configuration.Settings.GetSetting(
                    "Elision.QueryTokenPrefix", "{");
            }
        }

        public string QueryTokenSuffix
        {
            get
            {
                return Sitecore.Configuration.Settings.GetSetting(
                    "Elision.QueryTokenSufix", "}");
            }
        }
    }
}

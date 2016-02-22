<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BackupSecurity.aspx.cs" Inherits="Elision.Web.sitecore.admin.BackupSecurity" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Backup and Restore Item Security</title>
    <link rel="shortcut icon" href="/sitecore/images/favicon.ico" />
    <link rel="Stylesheet" type="text/css" href="/sitecore/shell/themes/standard/default/WebFramework.css" />

    <script type="text/javascript" src="/sitecore/shell/controls/lib/jQuery/jquery.js"></script>
    <script type="text/javascript" src="/sitecore/shell/controls/lib/jQuery/jquery.watermark.js"></script>
    <script type="text/javascript" src="/sitecore/shell/controls/webframework/webframework.js"></script>
    <style type="text/css">
        .error {
            color: red;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" class="wf-container">
        <asp:ScriptManager runat="server"></asp:ScriptManager>
        <asp:Timer ID="ProgressTimer" Interval="2000" runat="server" OnTick="ProgressTimer_Tick" Enabled="False"></asp:Timer>
        <div class="wf-content">
            <h1>Security Backup Utility</h1>
            <p class="wf-subtitle">Reset all content items to the final state of their default workflow</p>
            <asp:UpdatePanel runat="server">
                <ContentTemplate>
                    <div class="wf-configsection">
                        <h2><span>Backup Security</span></h2>
                        <p>
                            Stores a backup of the security for the entire sitecore tree. This file is stored in the data folder.
                        </p>
                        <p>
                            <asp:Button runat="server" ID="StartBackupButton" OnClick="StartBackup" Text="Start Backup" />
                        </p>
                    </div>
                    <div class="wf-configsection">
                        <h2><span>Restore Security</span></h2>
                        <p>
                            <asp:Label runat="server" AssociatedControlID="StartingItem" Text="Starting item:" ToolTip="This item and all of its descendants will be considered for update." />
                            <asp:TextBox runat="server" ID="StartingItem" Text="/sitecore" />
                        </p>
                        <p>
                            <asp:ListBox runat="server" ID="BackupFiles" DataValueField="Key" DataTextField="Value"></asp:ListBox>
                        </p>
                        <p>
                            <asp:CheckBox runat="server" ID="SkipPathIntegrityCheck" Text="Skip integrity check on item paths." TextAlign="Right" ToolTip="When checked, item security will be updated, even if the item has been moved to a new path." />
                        </p>
                        <p>
                            <asp:CheckBox runat="server" ID="PreviewOnly" Text="Preview what changes would be made, but do not modify any items." TextAlign="Right" ToolTip="" />
                        </p>
                        <p>
                            <asp:Button runat="server" ID="StartRestoreButton" OnClick="Start" Text="Start Restore" />
                        </p>
                    </div>
                    <div class="wf-configsection">
                        <h2><span>Status</span></h2>
                        <div>
                            <asp:Literal runat="server" ID="Output"></asp:Literal>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ProgressTimer" EventName="Tick" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </form>
</body>
</html>

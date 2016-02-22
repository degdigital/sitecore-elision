<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResetWorkflow.aspx.cs" Inherits="Elision.Web.sitecore.admin.ResetWorkflow" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reset Item Workflows</title>
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
            <h1>Workflow Reset Utility</h1>
            <p class="wf-subtitle">Reset all content items to the final state of their default workflow</p>
            <asp:UpdatePanel runat="server">
                <ContentTemplate>
                    <div class="wf-configsection">
                        <h2><span>Options</span></h2>
                        <p>
                            <asp:CheckBox runat="server" ID="IncludePending" Text="Include items pending approval" TextAlign="Right" ToolTip="Even items that are not in a final workflow state will be updated." />
                        </p>
                        <p>
                            <asp:CheckBox runat="server" ID="ClearWfWhenNoDefault" Text="Clear workflow when item has no default workflow" TextAlign="Right" ToolTip="If an item is in a workflow state, but does not have a default workflow, then the item's workflow state will be cleared." />
                        </p>
                        <p>
                            <asp:Label runat="server" AssociatedControlID="StartingItem" Text="Starting item:" ToolTip="This item and all of its descendants will be considered for update." />
                            <asp:TextBox runat="server" ID="StartingItem" Text="/sitecore/Content" />
                        </p>
                        <p>
                            <asp:CheckBox runat="server" ID="PreviewOnly" Text="Preview what changes would be made, but do not modify any items." TextAlign="Right" ToolTip="" />
                        </p>
                        <p>
                            <asp:Button runat="server" ID="StartButton" OnClick="Start" Text="Start" />
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

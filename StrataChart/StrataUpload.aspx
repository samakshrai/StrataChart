<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StrataUpload.aspx.cs" Inherits="StrataChart.StrataUpload" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <link href="Styles/StrataStyle.css" rel="stylesheet" />
    <link href="Content/site.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="form-container">
    <!-- Input Form -->
    <div class="input-card">
        <label>Site Name:</label>
        <asp:TextBox ID="txtSiteName" runat="server" />

        <label>Drilling Depth (m):</label>
        <asp:TextBox ID="txtDrillingDepth" runat="server" />

        <label>Logging Depth (m):</label>
        <asp:TextBox ID="txtLoggingDepth" runat="server" />

        <label>Blank Pipe Length (m):</label>
        <asp:TextBox ID="txtBlankPipeLength" runat="server" />

        <label>Screen Pipe Length (m):</label>
        <asp:TextBox ID="txtScreenPipeLength" runat="server" />

        <label>Bail Plug:</label>
        <asp:TextBox ID="txtBailPlug" runat="server" />

        <asp:FileUpload ID="FileUpload1" runat="server" CssClass="upload-btn" />
        <asp:Button ID="btnUpload" runat="server" Text="Generate" OnClick="btnUpload_Click" CssClass="upload-btn" />
        <asp:Label ID="lblMessage" runat="server" ForeColor="Red" />
    </div>

    <!-- Diagram Area -->
    <div class="diagram-area">
        <asp:Panel ID="strataPanel" runat="server" CssClass="strata-container"></asp:Panel>
    </div>
</div>
        <div class="legend-container">
    <div class="legend-item"><span class="legend-color clay"></span>Clay</div>
    <div class="legend-item"><span class="legend-color sand"></span>Sand</div>
    <div class="legend-item"><span class="legend-color gravel"></span>Gravel</div>
</div>
            </form>
</body>
</html>

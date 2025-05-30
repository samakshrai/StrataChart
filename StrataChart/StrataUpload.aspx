<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StrataUpload.aspx.cs" Inherits="StrataChart.StrataUpload" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>    
    .strata-container {
        position: relative;
        width: 200px;
        border-left: 3px solid black;
        margin: 20px;
        padding-left: 20px;
    }

    .strata-block {
        position: absolute;
        left: 100px;
        width: 120px;
        border: 1px solid #999;
        color: black;
        font-size: 10px;
        text-align: center;
        line-height: 1.2;
    }

    .label-text {
        position: absolute;
        left: 10px;
        width: 80px;
        font-size: 12px;
        color: #222;
        text-align: right;
        padding-right: 5px;
        overflow: hidden;
    }

    .depth-label {
        position: absolute;
        left: 230px;
        font-size: 12px;
        color: #555;
        width: 100px;
    }

    .clay {
        background: repeating-linear-gradient(
            -45deg,
            #d9c3a8,
            #d9c3a8 5px,
            #b49b7f 5px,
            #b49b7f 10px
        );
        background-color: #c97b44;
    }
    .sand {
        background: repeating-linear-gradient(
            45deg,
            #f2e3bc,
            #f2e3bc 5px,
            #dec9a5 5px,
            #dec9a5 10px
        );
        background-color: #f4e7b5;
    }
</style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:FileUpload ID="FileUpload1" runat="server" />
            <asp:Button ID="btnUpload" runat="server" Text="Upload Excel" OnClick="btnUpload_Click" />
            <asp:Label ID="lblMessage" runat="server" ForeColor="Red" />

            <asp:Panel ID="strataPanel" runat="server" CssClass="strata-container"></asp:Panel>

        </div>
    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="StrataVisual.aspx.cs" Inherits="StrataVisual" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Strata Chart Visualization</title>
    <style>
        .chart-container {
            width: 180px;
            border-left: 3px solid black;
            margin: 20px;
            position: relative;
        }

        .strata-block {
            width: 100%;
            border-bottom: 1px solid #000;
            color: black;
            text-align: center;
            font-size: 8px;
            line-height: normal;
            position: absolute; 
            width: 120px; border: 1px solid #999; 
            text-align: center; 
            line-height: 1.2; 
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
               
        .depth-label {
            position: absolute;
            left: -60px;
            width: 50px;
            text-align: right;
            font-size: 8px;
        }  
        .depth-label { position: absolute; left: 230px; font-size: 12px; color: #555; }

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
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h2>Strata Chart</h2>
        <div class="chart-container" runat="server" id="strataPanel">
        </div>
    </form>
</body>
</html>

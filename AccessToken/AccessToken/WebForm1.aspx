<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="AccessToken.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="Label3" runat="server" Text="OwinアクセストークンDecrypter" Font-Bold="True" Font-Size="Larger"></asp:Label>
        </div>
        <div>
            <asp:Label ID="Label4" runat="server" Text="Web.configのmachineKeyを合わせておくこと"></asp:Label>
        </div>
        <div>
            <asp:Label ID="Label1" runat="server" Text="AccessToken" Font-Size="Larger"></asp:Label>
        </div>
        <div>
            <asp:TextBox ID="TextBox1" runat="server" Height="89px" Width="1253px" TextMode="MultiLine"></asp:TextBox>
        </div>
        <div>
            <asp:Button ID="Button1" runat="server" Text="Decrypt" OnClick="Page_Load" />
        </div>
        <div>
            <asp:Label ID="Label2" runat="server" Text="Result" Font-Size="Larger"></asp:Label>
        </div>
        <div>
            <asp:TextBox ID="TextBox2" runat="server" Height="235px" Width="1253px" TextMode="MultiLine"></asp:TextBox>
        </div>
    </form>
</body>
</html>

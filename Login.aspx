<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SITConnect.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            Email Address&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="tb_email" runat="server"  TextMode="Email" ></asp:TextBox>
        <asp:Label ID="lbl_emailchecker" runat="server" Text="emailchecker"></asp:Label><br />

        Password&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="tb_password" runat="server"  TextMode="Password" onkeyup="javascript:validate()"></asp:TextBox>
        <asp:Label ID="lbl_pwdchecker" runat="server" Text="pwdchecker"></asp:Label><br />
        </div>
    </form>
</body>
</html>

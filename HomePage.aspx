<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HomePage.aspx.cs" Inherits="SITConnect.HomePage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
<div>
   <fieldset>
        <legend>HomePage</legend>
        <br />
        <asp:Label ID="lblMessage" runat="server" EnableViewState="false" />
       <br />
        <br />
        <a href="/duadsbuwbuiwbad"><p>404</p></a>
       <br />
        <br />
        <asp:Button ID="btnGeneric" runat="server" Text="GenericError" OnClick="InVokeGenericError" Visible="false" />
        <br />
        <br />
        <asp:Button ID="btnLogout" runat="server" Text="Logout" OnClick="Logout" Visible="false" />
        <p/>
    </fieldset>
</div>
</form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SITConnect.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="https://www.google.com/recaptcha/api.js?render=6LdDOW4eAAAAAAsnoTchYs8qKYMeBpNjr8fIjneH"></script>



</head>
<body>
    <form id="form1" runat="server">
        <div>
        Email Address&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="tb_email" runat="server"  TextMode="Email" ></asp:TextBox>

            <br />

        Password&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="tb_password" runat="server"  TextMode="Password" onkeyup="javascript:validate()"></asp:TextBox>
            <br />
            <br />

        </div>
        
        <asp:Button  ID="Button1" runat="server"  Text="Login" Width="437px" OnClick="btn_Submit_Click"  />


        <input type="hidden" id="g-recaptcha-response" name="g-recaptcha-response"/>

        <p>
        <asp:Label ID="error_msg" runat="server" Text=""></asp:Label>
        </p>

    </form>

    <script>
     grecaptcha.ready(function () {
         grecaptcha.execute('6LdDOW4eAAAAAAsnoTchYs8qKYMeBpNjr8fIjneH', { action: 'Login' }).then(function (token) {
            document.getElementById("g-recaptcha-response").value = token;
         });
     });
    </script>
</body>
</html>

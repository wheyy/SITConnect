<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePw.aspx.cs" Inherits="SITConnect.ChangePw" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
        <script type="text/javascript">
        function validate() {
            var str = document.getElementById('<%=tb_password.ClientID %>').value;

            if (str.length < 12) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password Length Must be at Least 12 Characters";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("too_short");
            }
            else if (str.search(/[0-9]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password requires at least 1 number";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_number");
            }
            else if (str.search(/[A-Z]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password requires an upper case character";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_upper");
            }
            else if (str.search(/[a-z]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password requires a lower case character";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_lower");
            }
            else if (str.search(/[^a-zA-Z0-9]/) == -1) {
                document.getElementById("lbl_pwdchecker").innerHTML = "Password requires one special character";
                document.getElementById("lbl_pwdchecker").style.color = "Red";
                return ("no_special");
            }

            document.getElementById("lbl_pwdchecker").innerHTML = "Excellent"
            document.getElementById("lbl_pwdchecker").style.color = "Blue"
        }

        </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            Email Address&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="tb_email" runat="server"  TextMode="Email" ></asp:TextBox>
        <asp:Label ID="lbl_emailchecker" runat="server" Text=""></asp:Label>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" ControlToValidate="tb_email" runat="server" ErrorMessage="Please enter your Email" Style="color: red"></asp:RequiredFieldValidator>
        <br />
            Password&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="tb_password" runat="server"  TextMode="Password" onkeyup="javascript:validate()"></asp:TextBox>
        <asp:Label ID="lbl_pwdchecker" runat="server" Text="pwdchecker"></asp:Label><br />
        </div>
        <asp:Button ID="Button1" runat="server"  Text="Register" Width="437px" OnClick="btn_Submit_Click"  />
    </form>
</body>
</html>

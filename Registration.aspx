<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="SITConnect.Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>My Registration</title>

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
            Registration<br />
        </div>
        <br /><br />

        First Name&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="tb_fname" runat="server"  TextMode="SingleLine" ></asp:TextBox> 
        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="tb_fname" runat="server" ErrorMessage="Please enter your first name" Style="color: red"></asp:RequiredFieldValidator>

        <br />

        Last Name&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="tb_lname" runat="server"  TextMode="SingleLine" ></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="tb_lname" runat="server" ErrorMessage="Please enter your last name" Style="color: red"></asp:RequiredFieldValidator>


        <br />


        Credit Card Info&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="tb_cardInfo" runat="server"  TextMode="SingleLine" ></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="tb_cardInfo" runat="server" ErrorMessage="Please enter your credit card information" Style="color: red"></asp:RequiredFieldValidator>
        <asp:RegularExpressionValidator ID="regCard" ControlToValidate="tb_cardInfo" Text="Sorry, we only accept VISA card numbers (without spaces)" ValidationExpression="^4[0-9]{12}(?:[0-9]{3})?$" runat="server" Style="color: red" />

        

        <br />

        Email Address&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="tb_email" runat="server"  TextMode="Email" ></asp:TextBox>
        <asp:Label ID="lbl_emailchecker" runat="server" Text=""></asp:Label>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" ControlToValidate="tb_email" runat="server" ErrorMessage="Please enter your Email" Style="color: red"></asp:RequiredFieldValidator>
        <br />

        Password&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="tb_password" runat="server"  TextMode="Password" onkeyup="javascript:validate()"></asp:TextBox>
        <asp:Label ID="lbl_pwdchecker" runat="server" Text="pwdchecker"></asp:Label><br />

        Date of Birth&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="tb_dob" runat="server"  TextMode="Date" ></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" ControlToValidate="tb_dob" runat="server" ErrorMessage="Please enter your Date of birth" Style="color: red"></asp:RequiredFieldValidator>

        <br />

        Photo&nbsp;&nbsp;&nbsp;
        <asp:FileUpload ID="fu_photo" runat="server" ></asp:FileUpload><br />
        <asp:Label ID="lbl_photo" runat="server" Text=""></asp:Label><br />


        <p>
            <asp:Button ID="Button1" runat="server"  Text="Register" Width="437px" OnClick="btn_Submit_Click"  />
        </p>
    </form>

</body>
</html>

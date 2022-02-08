using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;


namespace SITConnect
{
    public partial class Registration : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(MYDBConnectionString);


        }

        protected void btn_checkPassword_Click(int score)
        {
            // implement codes for the button event
            // Extract data from textbox
            int pwdScore = score;
            string pwdStrength = "";
            switch (pwdScore)
            {
                case 1:
                    pwdStrength = "Very Weak";
                    break;
                case 2:
                    pwdStrength = "Weak";
                    break;
                case 3:
                    pwdStrength = "Medium";
                    break;
                case 4:
                    pwdStrength = "Strong";
                    break;
                case 5:
                    pwdStrength = "Very Strong";
                    break;
                default:
                    break;
            }
            lbl_pwdchecker.Text = "Status : " + pwdStrength;
            if (pwdScore < 4)
            {
                lbl_pwdchecker.ForeColor = Color.Red;
                return;
            } 
            lbl_pwdchecker.ForeColor = Color.Green; 
            
        }


        private int checkPassword(string pwd)
        {
            int pwdScore = 0;

            if (pwd.Length < 12)
            {
                return 1;
            }

            pwdScore = 1;

            if (Regex.IsMatch(pwd, "[a-z]"))
            {
                pwdScore++;
            }

            if (Regex.IsMatch(pwd, "[A-Z]"))
            {
                pwdScore++;
            }

            if (Regex.IsMatch(pwd, "[0-9]"))
            {
                pwdScore++;
            }

            if (Regex.IsMatch(pwd, "[^a-zA-z0-9]"))
            {
                pwdScore++;
            }


            return pwdScore;
        }

        protected void createAccount()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDBConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Users VALUES(@FirstName,@LastName,@CreditCardInfo,@Email,@PwdHash,@PwdSalt,@DOB,@Photo,@EmailVerified) "))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            string sql = "select Email FROM Account WHERE Email=@EMAIL";
                            SqlCommand command = new SqlCommand(sql, con);
                            command.Parameters.AddWithValue("@EMAIL", tb_email);

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if (reader["Email"] != null)
                                    {
                                        lbl_emailchecker.Text = "This email has already been registered!";
                                        lbl_emailchecker.ForeColor = Color.Red;
                                        return;
                                    }
                                       
                                }
                            }

                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Email", tb_email.Text.Trim());
                            cmd.Parameters.AddWithValue("@FirstName", tb_fname.Text.Trim());
                            //cmd.Parameters.AddWithValue("@Nric", encryptData(tb_nric.Text.Trim()));
                            cmd.Parameters.AddWithValue("@LastName", tb_lname.Text.Trim());
                            cmd.Parameters.AddWithValue("@CreditCardInfo", encryptData(tb_cardInfo.Text.Trim()));
                            cmd.Parameters.AddWithValue("@PwdHash", finalHash);
                            cmd.Parameters.AddWithValue("@PwdSalt", salt);
                            cmd.Parameters.AddWithValue("@DOB", tb_dob.Text.Trim());
                            if (fu_photo.HasFile)
                            {

                                cmd.Parameters.AddWithValue("@Photo", fu_photo.FileContent);
                            }
                            else
                            {
                                lbl_photo.Text = "No Images Uploaded.";
                                return;
                            }
                            cmd.Parameters.AddWithValue("@EmailVerified", DBNull.Value);
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select Email FROM Users WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", tb_email.Text.Trim());
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Email"] != null)
                        {
                            lbl_emailchecker.Text = "This email has already been registered!";
                            lbl_emailchecker.ForeColor = Color.Red;
                            return;
                        }

                    }      
                    
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            string pwd = tb_password.Text.ToString().Trim();
            int pwdScore = checkPassword(pwd);
            btn_checkPassword_Click(pwdScore);
            if (pwdScore > 4)
            {
                //Generate random "salt"
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] saltByte = new byte[8];
                //Fills array of bytes with a cryptographically strong sequence of random values.
                rng.GetBytes(saltByte);
                salt = Convert.ToBase64String(saltByte);
                SHA512Managed hashing = new SHA512Managed();
                string pwdWithSalt = pwd + salt;
                byte[] plainHash = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                finalHash = Convert.ToBase64String(hashWithSalt);
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.GenerateKey();
                Key = cipher.Key;
                IV = cipher.IV;
                createAccount();
            }
        }

        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                //ICryptoTransform decryptTransform = cipher.CreateDecryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0,
               plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }



    }

    

}


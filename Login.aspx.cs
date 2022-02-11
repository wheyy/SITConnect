using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.Services;
namespace SITConnect
{
    public partial class Login : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;
        public string success { get; set; }
        int noOfFailedAttempts;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ValidateCaptcha = notyet");
            if (ValidateCaptcha())
            {
                System.Diagnostics.Debug.WriteLine("ValidateCaptcha = true");

                string pwd = tb_password.Text.ToString().Trim();
                string email = tb_email.Text.ToString().Trim();
                SHA512Managed hashing = new SHA512Managed();
                string dbHash = getDBHash(email);
                string dbSalt = getDBSalt(email);

                Boolean accIsLocked = false;
                TimeSpan accLockOutTimeSpan = TimeSpan.Zero;
                SqlConnection connection = new SqlConnection(MYDBConnectionString);
                string sql = "select LockedDateTime,Locked FROM Users WHERE Email=@EMAIL";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@EMAIL", email);
                try
                {
                    System.Diagnostics.Debug.WriteLine("Hi i entered the try loop for reader");
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["Locked"] != DBNull.Value)
                            {
                                System.Diagnostics.Debug.WriteLine(reader["Locked"].ToString());
                                if (Convert.ToBoolean(reader["Locked"].ToString()))
                                {
                                    accIsLocked = true;
                                    if (reader["LockedDateTime"] != DBNull.Value)
                                    {
                                        accLockOutTimeSpan = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyy HH:mm:ss")).Subtract(Convert.ToDateTime(reader["LockedDateTime"].ToString()));
                                        System.Diagnostics.Debug.WriteLine(accLockOutTimeSpan.Minutes);
                                        if (accLockOutTimeSpan.Minutes >= 1)
                                        {
                                            changeLocked(false);
                                            changeFailedAttempts(0);
                                            accIsLocked = false;
                                        } else
                                        {
                                            accIsLocked = true;
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
                finally { connection.Close(); }
                if (!accIsLocked)
                {
                    try
                    {
                        if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                        {
                            string pwdWithSalt = pwd + dbSalt;
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                            string userHash = Convert.ToBase64String(hashWithSalt);


                            if (userHash.Equals(dbHash))
                            {
                                changeFailedAttempts(0);
                                setAuditLog("Log in", email);

                                Session["LoggedIn"] = email;

                                // createa a new GUID and save into the session
                                string guid = Guid.NewGuid().ToString();
                                Session["AuthToken"] = guid;
                                // now create a new cookie with this guid value
                                Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                                Response.Redirect("HomePage.aspx", false);
                            }
                            else
                            {
                                noOfFailedAttempts = checkFailedAttempts(email) + 1;


                                if (noOfFailedAttempts == 3)
                                {
                                    error_msg.Text = $"Your account will be locked out for 1min";
                                    error_msg.ForeColor = System.Drawing.Color.Red;
                                    changeLocked(true);
                                    return;

                                }
                                else if(noOfFailedAttempts < 3)
                                {
                                    changeFailedAttempts(noOfFailedAttempts);
                                    error_msg.Text = $"Email or password is not valid. Please try again. You have {3 - noOfFailedAttempts} attempts left before your account locks out";
                                    error_msg.ForeColor = System.Drawing.Color.Red;
                                    return;
                                }
                                Response.Redirect("Login.aspx", false);


                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.ToString());
                    }
                    finally { }
                }
                else
                {
                    error_msg.Text = $"Your account will be locked out for another {60 - Convert.ToInt16(accLockOutTimeSpan.Seconds)}seconds";
                    error_msg.ForeColor = System.Drawing.Color.Red;
                }
            }
        }


        protected string getDBHash(string email)
        {
            string h = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PwdHash FROM Users WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (reader["PwdHash"] != DBNull.Value)
                        {
                            h = reader["PwdHash"].ToString();
                        }
                    
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return h;
        }


        
        protected string getDBSalt(string email)
        {
            string s = null;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select PwdSalt FROM Users WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PwdSalt"] != null)
                        {
                            if (reader["PwdSalt"] != DBNull.Value)
                            {
                                s = reader["PwdSalt"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return s;
        }

        public bool ValidateCaptcha()
        {
            bool result = true;

            string captchaResponse = Request.Form["g-recaptcha-response"];
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
                ("https://www.google.com/recaptcha/api/siteverify?secret=6LdDOW4eAAAAAJMIOBPwQOPvbY16AWzqLgxe9IM6 &response=" + captchaResponse);

            try
            {
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        string jsonResponse = readStream.ReadToEnd();

                        var gScore = jsonResponse.ToString();
                        System.Diagnostics.Debug.WriteLine(gScore);
                        JavaScriptSerializer js = new JavaScriptSerializer();

                        Login jsonobject = js.Deserialize<Login>(jsonResponse);
                        result = Convert.ToBoolean(jsonobject.success);//
                    }
                }
                return result;
            } 
            catch (WebException ex)
            {
                throw ex;
            }
        } 
        
        void changeLocked(bool boolean)
        {

            if (boolean)
            {
                SqlConnection con = new SqlConnection(MYDBConnectionString);
                string format = "MM/dd/yyy HH:mm:ss";
                string sql = "Update Users set Locked=1, LockedDateTime='" + DateTime.Now.ToString(format) + "' WHERE EMAIL = '" + tb_email.Text.ToString().Trim() + "'";
                changeFailedAttempts(3);
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            } else
            {
                SqlConnection con = new SqlConnection(MYDBConnectionString);
                string format = "MM/dd/yyy HH:mm:ss";
                string sql = "Update Users set Locked=0, Lockeddatetime=null WHERE EMAIL = '" + tb_email.Text.ToString().Trim() + "'";
                changeFailedAttempts(0);
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

     

        void changeFailedAttempts(int i)
        {

            SqlConnection con = new SqlConnection(MYDBConnectionString);
            string format = "MM/dd/yyy HH:mm:ss";
            string sql = "Update Users set FailedAttempts='" + i + "' WHERE EMAIL = '" + tb_email.Text.ToString().Trim() + "'";
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

                    
        }

        public int checkFailedAttempts(string email)
        {
            int i = 0;
            SqlConnection connection = new SqlConnection(MYDBConnectionString);
            string sql = "select FailedAttempts FROM Users WHERE Email=@EMAIL";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EMAIL", email);

            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader["FailedAttempts"] != null)
                    {
                        if (reader["FailedAttempts"] != DBNull.Value)
                        {
                            i = Convert.ToInt32(reader["FailedAttempts"].ToString());
                            return i;
                        }
                    }
                }

            }

            return 0;

        }

        void setAuditLog(string auditAction, string email)
        {
            using (SqlConnection con = new SqlConnection(MYDBConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO AuditLog VALUES(@Email,@Action,@AuditDateTime) "))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {


                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Action", auditAction);
                        cmd.Parameters.AddWithValue("@AuditDateTime", DateTime.Now);

                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
        }

    }
}
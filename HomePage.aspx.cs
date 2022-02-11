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

namespace SITConnect
{
    public partial class HomePage : System.Web.UI.Page
    {
        string MYDBConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYDBConnection"].ConnectionString;
        string email;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedIn"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    email = (string)Session["LoggedIn"];
                    lblMessage.Text = "Welcome!, you are logged in as: " + email;
                    lblMessage.ForeColor = System.Drawing.Color.Green;
                    btnLogout.Visible = true;
                    btnGeneric.Visible = true;
                    System.Diagnostics.Debug.WriteLine("This is the email i got: " + email);
                }
                else
                {
                    Response.Redirect("Login.aspx", false);

                }
            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }
        }

        protected void Logout(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            Response.Redirect("Login.aspx", false);

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }
            
            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = string.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }

            
            setAuditLog("Log out", email);
        }

        protected void InVokeGenericError(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(MYDBConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO AuditLog VALUES(@Email,@Action,@AuditDateTime) "))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {

                        string email0 = null;
                        string auditAction0 = null;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Email", email0);
                        cmd.Parameters.AddWithValue("@Action", auditAction0);
                        cmd.Parameters.AddWithValue("@AuditDateTime", DateTime.Now);

                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
        }
        

        void setAuditLog(string auditAction, string email0)
        {
            using (SqlConnection con = new SqlConnection(MYDBConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO AuditLog VALUES(@Email,@Action,@AuditDateTime) "))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {


                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@Email", email0);
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
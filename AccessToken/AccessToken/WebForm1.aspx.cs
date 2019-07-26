using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AccessToken
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var token = TextBox1.Text;
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            // Tokenはこれで解析できる
            var dec = new TokenDecrypt();
            if( dec.Decrypt(token) == false )
            {
                TextBox2.Text = "Decrypt Error";
                return;
            }

            string result = "Decrypt Success!\r\n\r\n";
            result = result + "トークン発行日時 : " + dec.DateTimeIssued.ToString() + "\r\n";
            result = result + "トークン失効日時 : " + dec.DateTimeExpires.ToString() + "\r\n";
            result = result + "IdentityName : " + dec.IdentityName.ToString() + "\r\n";

            result = result + $"Roles({dec.Roles.Count}) : \r\n";
            foreach (var r in dec.Roles)
            {
                result = result + "- " + r + "\r\n";
            }

            result = result + $"Claims({dec.Claims.Count}) : \r\n";
            foreach (var c in dec.Claims)
            {
                result = result + $"- Type={c.Type} , Value={c.Value}" + "\r\n";
            }

            TextBox2.Text = result;

        }
    }
}
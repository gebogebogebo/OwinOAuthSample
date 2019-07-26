using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AccessTokenDesktop
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try {
                string token = TextBox1.Text;
                TokenDecrypt.TokenType type = TokenDecrypt.TokenType.AccessToken;
                if (RadioRefresh.IsChecked.Value) {
                    type = TokenDecrypt.TokenType.RefreshToken;
                }

                // Tokenはこれで解析できる
                var dec = new TokenDecrypt(type);
                if (dec.Decrypt(token) == false) {
                    TextBox2.Text = "Decrypt Error";
                    return;
                }

                string result = "Decrypt Success!\r\n\r\n";
                result = result + "トークン発行日時 : " + dec.DateTimeIssued.ToString() + "\r\n";
                result = result + "トークン失効日時 : " + dec.DateTimeExpires.ToString() + "\r\n";
                result = result + "IdentityName : " + dec.IdentityName.ToString() + "\r\n";

                result = result + $"Roles({dec.Roles.Count}) : \r\n";
                foreach (var r in dec.Roles) {
                    result = result + "- " + r + "\r\n";
                }

                result = result + $"Claims({dec.Claims.Count}) : \r\n";
                foreach (var c in dec.Claims) {
                    result = result + $"- Type={c.Type} , Value={c.Value}" + "\r\n";
                }

                TextBox2.Text = result;

            } catch {
                TextBox2.Text = "Decrypt Error";
            }

        }
    }
}

using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace AccessTokenDesktop
{
    public class TokenDecrypt
    {
        public enum TokenType
        {
            AccessToken,
            RefreshToken,
        }

        public class Claim
        {
            public string Type;
            public string Value;
        }

        public DateTime DateTimeIssued { get; private set; }
        public DateTime DateTimeExpires { get; private set; }

        public string IdentityName { get; private set; }
        public List<string> Roles { get; private set; }
        public List<TokenDecrypt.Claim> Claims { get; private set; }

        public TokenType Type { get; private set; }
        public TokenDecrypt(TokenType type)
        {
            this.Type = type;
        }

        // OWINで生成したアクセストークンを復号する
        // https://long2know.com/2015/05/decrypting-owin-authentication-ticket/
        public bool Decrypt(string accessToken)
        {
            IDataProtector protector = null;
            if( this.Type == TokenType.AccessToken) {
                protector = new AccessTokenMachineKeyProtector();
            } else if(this.Type == TokenType.RefreshToken){
                protector = new RefreshTokenMachineKeyProtector();
            }

            // Decrypt
            var secureDataFormat = new TicketDataFormat(protector);
            AuthenticationTicket ticket = secureDataFormat.Unprotect(accessToken);
            if( ticket == null) {
                return false;
            }

            {
                var jstTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
                DateTimeIssued = System.TimeZoneInfo.ConvertTimeFromUtc(ticket.Properties.IssuedUtc.Value.DateTime, jstTimeZoneInfo);
                DateTimeExpires = System.TimeZoneInfo.ConvertTimeFromUtc(ticket.Properties.ExpiresUtc.Value.DateTime, jstTimeZoneInfo);
            }

            // get identity
            var identity = ticket.Identity as ClaimsIdentity;
            var roleClaims = identity.Claims.Where(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Select(x => x.Value).ToList();
            var nonRoleClaims = identity.Claims.Where(x => x.Type != ClaimsIdentity.DefaultRoleClaimType).Select(x => new { Type = x.Type, Value = x.Value }).ToList();

            IdentityName = identity.Name;
            Roles = roleClaims;

            Claims = new List<Claim>();
            foreach ( var c in nonRoleClaims)
            {
                var setc = new TokenDecrypt.Claim();
                setc.Type = c.Type;
                setc.Value = c.Value;
                Claims.Add(setc);
            }

            return true;
        }

        /// <summary>
        /// Helper method to decrypt the OWIN ticket
        /// </summary>
        private class AccessTokenMachineKeyProtector : IDataProtector
        {
            private readonly string[] _purpose =
            {
                typeof(OAuthAuthorizationServerMiddleware).Namespace,
                "Access_Token",
                "v1"
            };

            private readonly string[] _purposeRefreshToken =
            {
                typeof(OAuthAuthorizationServerMiddleware).Namespace,
                "Refresh_Token",
                "v1"
            };

            public byte[] Protect(byte[] userData)
            {
                throw new NotImplementedException();
            }

            public byte[] Unprotect(byte[] protectedData)
            {
                // 復号する
                // MachineKey＝復号するキー（暗号化するときと同じ値を指定する必要がある）
                // Web.config で MachineKey を明示的に指定していない場合には自動生成される
                return System.Web.Security.MachineKey.Unprotect(protectedData, _purpose);
            }
        }

        private class RefreshTokenMachineKeyProtector : IDataProtector
        {
            private readonly string[] _purpose =
            {
                typeof(OAuthAuthorizationServerMiddleware).Namespace,
                "Refresh_Token",
                "v1"
            };

            public byte[] Protect(byte[] userData)
            {
                throw new NotImplementedException();
            }

            public byte[] Unprotect(byte[] protectedData)
            {
                return System.Web.Security.MachineKey.Unprotect(protectedData, _purpose);
            }
        }

    }
}
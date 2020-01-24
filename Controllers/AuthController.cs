using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel;
using System.Text.Unicode;

namespace JWT.Controllers
{
    [Route("api/[controller]")]
    // [ApiController]
    public class AuthController //: ControllerBase
    {
        public AuthController(JwtBearerHandler bearerHandler)
        {
        }

        public static SecurityKey SymetricSecurityKey
        {
            get
            {
                var key = "myPassmyPassmyPassmyPassmyPassmyPassmyPassmyPassmyPass";
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                return securityKey;
            }
        }

        public static SecurityKey AsymetricSecurityKey()
        {
            var n =
                "tXqCj19l6Min6hkIf2mJjZGtbQPTSMf2looetVWqTCiaYLrX11YL4TdQzY8wzsL3vFNyinD33A6Y6zh04hDNDuoGaa3ybewMNqK-wm1jEdr253hkfnXqz_RCrmQfaBksXwBLRt27dJgDycpRH-zwZWS5xzNuyfoocoW7CBYYD-B8KXGzX2X9_c30qqciZSJck4XLv1z2ZxRBjWJzDk-PvUm1GfwsVeC9atOZ8ri3PKLPSQAMJhlQcCsYTjlbvOSPkMx3ZQYLeJO5KesqkGA-jp_NMvz4Pry-RMOM5X4RiCPzRtwJGN5N10kMrrqzXl1i8wHV1lO44l3oKcYvM4aowQ";
            var e = "AQAB";
           var nDecoded= IdentityModel.Base64Url.Decode(n);
           var eDecoded = IdentityModel.Base64Url.Decode(e);
            SecurityKey securityKey;
            var rsa = new RsaSecurityKey(RSA.Create());
            var rsa2 = new RsaSecurityKey(new RSAParameters()
            {
                Modulus = nDecoded,
                Exponent = eDecoded

            });
          

            return rsa2;
        }

        [HttpGet("token")]
        public ActionResult<string> Token()
        {
            var signingCredentials = new SigningCredentials(SymetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            var jsonWebToken = new JwtSecurityToken(signingCredentials: signingCredentials, issuer: "vit",
                expires: DateTime.Now.AddHours(1));


            var claimsIdentity = new ClaimsIdentity(new List<Claim>() {new Claim(ClaimTypes.Role, "admin")});

            var aToken =
                new JwtSecurityTokenHandler().CreateJwtSecurityToken(signingCredentials: signingCredentials,
                    audience: "vit",subject:claimsIdentity, issuer:"vit");

            aToken.Claims.Append(new Claim(ClaimTypes.Role, "admin"));
        

            var jwt = new JwtSecurityTokenHandler().WriteToken(aToken);
            return jwt;

        }
    }
}
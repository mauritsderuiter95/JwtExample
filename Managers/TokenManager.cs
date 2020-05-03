using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using JwtExample.Models;
using JWT.Builder;
using JWT.Algorithms;
using System.Text;

namespace JwtExample.Managers
{
    public static class TokenManager
    {
        private static readonly string _secret = "Superlongsupersecret!";

        public static string GenerateAccessToken(User user)
        {
            return new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(Encoding.ASCII.GetBytes(_secret))
                .AddClaim("exp", DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds())
                .AddClaim("username", user.Username)
                .Issuer("JwtExample")
                .Audience("access")
                .Encode();
        }

        public static (string refreshToken, string jwt) GenerateRefreshToken(User user)
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create()){
                rng.GetBytes(randomNumber);
                Convert.ToBase64String(randomNumber);
            }

            var randomString = System.Text.Encoding.ASCII.GetString(randomNumber);

            string jwt = new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(_secret)
                .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(4).ToUnixTimeSeconds())
                .AddClaim("refresh", randomString)
                .AddClaim("username", user.Username)
                .Issuer("JwtExample")
                .Audience("refresh")
                .Encode();

            return (randomString, jwt);
        }

        public static IDictionary<string, object> VerifyToken(string token)
        {
            return new JwtBuilder()
                 .WithSecret(_secret)
                 .MustVerifySignature()
                 .Decode<IDictionary<string, object>>(token);
        }
    }
}
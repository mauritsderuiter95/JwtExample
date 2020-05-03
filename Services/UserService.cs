using JwtExample.Models;
using JwtExample.Managers;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace JwtExample.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>("users");
        }

        public List<User> Get() =>
            _users.Find(user => true).ToList();

        public User Get(string id) =>
            _users.Find<User>(user => user.Id == id).FirstOrDefault();

        public User Create(User user)
        {
            _users.InsertOne(user);
            return user;
        }

        public Tokens Login(Authentication authentication) {
            User user = _users.Find<User>(u => u.Username == authentication.Username).FirstOrDefault();

            bool validPassword = user.Password == authentication.Password;

            if (validPassword) {
                var refreshToken = TokenManager.GenerateRefreshToken(user);

                if (user.RefreshTokens == null)
                    user.RefreshTokens = new List<string>();

                user.RefreshTokens.Add(refreshToken.refreshToken);

                _users.ReplaceOne(u => u.Id == user.Id, user);

                return new Tokens
                {
                    AccessToken = TokenManager.GenerateAccessToken(user),
                    RefreshToken = refreshToken.jwt
                };
            } 
            else {
                throw new System.Exception("Username or password incorrect");
            }
        }

        public Tokens Refresh(Claim userClaim, Claim refreshClaim) {
            User user = _users.Find<User>(x => x.Username == userClaim.Value).FirstOrDefault();

            if (user == null) 
                throw new System.Exception("User doesn't exist");

            if (user.RefreshTokens == null)
                    user.RefreshTokens = new List<string>();

            string token = user.RefreshTokens.FirstOrDefault(x => x == refreshClaim.Value);

            if (token != null) {
                var refreshToken = TokenManager.GenerateRefreshToken(user);

                user.RefreshTokens.Add(refreshToken.refreshToken);

                user.RefreshTokens.Remove(token);

                _users.ReplaceOne(u => u.Id == user.Id, user);

                return new Tokens
                {
                    AccessToken = TokenManager.GenerateAccessToken(user),
                    RefreshToken = refreshToken.jwt
                };
            }
            else {
                throw new System.Exception("Refresh token incorrect");
            }
        }

        public void Update(string id, User userIn) =>
            _users.ReplaceOne(user => user.Id == id, userIn);

        public void Remove(User userIn) =>
            _users.DeleteOne(user => user.Id == userIn.Id);

        public void Remove(string id) =>
            _users.DeleteOne(user => user.Id == id);
    }
}
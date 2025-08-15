using Fartak.DbModels;
using Fartak.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Fartak.Services
{
    public class Me : IMe
    {
        private readonly DataBase context;

        public Me(DataBase context)
        {
            this.context = context;
        }

        public User UserIsConnected(string token) {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var claims = jwtToken.Claims.ToList();
            var tokenInfo = new
            {
                Email = claims[0].Value,
                UserId = claims[1].Value
            };

            var user = context.Users.FirstOrDefault(i => i.Id == Convert.ToInt32(tokenInfo.UserId));
            if (user != null) {
                return user;
            }
            return null;
        }

        public User UserIsConnected()
        {
            throw new NotImplementedException();
        }
    }
}

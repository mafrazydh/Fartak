using Fartak.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Fartak.Services
{
    public class GetAdmin : IGetAdmin
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataBase _context;
        private readonly IMe me;

        public GetAdmin(IHttpContextAccessor httpContextAccessor, DataBase context, IMe me)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            this.me = me;
        }

        public bool IsAdmin()
        {
            var token = _httpContextAccessor.HttpContext.Request.Cookies["jwt"];
            var user = me.UserIsConnected(token);

            if (user == null) {
                return false;
            }
            
            return user.IsAdmin;
        }
    }
}

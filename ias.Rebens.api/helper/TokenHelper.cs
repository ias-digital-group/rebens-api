using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ias.Rebens.api
{
    public class TokenHelper
    {
        public static string GetCurrentUser(IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            var claims = claimsIdentity?.Claims;
            if (claims == null || !claims.Any())
                return null;

            var claimOperationId = claims.FirstOrDefault(c => c.Type == "operationId");
            var claimId = claims.FirstOrDefault(c => c.Type == "Id");

            string ret = "";
            if (claimId != null)
                ret = claimId.Value;

            if (claimOperationId != null)
                ret += "|" + claimOperationId.Value;

            return ret;

            //return new User
            //{
            //    Username = claimsIdentity.Name,
            //    ID = new Guid(claimId.Value),
            //    EcommerceDatabaseName = claimDbName.Value
            //};
        }
    }
}

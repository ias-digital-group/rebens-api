using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ias.Rebens.api.Controllers
{
    public class BaseApiController : ControllerBase
    {
        public int GetAdminUserId(out string error)
        {
            int ret = 0;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var tempId = principal.Claims.SingleOrDefault(c => c.Type == "id");
                if (tempId == null)
                    error = "Usuário não encontrado!";
                if (int.TryParse(tempId.Value, out int tmpId))
                {
                    ret = tmpId;
                    error = null;
                }
                else
                    error = "Usuário não encontrado!";
            }
            else
                error = "Usuário não encontrado!";
            return ret;
        }

        public int GetOperationId(out string error)
        {
            int ret = 0;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var tempId = principal.Claims.SingleOrDefault(c => c.Type == "operationId");
                if (tempId == null)
                    error = "Operação não encontrada!";
                if (int.TryParse(tempId.Value, out int tmpId))
                {
                    ret = tmpId;
                    error = null;
                }
                else
                    error = "Operação não encontrada!";
            }
            else
                error = "Operação não encontrada!";
            return ret;
        }

        public int GetOperationPartnerId(out string error)
        {
            int ret = 0;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var tempId = principal.Claims.SingleOrDefault(c => c.Type == "operationPartnerId");
                if (tempId == null)
                    error = "Parceiro não encontrado!";
                if (int.TryParse(tempId.Value, out int tmpId))
                {
                    ret = tmpId;
                    error = null;
                }
                else
                    error = "Parceiro não encontrado!";
            }
            else
                error = "Parceiro não encontrado!";
            return ret;
        }

        public string GetAdminUserName(out string error)
        {
            string ret = "";
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var temp = principal.Claims.SingleOrDefault(c => c.Type == "name");
                if (temp == null)
                    error = "Usuário não encontrado!";
                else
                {
                    ret = temp.Value;
                    error = null;
                }
                
            }
            else
                error = "Usuário não encontrado!";
            return ret;
        }

        public string GetAdminUserSurname(out string error)
        {
            string ret = "";
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var temp = principal.Claims.SingleOrDefault(c => c.Type == "surname");
                if (temp == null)
                    error = "Usuário não encontrado!";
                else
                {
                    ret = temp.Value;
                    error = null;
                }

            }
            else
                error = "Usuário não encontrado!";
            return ret;
        }

        public string GetRole(out string error)
        {
            string role = "";
            var uRole = HttpContext.User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role);
            if (uRole != null)
            {
                role = uRole.Value;
                error = null;
            }
            else
                error = "Você não tem acesso a essa funcionalidade!";
            return role;
        }

        public int GetCustomerId(out string error)
        {
            int ret = 0;
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var tempId = principal.Claims.SingleOrDefault(c => c.Type == "Id");
                if (tempId == null)
                    error = "Cliente não encontrado!";
                if (int.TryParse(tempId.Value, out int tmpId))
                {
                    ret = tmpId;
                    error = null;
                }
                else
                    error = "Cliente não encontrado!";
            }
            else
                error = "Cliente não encontrado!";
            return ret;
        }

        public string GetCustomerName(out string error)
        {
            string ret = "";
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var tempId = principal.Claims.SingleOrDefault(c => c.Type == "Name");
                if (tempId == null)
                    error = "Cliente não encontrado!";
                else
                {
                    ret = tempId.Value;
                    error = null;
                }
            }
            else
                error = "Cliente não encontrado!";
            return ret;
        }

        public string GetCustomerEmail(out string error)
        {
            string ret = "";
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var tempId = principal.Claims.SingleOrDefault(c => c.Type == "Email");
                if (tempId == null)
                    error = "Cliente não encontrado!";
                else
                {
                    ret = tempId.Value;
                    error = null;
                }
            }
            else
                error = "Cliente não encontrado!";
            return ret;
        }

        public string GetCustomerStatus(out string error)
        {
            string ret = "";
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var tempId = principal.Claims.SingleOrDefault(c => c.Type == "Status");
                if (tempId == null)
                    error = "Cliente não encontrado!";
                else
                {
                    ret = tempId.Value;
                    error = null;
                }
            }
            else
                error = "Cliente não encontrado!";
            return ret;
        }

        public string GetCustomerCpf(out string error)
        {
            string ret = "";
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                var tempId = principal.Claims.SingleOrDefault(c => c.Type == "cpf");
                if (tempId == null)
                    error = "Cliente não encontrado!";
                else
                {
                    ret = tempId.Value;
                    error = null;
                }
            }
            else
                error = "Cliente não encontrado!";
            return ret;
        }

        public bool CheckRoles(string[] roles)
        {
            bool ret = false;
            foreach(string role in roles)
            {
                if(HttpContext.User.IsInRole(role))
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        public bool CheckRole(string role)
        {
            return HttpContext.User.IsInRole(role);
        }
    }
}

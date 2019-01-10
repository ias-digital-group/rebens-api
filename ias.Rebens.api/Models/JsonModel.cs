using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Modelo de resposta Json
    /// </summary>
    public class JsonModel
    {
        /// <summary>
        /// Status da requisição
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Mensagem de erro, alerta ou sucesso
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Informações requisitadas
        /// </summary>
        public object Data { get; set; }
    }
}

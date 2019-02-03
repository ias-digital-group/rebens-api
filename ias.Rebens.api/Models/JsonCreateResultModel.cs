using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Modelo de resposta para criação de um item
    /// </summary>
    public class JsonCreateResultModel
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
        /// Id do item criado
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Alguma informação extra
        /// </summary>
        public string Extra { get; set; }
    }
}

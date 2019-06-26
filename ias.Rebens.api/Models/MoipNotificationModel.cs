using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Objeto que retorna notificação do Moip
    /// </summary>
    public class MoipNotificationModel
    {
        /// <summary>
        /// Evento que gerou o webhook. Valores possíveis apresentados na lista abaixo.
        /// </summary>
        public string Event { get; set; }
        /// <summary>
        /// Ambiente onde o evento ocorreu (production/sandbox).
        /// </summary>
        public string Env { get; set; }
        /// <summary>
        /// Data da criação do evento.
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        /// Estrutura de recurso que gerou o webhook.
        /// </summary>
        public object Resource { get; set; }
    }
}

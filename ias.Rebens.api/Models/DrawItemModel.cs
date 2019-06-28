using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Item do sorteio
    /// </summary>
    public class DrawItemModel
    {
        /// <summary>
        /// Id do item
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Id do sorteio
        /// </summary>
        public int IdDraw { get; set; }
        /// <summary>
        /// número da sorte
        /// </summary>
        public string LuckyNumber { get; set; }
        /// <summary>
        /// Id do cliente
        /// </summary>
        public int? IdCustomer { get; set; }
        /// <summary>
        /// Nome do cliente
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// Sorteado
        /// </summary>
        public bool Drawn { get; set; }

        /// <summary>
        /// Construtor
        /// </summary>
        public DrawItemModel() { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public DrawItemModel(DrawItem item) {
            this.Id = item.Id;
            this.IdDraw = item.IdDraw;
            this.LuckyNumber = item.LuckyNumber;
            this.IdCustomer = item.IdCustomer;
            this.CustomerName = item.IdCustomer.HasValue && item.Customer != null ? item.Customer.Name : "";
            this.Drawn = item.Won;
        }
    }
}

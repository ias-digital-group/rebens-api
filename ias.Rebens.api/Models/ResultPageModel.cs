using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ias.Rebens.api.Models
{
    /// <summary>
    /// Modelo de Resultado com paginação
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultPageModel<T>
    {
        /// <summary>
        /// Lista com os itens
        /// </summary>
        public List<T> Data { get; set; }
        /// <summary>
        /// Página atual
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// Itens por página
        /// </summary>
        public int ItemsPerPage { get; set; }
        /// <summary>
        /// Total de itens
        /// </summary>
        public int TotalItems { get; set; }
        /// <summary>
        /// Total de páginas
        /// </summary>
        public int TotalPages { get; set; }
        /// <summary>
        /// Possui próxima página
        /// </summary>
        public bool HasNextPage { get; set; }
        /// <summary>
        /// Possui página anterior
        /// </summary>
        public bool HasPreviousPage { get; set; }
    }
}

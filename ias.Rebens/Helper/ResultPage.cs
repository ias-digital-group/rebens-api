using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ias.Rebens
{
    public interface IResultPage : IEnumerable
    {
        int StartPage { get; }
        int EndPage { get; }
        int CurrentPage { get; }
        bool HasNextPage { get; }
        bool HasPreviousPage { get; }
        int ItemsPerPage { get; }
        IList Page { get; }
        int TotalItems { get; }
        int TotalPages { get; }
        int ShowingStart { get; }
        int ShowingEnd { get; }

    }

    [Serializable]
    public class ResultPage<T> : IResultPage, IEnumerable<T>
    {
        #region Properties
        private int pagesToShow = 0;

        public int StartPage
        {
            get
            {
                if (this.TotalPages > this.pagesToShow)
                {
                    if (this.CurrentPage <= this.pagesToShow / 2)
                        return 0;
                    else if ((this.CurrentPage + ((this.pagesToShow / 2) - 1)) < this.TotalPages)
                        return this.CurrentPage - ((this.pagesToShow / 2) - 1);
                    else
                        return this.TotalPages - this.pagesToShow;
                }
                return 0;
            }
        }

        public int EndPage
        {
            get
            {
                if (this.TotalPages > this.pagesToShow)
                {
                    if (this.CurrentPage <= this.pagesToShow / 2)
                        return this.pagesToShow;
                    else if ((this.CurrentPage + (this.pagesToShow / 2)) < this.TotalPages)
                        return this.CurrentPage + (this.pagesToShow / 2);
                    else
                        return this.TotalPages;
                }
                return this.TotalPages;
            }
        }
        /// <summary>
        /// Retorna a lista de itens da página atual.
        /// </summary>
        public IList<T> Page { get; private set; }
        /// <summary>
        /// Indica a página atual (começando por zero).
        /// </summary>
        public int CurrentPage { get; private set; }
        /// <summary>
        /// Indica o número de itens por página.
        /// </summary>
        public int ItemsPerPage { get; private set; }
        /// <summary>
        /// Indica o total de itens.
        /// </summary>
        public int TotalItems { get; private set; }
        /// <summary>
        /// Indica o número total de páginas (retorna -1 quando essa informação não estiver disponível).
        /// </summary>
        public int TotalPages { get { return (this.TotalItems / this.ItemsPerPage) + (this.TotalItems % this.ItemsPerPage > 0 ? 1 : 0); } }
        /// <summary>
        /// Indica se existe uma próxima página.
        /// </summary>
        public bool HasNextPage { get; private set; }
        /// <summary>
        /// Indica se existe uma página anterior.
        /// </summary>
        public bool HasPreviousPage { get { return this.CurrentPage > 0; } }
        public int ShowingStart { get { return (this.CurrentPage * this.ItemsPerPage) + 1; } }
        public int ShowingEnd { get { return ((this.CurrentPage * this.ItemsPerPage) + this.ItemsPerPage) > this.TotalItems ? this.TotalItems : ((this.CurrentPage * this.ItemsPerPage) + this.ItemsPerPage); } }
        System.Collections.IList IResultPage.Page { get { return this.Page.ToArray(); } }
        #endregion Properties

        #region Constructors
        public ResultPage()
        {
            this.Page = new List<T>();
            this.CurrentPage = 0;
            this.ItemsPerPage = 10;
            this.TotalItems = 0;
            this.HasNextPage = false;
            this.pagesToShow = 6;
        }
        /// <summary>
        /// ResultPage
        /// </summary>
        /// <param name="page"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="totalItems"></param>
        public ResultPage(IList<T> page, int currentPage, int itemsPerPage, int totalItems)
        {
            this.Page = page;
            this.CurrentPage = currentPage;
            this.ItemsPerPage = itemsPerPage;
            this.TotalItems = totalItems;
            this.HasNextPage = (this.CurrentPage + 1) < this.TotalPages;
            this.pagesToShow = 6;
        }
        /// <summary>
        /// ResultPage
        /// Este construtor remove o ultimo item da lista, se este for maior que "itemsPerPage" + 1
        /// Com isso, ele sinaliza se existe uma próxima página
        /// </summary>
        /// <param name="page"></param>
        /// <param name="currentPage"></param>
        /// <param name="itemsPerPage"></param>
        public ResultPage(IList<T> page, int currentPage, int itemsPerPage)
        {
            this.Page = page;
            this.CurrentPage = currentPage;
            this.ItemsPerPage = itemsPerPage;
            this.TotalItems = -1;
            if (this.HasNextPage = page.Count > itemsPerPage)
                page.RemoveAt(itemsPerPage);
        }
        #endregion Constructors

        #region IEnumerable<T> Members
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.Page.GetEnumerator();
        }
        #endregion

        #region IEnumerable Members
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Page.GetEnumerator();
        }
        #endregion

    }
}

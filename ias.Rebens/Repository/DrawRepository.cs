using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;

namespace ias.Rebens
{
    public class DrawRepository : IDrawRepository
    {
        private string _connectionString;
        public DrawRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public DrawRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool Create(Draw draw, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    draw.Modified = draw.Created = DateTime.UtcNow;
                    draw.Deleted = false;
                    db.Draw.Add(draw);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("DrawRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar o sorteio. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool Delete(int id, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var item = db.Draw.Single(c => c.Id == id);
                    item.Deleted = true;
                    item.Modified = DateTime.UtcNow;
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("DrawRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o sorteio. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public void GenerateItems()
        {
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var draw = db.Draw.FirstOrDefault(d => d.ToGenerate && !d.Generated && !d.Deleted && d.Active);
                    if (draw != null)
                    {
                        var digits = draw.Quantity.ToString().Length;
                        if (draw.Quantity <= int.Parse("1" + 0.ToString($"d{(digits - 1)}")))
                            digits--;

                        int counter = db.DrawItem.Count(d => d.IdDraw == draw.Id);
                        int totalItems = 100000 + counter;

                        if (totalItems > draw.Quantity)
                            totalItems = draw.Quantity;

                        var connection = ((SqlConnection)db.Database.GetDbConnection());
                        connection.Open();

                        var command = connection.CreateCommand();
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 60;
                        command.CommandText = "GenerateDrawItems";
                        command.Parameters.AddWithValue("@id", draw.Id);
                        command.Parameters.AddWithValue("@digits", digits);
                        command.Parameters.AddWithValue("@totalItems", totalItems);
                        command.Parameters.AddWithValue("@counter", counter);

                        command.ExecuteNonQuery();

                        if(draw.Quantity == totalItems)
                        {
                            draw.Generated = true;
                            draw.Modified = DateTime.UtcNow;
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("DrawRepository.GenerateItems", ex.Message, "", ex.StackTrace);
            }
        }

        public ResultPage<DrawItem> ItemListPage(int page, int pageItems, string word, string sort, int idDraw, out string error)
        {
            ResultPage<DrawItem> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.DrawItem.Where(d => d.IdDraw == idDraw
                                    && (string.IsNullOrEmpty(word) || d.Customer.Name.Contains(word)));
                    switch (sort.ToLower())
                    {
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = tmpList.Count();

                    ret = new ResultPage<DrawItem>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("DrawRepository.ItemListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os itens do sorteio. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public void DistributeNumbers()
        {
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var dt = DateTime.Now.Date;
                    var listDraws = db.Draw.Where(d => d.Active && !d.Deleted && d.Generated && d.Operation.Active && d.StartDate <= dt && d.EndDate >= dt);
                    if (listDraws != null && listDraws.Count() > 0)
                    {
                        foreach (var draw in listDraws)
                        {
                            var listCustomers = db.Customer.Where(c => c.IdOperation == draw.IdOperation);
                            foreach (var customer in  listCustomers)
                            {
                                if(!db.DrawItem.Any(d => d.IdCustomer == customer.Id && d.IdDraw == draw.Id && d.Modified.Year == DateTime.Now.Year && d.Modified.Month == DateTime.Now.Month))
                                {
                                    var item = db.DrawItem.Where(d => d.IdDraw == draw.Id && !d.IdCustomer.HasValue).OrderBy(d => Guid.NewGuid()).FirstOrDefault();
                                    if (item != null)
                                    {
                                        item.IdCustomer = customer.Id;
                                        item.Modified = DateTime.Now;
                                    }
                                }
                            }
                            db.SaveChanges();
                            Thread.Sleep(50);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                logError.Create("DrawRepository.DistributeNumbers", ex.Message, "", ex.StackTrace);
            }
        }

        public ResultPage<Draw> ListPage(int page, int pageItems, string word, string sort, out string error, int? idOperation = null)
        {
            ResultPage<Draw> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Draw.Where(d => !d.Deleted && (!idOperation.HasValue || (idOperation.HasValue && d.IdOperation == idOperation.Value))
                                    && (string.IsNullOrEmpty(word) || d.Name.Contains(word)));
                    switch (sort.ToLower())
                    {
                        case "name asc":
                            tmpList = tmpList.OrderBy(f => f.Name);
                            break;
                        case "name desc":
                            tmpList = tmpList.OrderByDescending(f => f.Name);
                            break;
                        case "id asc":
                            tmpList = tmpList.OrderBy(f => f.Id);
                            break;
                        case "id desc":
                            tmpList = tmpList.OrderByDescending(f => f.Id);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = tmpList.Count();

                    ret = new ResultPage<Draw>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("DrawRepository.ListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os sorteios. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public Draw Read(int id, out string error)
        {
            Draw ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Draw.SingleOrDefault(b => !b.Deleted && b.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("DrawRepository.Read", ex.Message, $"id: {id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o sorteio. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(Draw draw, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.Draw.SingleOrDefault(c => c.Id == draw.Id);
                    if (update != null)
                    {
                        update.Name = draw.Name;
                        update.StartDate = draw.StartDate;
                        update.EndDate = draw.EndDate;
                        update.Quantity = draw.Quantity;
                        update.Active = draw.Active;
                        update.Modified = DateTime.UtcNow;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                        error = "Sorteio não encontrado!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("DrawRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o sorteio. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool SetToGenerate(int id, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var draw = db.Draw.SingleOrDefault(d => d.Id == id);
                    if (draw != null)
                    {
                        draw.ToGenerate = true;
                        draw.Modified = DateTime.UtcNow;
                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        ret = false;
                        error = "Sorteio não encontrado!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("DrawRepository.SetToGenerate", ex.Message, $"id{id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar gerar os numeros do sorteio. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public List<DrawItem> ListDrawItems(int idCustomer, out string error)
        {
            List<DrawItem> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.DrawItem.Where(d => d.IdCustomer == idCustomer && d.Draw.Active && !d.Draw.Deleted).OrderByDescending(d => d.Created).ToList();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("DrawRepository.ItemListPage", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os itens do sorteio. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }
    }
}

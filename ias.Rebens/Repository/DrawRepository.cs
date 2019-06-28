using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ias.Rebens
{
    public class DrawRepository : IDrawRepository
    {
        private string _connectionString;
        public DrawRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
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

        public bool GenerateItems(int id, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var draw = db.Draw.SingleOrDefault(d => d.Id == id);
                    if (draw != null)
                    {
                        var digits = draw.Quantity.ToString().Length;
                        if (draw.Quantity <= int.Parse("1" + 0.ToString($"d{(digits - 1)}")))
                            digits--;

                        int counter = 0;
                        for (int i = 0; i < draw.Quantity; i++)
                        {
                            var item = new DrawItem()
                            {
                                Created = DateTime.UtcNow,
                                IdDraw = id,
                                LuckyNumber = i.ToString($"d{digits}"),
                                Modified = DateTime.UtcNow,
                                Won = false
                            };
                            db.DrawItem.Add(item);
                            counter++;
                            if (counter > 1000)
                            {
                                db.SaveChanges();
                                counter = 0;
                            }
                        }
                        draw.Generated = true;
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
                int idLog = logError.Create("DrawRepository.GenerateItems", ex.Message, $"id{id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar gerar os numeros do sorteio. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
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

        public bool ItemSetCustomer(int idDraw, int idCustomer, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var item = db.DrawItem.Where(d => d.IdDraw == idDraw && !d.IdCustomer.HasValue).OrderBy(d => Guid.NewGuid()).FirstOrDefault();
                    if (item != null)
                    {
                        item.IdCustomer = idCustomer;
                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        ret = false;
                        error = "Não existe item disponível";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("DrawRepository.ItemSetCustomer", ex.Message, $"idDraw{idDraw}, idCustomer:{idCustomer}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atribuir um cliente a um numero do sorteio. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
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
    }
}

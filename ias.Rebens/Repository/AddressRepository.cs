using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ias.Rebens
{
    public class AddressRepository
    {
        public bool Create(Address address, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext())
                {
                    address.Modified = address.Created = DateTime.UtcNow;
                    db.Address.Add(address);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("AddressRepository.Create", ex);
                error = "Ocorreu um erro ao tentar criar o endereço. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool Delete(int id, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext())
                {
                    if (db.Contact.Any(c => c.IdAddress == id))
                    {
                        ret = false;
                        error = "Esso endereço não pode ser excluida pois está associado a um contato.";
                    }
                    else if (db.PartnerAddress.Any(p => p.IdAddress == id))
                    {
                        ret = false;
                        error = "Esso endereço não pode ser excluida pois está associado a um parceiro.";
                    }
                    else if (db.BenefitAddress.Any(b => b.IdAddress == id))
                    {
                        ret = false;
                        error = "Esso endereço não pode ser excluida pois está associado a um benefício.";
                    }
                    else
                    {
                        var cat = db.Address.SingleOrDefault(c => c.Id == id);
                        db.Address.Remove(cat);
                        db.SaveChanges();
                        error = null;
                    }
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("AddressRepository.Delete", ex);
                error = "Ocorreu um erro ao tentar excluir o endereço. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<Address> ListPage(int page, int pageItems, out string error)
        {
            ResultPage<Address> ret;
            try
            {
                using (var db = new RebensContext())
                {
                    var list = db.Address.OrderBy(c => c.Name).Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Address.Count();

                    ret = new ResultPage<Address>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("AddressRepository.ListPage", ex);
                error = "Ocorreu um erro ao tentar listar os endereços. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public Address Read(int id, out string error)
        {
            Address ret;
            try
            {
                using (var db = new RebensContext())
                {
                    ret = db.Address.SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("AddressRepository.Read", ex);
                error = "Ocorreu um erro ao tentar criar ler o endereço. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Address> SearchPage(string word, int page, int pageItems, out string error)
        {
            ResultPage<Address> ret;
            try
            {
                using (var db = new RebensContext())
                {
                    var list = db.Address.Where(c => c.Name.Contains(word) || c.Street.Contains(word)).OrderBy(c => c.Name).Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Address.Count(c => c.Name.Contains(word) || c.Street.Contains(word));

                    ret = new ResultPage<Address>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("AddressRepository.SearchPage", ex);
                error = "Ocorreu um erro ao tentar listar os endereços. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(Address address, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext())
                {
                    var update = db.Address.SingleOrDefault(c => c.Id == address.Id);
                    if (update != null)
                    {
                        update.City = address.City;
                        update.Complement = address.Complement;
                        update.Country = address.Country;
                        update.Latitude = address.Latitude;
                        update.Longitude = address.Longitude;
                        update.Modified = DateTime.UtcNow;
                        update.Name = address.Name;
                        update.Neighborhood = address.Neighborhood;
                        update.Number = address.Number;
                        update.State = address.State;
                        update.Street = address.Street;
                        update.Zipcode = address.Zipcode;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        error = "Endereço não encontrada!";
                    }
                }
            }
            catch (Exception ex)
            {
                int idLog = Helper.LogHelper.Add("AddressRepository.Update", ex);
                error = "Ocorreu um erro ao tentar atualizar o endereço. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }
    }
}

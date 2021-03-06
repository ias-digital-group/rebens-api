using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ias.Rebens
{
    public class AddressRepository : IAddressRepository
    {
        private string _connectionString;
        public AddressRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        public bool Create(Address address, int idAdminUser, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    address.Modified = address.Created = DateTime.UtcNow;
                    db.Address.Add(address);
                    db.SaveChanges();

                    if (idAdminUser > 0)
                    {
                        db.LogAction.Add(new LogAction()
                        {
                            Action = (int)Enums.LogAction.create,
                            Created = DateTime.UtcNow,
                            IdAdminUser = idAdminUser,
                            IdItem = address.Id,
                            Item = (int)Enums.LogItem.Address
                        });
                        db.SaveChanges();
                    }

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("AddressRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar o endereço. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool Delete(int id, int idAdminUser, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
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
                        var address = db.Address.SingleOrDefault(c => c.Id == id);
                        db.Address.Remove(address);

                        if (idAdminUser > 0)
                        {
                            db.LogAction.Add(new LogAction()
                            {
                                Action = (int)Enums.LogAction.delete,
                                Created = DateTime.UtcNow,
                                IdAdminUser = idAdminUser,
                                IdItem = address.Id,
                                Item = (int)Enums.LogItem.Address
                            });
                        }

                        db.SaveChanges();
                        error = null;
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("AddressRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar excluir o endereço. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<Address> ListByOperation(int idOperation, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Address> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Address.Where(a => a.OperationAddresses.Any(oa => oa.IdOperation == idOperation) && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Street.Contains(word) || a.City.Contains(word) || a.State.Contains(word)));
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
                        case "street asc":
                            tmpList = tmpList.OrderBy(f => f.Street);
                            break;
                        case "street desc":
                            tmpList = tmpList.OrderByDescending(f => f.Street);
                            break;
                        case "city asc":
                            tmpList = tmpList.OrderBy(f => f.City);
                            break;
                        case "city desc":
                            tmpList = tmpList.OrderByDescending(f => f.City);
                            break;
                        case "state asc":
                            tmpList = tmpList.OrderBy(f => f.State);
                            break;
                        case "state desc":
                            tmpList = tmpList.OrderByDescending(f => f.State);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Address.Count(a => a.OperationAddresses.Any(oa => oa.IdOperation == idOperation) && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Street.Contains(word) || a.City.Contains(word) || a.State.Contains(word)));

                    ret = new ResultPage<Address>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("AddressRepository.ListByOperation", ex.Message, $"idOperation: {idOperation}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os endereços. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Address> ListByBenefit(int idBenefit, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Address> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Address.Where(a => a.BenefitAddresses.Any(ba => ba.IdBenefit == idBenefit) && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Street.Contains(word) || a.City.Contains(word) || a.State.Contains(word)));
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
                        case "street asc":
                            tmpList = tmpList.OrderBy(f => f.Street);
                            break;
                        case "street desc":
                            tmpList = tmpList.OrderByDescending(f => f.Street);
                            break;
                        case "city asc":
                            tmpList = tmpList.OrderBy(f => f.City);
                            break;
                        case "city desc":
                            tmpList = tmpList.OrderByDescending(f => f.City);
                            break;
                        case "state asc":
                            tmpList = tmpList.OrderBy(f => f.State);
                            break;
                        case "state desc":
                            tmpList = tmpList.OrderByDescending(f => f.State);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Address.Count(a => a.BenefitAddresses.Any(ba => ba.IdBenefit == idBenefit) && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Street.Contains(word) || a.City.Contains(word) || a.State.Contains(word)));

                    ret = new ResultPage<Address>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("AddressRepository.ListByBenefit", ex.Message, $"idBenefit: {idBenefit}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os endereços. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Address> ListByPartner(int idPartner, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Address> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Address.Where(a => a.PartnerAddresses.Any(pa => pa.IdPartner == idPartner) && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Street.Contains(word) || a.City.Contains(word) || a.State.Contains(word)));
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
                        case "street asc":
                            tmpList = tmpList.OrderBy(f => f.Street);
                            break;
                        case "street desc":
                            tmpList = tmpList.OrderByDescending(f => f.Street);
                            break;
                        case "city asc":
                            tmpList = tmpList.OrderBy(f => f.City);
                            break;
                        case "city desc":
                            tmpList = tmpList.OrderByDescending(f => f.City);
                            break;
                        case "state asc":
                            tmpList = tmpList.OrderBy(f => f.State);
                            break;
                        case "state desc":
                            tmpList = tmpList.OrderByDescending(f => f.State);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Address.Count(a => a.PartnerAddresses.Any(pa => pa.IdPartner == idPartner) && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Street.Contains(word) || a.City.Contains(word) || a.State.Contains(word)));

                    ret = new ResultPage<Address>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("AddressRepository.ListByPartner", ex.Message, $"idPartner: {idPartner}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os endereços. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Address> ListPage(int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Address> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Address.Where(a => string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Street.Contains(word) || a.City.Contains(word) || a.State.Contains(word));
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
                        case "street asc":
                            tmpList = tmpList.OrderBy(f => f.Street);
                            break;
                        case "street desc":
                            tmpList = tmpList.OrderByDescending(f => f.Street);
                            break;
                        case "city asc":
                            tmpList = tmpList.OrderBy(f => f.City);
                            break;
                        case "city desc":
                            tmpList = tmpList.OrderByDescending(f => f.City);
                            break;
                        case "state asc":
                            tmpList = tmpList.OrderBy(f => f.State);
                            break;
                        case "state desc":
                            tmpList = tmpList.OrderByDescending(f => f.State);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Address.Count(a => string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Street.Contains(word) || a.City.Contains(word) || a.State.Contains(word));

                    ret = new ResultPage<Address>(list, page, pageItems, total);

                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("AddressRepository.ListPage", ex.Message, "", ex.StackTrace);
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
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.Address.SingleOrDefault(c => c.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("AddressRepository.Read", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar ler o endereço. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool Update(Address address, int idAdminUser, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
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

                        if (idAdminUser > 0)
                        {
                            db.LogAction.Add(new LogAction()
                            {
                                Action = (int)Enums.LogAction.update,
                                Created = DateTime.UtcNow,
                                IdAdminUser = idAdminUser,
                                IdItem = address.Id,
                                Item = (int)Enums.LogItem.Address
                            });
                        }

                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        error = "Endereço não encontrado!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("AddressRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o endereço. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<Address> ListByCourseCollege(int idCourseCollege, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Address> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Address.Where(a => a.CourseCollegeAddresses.Any(pa => pa.IdCollege == idCourseCollege) && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Street.Contains(word) || a.City.Contains(word) || a.State.Contains(word)));
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
                        case "street asc":
                            tmpList = tmpList.OrderBy(f => f.Street);
                            break;
                        case "street desc":
                            tmpList = tmpList.OrderByDescending(f => f.Street);
                            break;
                        case "city asc":
                            tmpList = tmpList.OrderBy(f => f.City);
                            break;
                        case "city desc":
                            tmpList = tmpList.OrderByDescending(f => f.City);
                            break;
                        case "state asc":
                            tmpList = tmpList.OrderBy(f => f.State);
                            break;
                        case "state desc":
                            tmpList = tmpList.OrderByDescending(f => f.State);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Address.Count(a => a.CourseCollegeAddresses.Any(pa => pa.IdCollege == idCourseCollege) && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Street.Contains(word) || a.City.Contains(word) || a.State.Contains(word)));

                    ret = new ResultPage<Address>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("AddressRepository.ListByCourseCollege", ex.Message, $"idCourseCollege: {idCourseCollege}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os endereços. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<Address> ListByCourse(int idCourse, int page, int pageItems, string word, string sort, out string error)
        {
            ResultPage<Address> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.Address.Where(a => a.CourseAddresses.Any(pa => pa.IdCourse == idCourse) && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Street.Contains(word) || a.City.Contains(word) || a.State.Contains(word)));
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
                        case "street asc":
                            tmpList = tmpList.OrderBy(f => f.Street);
                            break;
                        case "street desc":
                            tmpList = tmpList.OrderByDescending(f => f.Street);
                            break;
                        case "city asc":
                            tmpList = tmpList.OrderBy(f => f.City);
                            break;
                        case "city desc":
                            tmpList = tmpList.OrderByDescending(f => f.City);
                            break;
                        case "state asc":
                            tmpList = tmpList.OrderBy(f => f.State);
                            break;
                        case "state desc":
                            tmpList = tmpList.OrderByDescending(f => f.State);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.Address.Count(a => a.CourseAddresses.Any(pa => pa.IdCourse == idCourse) && (string.IsNullOrEmpty(word) || a.Name.Contains(word) || a.Street.Contains(word) || a.City.Contains(word) || a.State.Contains(word)));

                    ret = new ResultPage<Address>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("AddressRepository.ListByCourse", ex.Message, $"idCourse: {idCourse}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os endereços. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }
    }
}

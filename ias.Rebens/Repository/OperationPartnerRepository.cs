﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace ias.Rebens
{
    public class OperationPartnerRepository : IOperationPartnerRepository
    {
        private string _connectionString;
        public OperationPartnerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

        
        public bool Create(OperationPartner partner, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    partner.Modified = partner.Created = DateTime.UtcNow;
                    partner.Deleted = false;
                    db.OperationPartner.Add(partner);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationPartnerRepository.Create", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar o parceiro. (erro:" + idLog + ")";
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
                    var update = db.OperationPartner.SingleOrDefault(p => p.Id == id);
                    update.Modified = DateTime.UtcNow;
                    update.Deleted = true;
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationPartnerRepository.Delete", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar apagar o parceiro. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool DeleteCustomer(int idCustomer, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.OperationPartnerCustomer.SingleOrDefault(p => p.Id == idCustomer);
                    update.Modified = DateTime.UtcNow;
                    update.Status = (int)Enums.OperationPartnerCustomerStatus.deleted;
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationPartnerRepository.DeleteCustomer", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar apagar o cliente parceiro. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public ResultPage<OperationPartnerCustomer> ListCustomers(int idOperationPartner, int page, int pageItems, string word, string sort, out string error, int? status = null)
        {
            ResultPage<OperationPartnerCustomer> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.OperationPartnerCustomer.Where(c => !c.OperationPartner.Deleted 
                                        && c.Status != (int)Enums.OperationPartnerCustomerStatus.deleted 
                                        && c.IdOperationPartner == idOperationPartner 
                                        && (!status.HasValue || c.Status == status.Value)
                                        && (string.IsNullOrEmpty(word) || c.Name.Contains(word) || c.Email.Contains(word)));
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
                        case "email asc":
                            tmpList = tmpList.OrderBy(f => f.Email);
                            break;
                        case "email desc":
                            tmpList = tmpList.OrderByDescending(f => f.Email);
                            break;
                        case "cpf asc":
                            tmpList = tmpList.OrderBy(f => f.Cpf);
                            break;
                        case "cpf desc":
                            tmpList = tmpList.OrderByDescending(f => f.Cpf);
                            break;
                    }

                    var list = tmpList.Skip(page * pageItems).Take(pageItems).ToList();
                    var total = db.OperationPartnerCustomer.Count(c => !c.OperationPartner.Deleted && c.Status != (int)Enums.OperationPartnerCustomerStatus.deleted && c.IdOperationPartner == idOperationPartner && (string.IsNullOrEmpty(word) || c.Name.Contains(word) || c.Email.Contains(word)));

                    ret = new ResultPage<OperationPartnerCustomer>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationPartnerRepository.ListCustomers", ex.Message, $"idOperationPartner: {idOperationPartner}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os clientes. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public ResultPage<OperationPartner> ListPage(int page, int pageItems, string word, string sort, int idOperation, out string error, bool? status = null)
        {
            ResultPage<OperationPartner> ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var tmpList = db.OperationPartner.Where(p => !p.Deleted && p.IdOperation == idOperation && (!status.HasValue || p.Active == status.Value) && (string.IsNullOrEmpty(word) || p.Name.Contains(word)));
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
                    var total = db.OperationPartner.Count(p => !p.Deleted && p.IdOperation == idOperation && (!status.HasValue || p.Active == status.Value) && (string.IsNullOrEmpty(word) || p.Name.Contains(word)));

                    ret = new ResultPage<OperationPartner>(list, page, pageItems, total);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationPartnerRepository.ListPage", ex.Message,"", ex.StackTrace);
                error = "Ocorreu um erro ao tentar listar os parceiros. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public OperationPartner Read(int id, out string error)
        {
            OperationPartner ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.OperationPartner.SingleOrDefault(p => !p.Deleted && p.Id == id);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationPartnerRepository.Read", ex.Message, $"id: {id}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler o parceiro. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public OperationPartnerCustomer ReadCustomer(int idCustomer, out string error)
        {
            OperationPartnerCustomer ret;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    ret = db.OperationPartnerCustomer.SingleOrDefault(p => p.Status != (int)Enums.OperationPartnerCustomerStatus.deleted && p.Id == idCustomer);
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationPartnerRepository.ReadCustomer", ex.Message, $"idCustomer: {idCustomer}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar ler o parceiro. (erro:" + idLog + ")";
                ret = null;
            }
            return ret;
        }

        public bool CreateCustomer(OperationPartnerCustomer customer, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    customer.Modified = customer.Created = DateTime.UtcNow;
                    db.OperationPartnerCustomer.Add(customer);
                    db.SaveChanges();
                    error = null;
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationPartnerRepository.CreateCustomer", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar criar o cliente. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool Update(OperationPartner partner, out string error)
        {
            bool ret = true;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.OperationPartner.SingleOrDefault(c => c.Id == partner.Id);
                    if (update != null)
                    {
                        update.Active = partner.Active;
                        update.Name = partner.Name;
                        update.Modified = DateTime.UtcNow;

                        db.SaveChanges();
                        error = null;
                    }
                    else
                    {
                        error = "Parceiro não encontrado!";
                    }
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationPartnerRepository.Update", ex.Message, "", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o parceiro. (erro:" + idLog + ")";
                ret = false;
            }
            return ret;
        }

        public bool UpdateCustomerStatus(int idCustomer, int status, out string error)
        {
            bool ret = false;
            try
            {
                using (var db = new RebensContext(this._connectionString))
                {
                    var update = db.OperationPartnerCustomer.SingleOrDefault(c => c.Id == idCustomer);
                    if (update != null)
                    {
                        if (update.Status == (int)Enums.OperationPartnerCustomerStatus.newCustomer || update.Status == (int)Enums.OperationPartnerCustomerStatus.reproved)
                        {
                            update.Status = status;
                            update.Modified = DateTime.UtcNow;

                            if(update.Status == (int)Enums.OperationPartnerCustomerStatus.approved)
                            {
                                var customer = new Customer()
                                {
                                    Name = update.Name,
                                    Cpf = update.Cpf,
                                    Email = update.Email,
                                    CustomerType = (int)Enums.CustomerType.Customer
                                };
                                /// TODO: create a customer if the status is approved, on the controler send the email for validation
                            }

                            db.SaveChanges();
                            error = null;
                            ret = true;
                        }
                        else
                            error = "O status desse cliente não pode ser alterado, pois ele já foi aprovado!";
                    }
                    else
                        error = "Cliente não encontrado!";
                }
            }
            catch (Exception ex)
            {
                var logError = new LogErrorRepository(this._connectionString);
                int idLog = logError.Create("OperationPartnerRepository.UpdateCustomerStatus", ex.Message, $"idCustomer: {idCustomer}, status:{status}", ex.StackTrace);
                error = "Ocorreu um erro ao tentar atualizar o cliente. (erro:" + idLog + ")";
            }
            return ret;
        }
    }
}

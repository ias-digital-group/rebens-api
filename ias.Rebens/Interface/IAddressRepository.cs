﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IAddressRepository
    {
        Address Read(int id, out string error);

        ResultPage<Address> ListPage(int page, int pageItems, string word, string sort, out string error);

        bool Delete(int id, out string error);

        bool Create(Address address, out string error);

        bool Update(Address address, out string error);

        List<Address> ListByOperation(int idOperation, out string error);

        List<Address> ListByPartner(int idPartner, out string error);

        List<Address> ListByBenefit(int idBenefit, out string error);
    }
}
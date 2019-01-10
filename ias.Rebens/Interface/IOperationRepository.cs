using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IOperationRepository
    {
        Operation Read(int id, out string error);

        ResultPage<Operation> ListPage(int page, int pageItems, string word, string sort, out string error);

        bool Create(Operation operation, out string error);

        bool Update(Operation operation, out string error);

        bool AddContact(int idOperation, int idContact, out string error);

        bool AddAddress(int idOperation, int idAddress, out string error);

        bool DeleteContact(int idOperation, int idContact, out string error);

        bool DeleteAddress(int idOperation, int idAddress, out string error);

        List<Operation> ListByBenefit(int idBenefit, out string error);

        List<Operation> ListByBanner(int idBanner, out string error);
    }
}

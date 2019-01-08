using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IPartnerRepository
    {
        bool AddContact(int idOperation, int idContact, out string error);

        bool AddAddress(int idOperation, int idAddress, out string error);

        bool DeleteContact(int idOperation, int idContact, out string error);

        bool DeleteAddress(int idOperation, int idAddress, out string error);
    }
}

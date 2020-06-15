using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IMoipRepository
    {
        bool SaveSignature(MoipSignature signature, out string error);
        bool SaveInvoice(MoipInvoice invoice, string signatureCode);
        bool SavePayment(MoipPayment payment, string signatureCode);
        ResultPage<MoipPayment> ListPaymentsByCustomer(int idCustomer, int page, int pageItems, out string error);
        MoipSignature GetUserSignature(int idCustomer, out string error);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IWirecardPaymentRepository
    {
        bool Create(WirecardPayment wirecardPayment, out string error);
        bool Update(WirecardPayment wirecardPayment, out string error);
        WirecardPayment Read(string id, out string error);
        bool HasPaymentToProcess();
        void ProcessPayments();
        void ProcessSignatures();
    }
}

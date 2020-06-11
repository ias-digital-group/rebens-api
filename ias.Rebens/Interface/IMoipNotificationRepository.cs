using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public interface IMoipNotificationRepository
    {
        bool Create(MoipNotification notification, out string error);

        List<MoipNotification> ListToProcess(Enums.MoipNotificationEvent notificationEvent, out string error);

        ResultPage<MoipNotification> ListPage(int page, int pageItems, out string error, Enums.MoipNotificationStatus? status = null, Enums.MoipNotificationEvent? notificationEvent = null);

        bool UpdateStatus(int id, Enums.MoipNotificationStatus status, out string error);

        bool HasSubscriptionToProcess();
        bool HasInvoicesToProcess();
        bool HasPaymentsToProcess();
        void ProcessSubscription();
        void ProcessInvoices();
        void ProcessPayments();

    }
}

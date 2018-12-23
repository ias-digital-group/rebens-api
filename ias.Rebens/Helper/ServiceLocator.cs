using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens
{
    public static class ServiceLocator<TService>
    {
        private static Func<TService> s_ServiceCreator;

        public static void Config(Func<TService> serviceCreator)
        {
            s_ServiceCreator = serviceCreator;
        }

        public static TService Create()
        {
            if (s_ServiceCreator == null)
                throw new InvalidOperationException("Serviço não configurado: " + typeof(TService).FullName);

            return s_ServiceCreator();
        }
    }
}

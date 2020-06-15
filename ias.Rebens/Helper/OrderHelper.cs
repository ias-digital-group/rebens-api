using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ias.Rebens.Helper
{
    public class OrderHelper
    {
        public bool GeneratePdf(string dispId)
        {
            bool ret = true;
            var constant = new Constant();
            var request = (HttpWebRequest)WebRequest.Create($"{constant.AppSettings.App.URL}voucher/GetOrderPdf/{dispId}");
            request.Method = "GET";
            request.Timeout = 50000;

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    ret = response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ret;
        }

        public bool GenerateItemPdf(string dispId, string tkt)
        {
            bool ret = true;
            var constant = new Constant();
            var request = (HttpWebRequest)WebRequest.Create($"{constant.AppSettings.App.URL}voucher/GetOrderPdf/{dispId}");
            request.Method = "GET";
            request.Timeout = 50000;

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    ret = response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ret;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ias.Rebens.Helper
{
    public static class DecimalHelper
    {
        public static string ToExtensive(decimal amount)
        {
            if (amount <= 0 | amount >= 1000000000000000)
                return "Valor não suportado pelo sistema.";
            else
            {
                string strAmount = amount.ToString("000000000000000.00");
                string extensiveAmount = string.Empty;

                for (int i = 0; i <= 15; i += 3)
                {
                    extensiveAmount += WritePart(Convert.ToDecimal(strAmount.Substring(i, 3)));
                    if (i == 0 & extensiveAmount != string.Empty)
                    {
                        if (Convert.ToInt32(strAmount.Substring(0, 3)) == 1)
                            extensiveAmount += " Trilhão" + ((Convert.ToDecimal(strAmount.Substring(3, 12)) > 0) ? " e " : string.Empty);
                        else if (Convert.ToInt32(strAmount.Substring(0, 3)) > 1)
                            extensiveAmount += " Trilhões" + ((Convert.ToDecimal(strAmount.Substring(3, 12)) > 0) ? " e " : string.Empty);
                    }
                    else if (i == 3 & extensiveAmount != string.Empty)
                    {
                        if (Convert.ToInt32(strAmount.Substring(3, 3)) == 1)
                            extensiveAmount += " Bilhão" + ((Convert.ToDecimal(strAmount.Substring(6, 9)) > 0) ? " e " : string.Empty);
                        else if (Convert.ToInt32(strAmount.Substring(3, 3)) > 1)
                            extensiveAmount += " Bilhões" + ((Convert.ToDecimal(strAmount.Substring(6, 9)) > 0) ? " e " : string.Empty);
                    }
                    else if (i == 6 & extensiveAmount != string.Empty)
                    {
                        if (Convert.ToInt32(strAmount.Substring(6, 3)) == 1)
                            extensiveAmount += " Milhão" + ((Convert.ToDecimal(strAmount.Substring(9, 6)) > 0) ? " e " : string.Empty);
                        else if (Convert.ToInt32(strAmount.Substring(6, 3)) > 1)
                            extensiveAmount += " Milhões" + ((Convert.ToDecimal(strAmount.Substring(9, 6)) > 0) ? " e " : string.Empty);
                    }
                    else if (i == 9 & extensiveAmount != string.Empty)
                        if (Convert.ToInt32(strAmount.Substring(9, 3)) > 0)
                            extensiveAmount += " Mil" + ((Convert.ToDecimal(strAmount.Substring(12, 3)) > 0) ? " e " : string.Empty);

                    if (i == 12)
                    {
                        if (extensiveAmount.Length > 8)
                            if (extensiveAmount.Substring(extensiveAmount.Length - 6, 6) == "Bilhão" | extensiveAmount.Substring(extensiveAmount.Length - 6, 6) == "Milhão")
                                extensiveAmount += " de";
                            else
                                if (extensiveAmount.Substring(extensiveAmount.Length - 7, 7) == "Bilhões" | extensiveAmount.Substring(extensiveAmount.Length - 7, 7) == "Milhões" | extensiveAmount.Substring(extensiveAmount.Length - 8, 7) == "Trilhões")
                                extensiveAmount += " de";
                            else
                                    if (extensiveAmount.Substring(extensiveAmount.Length - 8, 8) == "Trilhões")
                                extensiveAmount += " de";

                        if (Convert.ToInt64(strAmount.Substring(0, 15)) == 1)
                            extensiveAmount += " Real";
                        else if (Convert.ToInt64(strAmount.Substring(0, 15)) > 1)
                            extensiveAmount += " Reais";

                        if (Convert.ToInt32(strAmount.Substring(16, 2)) > 0 && extensiveAmount != string.Empty)
                            extensiveAmount += " e ";
                    }

                    if (i == 15)
                        if (Convert.ToInt32(strAmount.Substring(16, 2)) == 1)
                            extensiveAmount += " Centavo";
                        else if (Convert.ToInt32(strAmount.Substring(16, 2)) > 1)
                            extensiveAmount += " Centavos";
                }
                return extensiveAmount;
            }
        }

        static string WritePart(decimal amount)
        {
            if (amount <= 0)
                return string.Empty;
            else
            {
                string mounting = string.Empty;
                if (amount > 0 & amount < 1)
                {
                    amount *= 100;
                }
                string strAmount = amount.ToString("000");
                int a = Convert.ToInt32(strAmount.Substring(0, 1));
                int b = Convert.ToInt32(strAmount.Substring(1, 1));
                int c = Convert.ToInt32(strAmount.Substring(2, 1));

                if (a == 1) mounting += (b + c == 0) ? "Cem" : "Cento";
                else if (a == 2) mounting += "Duzentos";
                else if (a == 3) mounting += "Trezentos";
                else if (a == 4) mounting += "Quatrocentos";
                else if (a == 5) mounting += "Quinhentos";
                else if (a == 6) mounting += "Seiscentos";
                else if (a == 7) mounting += "Setecentos";
                else if (a == 8) mounting += "Oitocentos";
                else if (a == 9) mounting += "Novecentos";

                if (b == 1)
                {
                    if (c == 0) mounting += ((a > 0) ? " E " : string.Empty) + "Dez";
                    else if (c == 1) mounting += ((a > 0) ? " E " : string.Empty) + "Onze";
                    else if (c == 2) mounting += ((a > 0) ? " E " : string.Empty) + "Doze";
                    else if (c == 3) mounting += ((a > 0) ? " E " : string.Empty) + "Treze";
                    else if (c == 4) mounting += ((a > 0) ? " E " : string.Empty) + "Quatorze";
                    else if (c == 5) mounting += ((a > 0) ? " E " : string.Empty) + "Quinze";
                    else if (c == 6) mounting += ((a > 0) ? " E " : string.Empty) + "Dezesseis";
                    else if (c == 7) mounting += ((a > 0) ? " E " : string.Empty) + "Dezessete";
                    else if (c == 8) mounting += ((a > 0) ? " E " : string.Empty) + "Dezoito";
                    else if (c == 9) mounting += ((a > 0) ? " E " : string.Empty) + "Dezenove";
                }
                else if (b == 2) mounting += ((a > 0) ? " E " : string.Empty) + "Vinte";
                else if (b == 3) mounting += ((a > 0) ? " E " : string.Empty) + "Trinta";
                else if (b == 4) mounting += ((a > 0) ? " E " : string.Empty) + "Quarenta";
                else if (b == 5) mounting += ((a > 0) ? " E " : string.Empty) + "Cinquenta";
                else if (b == 6) mounting += ((a > 0) ? " E " : string.Empty) + "Sessenta";
                else if (b == 7) mounting += ((a > 0) ? " E " : string.Empty) + "Setenta";
                else if (b == 8) mounting += ((a > 0) ? " E " : string.Empty) + "Oitenta";
                else if (b == 9) mounting += ((a > 0) ? " E " : string.Empty) + "Noventa";

                if (strAmount.Substring(1, 1) != "1" & c != 0 & mounting != string.Empty) mounting += " E ";

                if (strAmount.Substring(1, 1) != "1")
                    if (c == 1) mounting += "Um";
                    else if (c == 2) mounting += "Dois";
                    else if (c == 3) mounting += "Três";
                    else if (c == 4) mounting += "Quatro";
                    else if (c == 5) mounting += "Cinco";
                    else if (c == 6) mounting += "Seis";
                    else if (c == 7) mounting += "Sete";
                    else if (c == 8) mounting += "Oito";
                    else if (c == 9) mounting += "Nove";

                return mounting;
            }
        }

        public static string ToPercentageExtensive(decimal amount)
        {
            if (amount <= 0 | amount > 100)
                return "Valor não suportado pelo sistema.";
            else
                return WritePart(amount);
        }
    }
}

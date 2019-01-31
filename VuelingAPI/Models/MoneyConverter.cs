using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace VuelingAPI.Models
{
    public class MoneyConverter
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Rate { get; set; }


        public static async Task<String> GetAllRates()
        {
            String rates = String.Empty;

            try
            {
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Get, "http://quiet-stone-2094.herokuapp.com/rates.json"))
                using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        Debug.Write("StatusCode: " + response.StatusCode);

                        rates = await response.Content.ReadAsStringAsync();

                        //Persistir en servidor
                        var ratesEscritura = new StreamWriter(HostingEnvironment.MapPath("~/App_Data/rates.json"));
                        await ratesEscritura.WriteAsync(rates);
                        await ratesEscritura.FlushAsync();
                        ratesEscritura.Close();
                    }
                    else
                    {
                        Debug.Write("StatusCode: " + response.StatusCode);

                        //Recuperar transacciones de archivo si no se pudo obtener de la URL
                        var ratesLectura = new StreamReader(HostingEnvironment.MapPath("~/App_Data/rates.json"));
                        rates = ratesLectura.ReadToEnd();
                        ratesLectura.Close();
                    }

                    return rates;
                }
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);

                //Recuperar transacciones de archivo si no se pudo obtener de la URL
                var ratesLectura = new StreamReader(HostingEnvironment.MapPath("~/App_Data/rates.json"));
                rates = ratesLectura.ReadToEnd();
                ratesLectura.Close();

                return rates;
            }
        }


        //En todos los casos se usa Banker's Rounding con dos decimales
        public static string Convert(string from, string to, string value, List<MoneyConverter> rates)
        {
            string conversion = String.Empty;

            if (from == to)
            {
                //Si la moneda origen y destino es la misma
                conversion = Math.Round(decimal.Parse(value), 2, MidpointRounding.ToEven).ToString();
            }
            else
            {
                //Si se encuentra una conversión directa
                foreach (var r in rates)
                {
                    if (r.From == from && r.To == to)
                    {
                        conversion = Math.Round(decimal.Parse(value) * decimal.Parse(r.Rate), 2, MidpointRounding.ToEven).ToString();
                        break;
                    }
                }

                //Si no se encuentra una conversión directa
                if (String.IsNullOrEmpty(conversion))
                {

                    foreach (var r in rates)
                    {
                        if (to == r.To)
                        {
                            conversion = Convert(from, r.From, value, rates);
                            break;
                        }
                    }
                }
            }

            return conversion;
        }
    }
}
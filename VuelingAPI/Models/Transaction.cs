using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace VuelingAPI.Models
{
    public class Transaction
    {
        public string Sku { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }

        [JsonConstructor]
        Transaction() { }

        Transaction(string sku, string amount, string currency)
        {
            this.Sku = sku;
            this.Amount = amount;
            this.Currency = currency;
        }


        public static async Task<String> GetAllTransactions()
        {
            String transacciones = String.Empty;

            try
            {
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Get, "http://quiet-stone-2094.herokuapp.com/transactions.json"))
                using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        Debug.Write("StatusCode: " + response.StatusCode);

                        transacciones = await response.Content.ReadAsStringAsync();

                        //Persistir en el servidor
                        var transaccionesEscritura = new StreamWriter(HostingEnvironment.MapPath("~/App_Data/transacciones.json"));
                        await transaccionesEscritura.WriteAsync(transacciones);
                        await transaccionesEscritura.FlushAsync();
                        transaccionesEscritura.Close();
                    }
                    else
                    {
                        Debug.Write("StatusCode: " +  response.StatusCode);

                        //Recuperar transacciones de archivo en caso de que no se pueda acceder a la URL
                        var transaccionesLectura = new StreamReader(HostingEnvironment.MapPath("~/App_Data/transacciones.json"));
                        transacciones = transaccionesLectura.ReadToEnd();
                        transaccionesLectura.Close();
                    }

                    return transacciones;
                }
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);

                //Recuperar transacciones de archivo en caso de que no se pueda acceder a la URL
                var transaccionesLectura = new StreamReader(HostingEnvironment.MapPath("~/App_Data/transacciones.json"));
                transacciones = transaccionesLectura.ReadToEnd();
                transaccionesLectura.Close();

                return transacciones;
            }
        }


            public static async Task<IEnumerable<Transaction>> FiltrarSKUAsync(List<Transaction> listaTransactions, string sku)
            {
                //Obtengo los rates
                String rates = await MoneyConverter.GetAllRates();
                List<MoneyConverter> listaRates = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<MoneyConverter>>(rates));

                //Filtro las transacciones por el SKU requerido y convierto todas las transacciones a Euro
                var listaTransactionsSKU = from t in listaTransactions
                                           where t.Sku == sku
                                           select new Transaction(t.Sku, MoneyConverter.Convert(t.Currency, "EUR", t.Amount, listaRates), "EUR");

                //Obtengo el valor total y redondeo con Banker's Rounding
                var valorTotal = (from t in listaTransactionsSKU
                                  select decimal.Parse(t.Amount)).Sum();
                valorTotal = Math.Round(valorTotal, 2, MidpointRounding.ToEven);

                //Agrego la suma total a las transacciones
                listaTransactionsSKU = listaTransactionsSKU.Concat(new[] { new Transaction("TOTAL", valorTotal.ToString(), "EUR") });


                return listaTransactionsSKU;
            }
        }
    }
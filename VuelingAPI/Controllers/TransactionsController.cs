using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using VuelingAPI.Models;

namespace VuelingAPI.Controllers
{
    public class TransactionsController : ApiController
    {
        // GET: api/Transactions
        public async Task<HttpResponseMessage> Get()
        {
            try
            {
                //Obtener la cadena de todas las transacciones
                String transacciones = await Transaction.GetAllTransactions();

                //Generar respuesta
                HttpResponseMessage respuesta = this.Request.CreateResponse(HttpStatusCode.OK);
                respuesta.Content = new StringContent(transacciones, Encoding.UTF8, "application/json");
                return respuesta;
            }
            catch(Exception e)
            {
                Debug.Write(e.Message);

                HttpResponseMessage respuesta = this.Request.CreateResponse(HttpStatusCode.Conflict);
                respuesta.Content = new StringContent(e.Message, Encoding.UTF8, "application/text");
                return respuesta;
            }

        }


        // GET: api/Transactions/sku
        public async Task<HttpResponseMessage> Get(string sku)
        {
            try
            {
                //Obtener la cadena de todas las transacciones y convertirlas a objetos
                String transacciones = await Transaction.GetAllTransactions();
                List<Transaction> listaTransactions = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<Transaction>>(transacciones));

                //Filtrar por SKU
                IEnumerable<Transaction> listaTransactionsSKU = await Transaction.FiltrarSKUAsync(listaTransactions, sku);

                //Convertir a JSON
                String listaTransactionsSKUJSON = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(listaTransactionsSKU));
                
                //Generar respuesta
                HttpResponseMessage respuesta = this.Request.CreateResponse(HttpStatusCode.OK);
                respuesta.Content = new StringContent(listaTransactionsSKUJSON, Encoding.UTF8, "application/json");
                return respuesta;
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);

                HttpResponseMessage respuesta = this.Request.CreateResponse(HttpStatusCode.Conflict);
                respuesta.Content = new StringContent(e.Message, Encoding.UTF8, "application/text");
                return respuesta;
            }
        }
    }
}

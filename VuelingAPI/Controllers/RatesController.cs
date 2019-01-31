using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using VuelingAPI.Models;

namespace VuelingAPI.Controllers
{
    public class RatesController : ApiController
    {
        // GET: api/Rates
        public async Task<HttpResponseMessage> Get()
        {
            try
            {
                //Obtener todas las conversiones de moneda
                String rates = await MoneyConverter.GetAllRates();

                //Generar respuesta
                HttpResponseMessage respuesta = this.Request.CreateResponse(HttpStatusCode.OK);
                respuesta.Content = new StringContent(rates, Encoding.UTF8, "application/json");
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

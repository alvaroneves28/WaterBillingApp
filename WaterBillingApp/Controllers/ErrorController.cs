using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WaterBillingApp.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet]
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            ViewData["StatusCode"] = statusCode;
            ViewData["OriginalPath"] = statusCodeResult?.OriginalPath;
            ViewData["OriginalQueryString"] = statusCodeResult?.OriginalQueryString;

            switch (statusCode)
            {
                case 400:
                    ViewData["ErrorMessage"] = "Pedido inválido. Verifique os dados enviados.";
                    break;
                case 401:
                    ViewData["ErrorMessage"] = "Precisa de iniciar sessão para aceder a esta funcionalidade.";
                    break;
                case 403:
                    ViewData["ErrorMessage"] = "Não tem permissões para aceder a este recurso.";
                    break;
                case 404:
                    ViewData["ErrorMessage"] = "O recurso que procura não foi encontrado.";
                    break;
                case 405:
                    ViewData["ErrorMessage"] = $"Método HTTP não permitido para o caminho '{statusCodeResult?.OriginalPath}'. Verifique se está a usar GET ou POST corretamente.";
                    break;
                case 409:
                    ViewData["ErrorMessage"] = "Conflito de dados. Pode já existir um registo semelhante.";
                    break;
                default:
                    ViewData["ErrorMessage"] = "Ocorreu um erro inesperado ao processar o seu pedido.";
                    break;
            }

            return View("HttpStatusCode");
        }

        
        [HttpGet]
        [Route("Error")]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionDetails != null)
            {
                ViewData["ExceptionPath"] = exceptionDetails.Path;
                ViewData["ExceptionMessage"] = exceptionDetails.Error.Message;
                ViewData["StackTrace"] = exceptionDetails.Error.StackTrace;
            }

            return View("Error");
        }
    }
}

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WaterBillingApp.Controllers
{
    /// <summary>
    /// Controller responsible for handling HTTP errors and unhandled exceptions in the application.
    /// </summary>
    public class ErrorController : Controller
    {
        /// <summary>
        /// Handles errors with specific HTTP status codes (such as 404, 403, etc.).
        /// Provides a custom message based on the error code.
        /// </summary>
        /// <param name="statusCode">The returned HTTP status code.</param>
        /// <returns>
        /// A view named "HttpStatusCode" with error details, original request path, and a custom message.
        /// </returns>
        [HttpGet]
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            // Retrieve details about the original request that caused the error
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            // Pass the status code to the view via ViewData
            ViewData["StatusCode"] = statusCode;

            // Pass the original request path and query string to the view, if available
            ViewData["OriginalPath"] = statusCodeResult?.OriginalPath;
            ViewData["OriginalQueryString"] = statusCodeResult?.OriginalQueryString;

            // Set a custom error message based on the HTTP status code
            switch (statusCode)
            {
                case 400:
                    ViewData["ErrorMessage"] = "Bad request. Please check the data sent.";
                    break;
                case 401:
                    ViewData["ErrorMessage"] = "You need to log in to access this feature.";
                    break;
                case 403:
                    ViewData["ErrorMessage"] = "You do not have permission to access this resource.";
                    break;
                case 404:
                    ViewData["ErrorMessage"] = "The resource you are looking for was not found.";
                    break;
                case 405:
                    // Inform the user about disallowed HTTP method with original path reference
                    ViewData["ErrorMessage"] = $"HTTP method not allowed for path '{statusCodeResult?.OriginalPath}'. Please check if you are using GET or POST correctly.";
                    break;
                case 409:
                    ViewData["ErrorMessage"] = "Data conflict. A similar record may already exist.";
                    break;
                default:
                    // Default generic error message for unexpected status codes
                    ViewData["ErrorMessage"] = "An unexpected error occurred while processing your request.";
                    break;
            }

            // Return the HttpStatusCode view to display the error information
            return View("HttpStatusCode");
        }

        /// <summary>
        /// Handles unhandled exceptions, displaying details such as the error message and the path where it occurred.
        /// </summary>
        /// <returns>
        /// A view named "Error" with exception details including path, message, and stack trace.
        /// </returns>
        [HttpGet]
        [Route("Error")]
        public IActionResult Error()
        {
            // Get the exception details for the current request
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionDetails != null)
            {
                // Pass the path where the exception occurred to the view
                ViewData["ExceptionPath"] = exceptionDetails.Path;

                // Pass the exception message to the view
                ViewData["ExceptionMessage"] = exceptionDetails.Error.Message;

                // Pass the full stack trace to the view for debugging purposes
                ViewData["StackTrace"] = exceptionDetails.Error.StackTrace;
            }

            // Return the Error view to show exception details
            return View("Error");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manmudra.DTO.Exceptions
{
    /// <summary>
    /// The generic exception class
    /// </summary>
    public class ExceptionResponse : IExceptionResponse
    {
        /// <summary>
        /// Gets or sets the status code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets the stack trace
        /// </summary>
        public string? StackTrace { get; set; } = string.Empty;

        /// <summary>
        /// This property is being used by UI. Api inform that the passed error should be translate on appropriate UI language
        /// </summary>
        public bool DoTranslate { get; set; }

        /// <summary>
        /// Gets or Sets the error message
        /// </summary>
        public string? ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or Sets the inner exception
        /// </summary>
        public string? InnerExceptionMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// The generic exception interface
    /// </summary>
    public interface IExceptionResponse
    {
        /// <summary>
        /// Gets or sets the status code
        /// </summary>
        int StatusCode { get; set; }

        /// <summary>
        /// Gets or Sets the stack trace
        /// </summary>
        string? StackTrace { get; set; }

        /// <summary>
        /// This property is being used by UI. Api inform that the passed error should be translate on appropriate UI language
        /// </summary>
        bool DoTranslate { get; set; }

        /// <summary>
        /// Gets or Sets the error message
        /// </summary>
        string? ErrorMessage { get; set; }

        /// <summary>
        /// Gets or Sets the inner exception
        /// </summary>
        string? InnerExceptionMessage { get; set; }
    }
}

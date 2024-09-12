using Manmudra.DTO.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manmudra.DTO.Response
{
    /// <summary>
    /// The generic api response class
    /// </summary>
    public class ApiResponse<T>
    {
        /// <summary>
        /// To get and set generic success status
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// To get and set generic success data
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// To get and set generic message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// To get and set generic total count of list
        /// </summary>
        public int? Entries { get; set; }

        /// <summary>
        /// To get and set generic error
        /// </summary>
        public IExceptionResponse ExceptionResponse { get; set; } = new ExceptionResponse();
    }
}

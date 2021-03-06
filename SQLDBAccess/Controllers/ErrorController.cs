﻿using Microsoft.AspNetCore.Mvc;
using SQLDBAccess.ErrorHandling;

namespace SQLDBAccess.Controllers
{
    public class ErrorController: Controller
    {
        [Route("error/{code}")]
        public IActionResult Error(int code)
        {
            return new ObjectResult(new ApiResponse(code));
        }
    }
}

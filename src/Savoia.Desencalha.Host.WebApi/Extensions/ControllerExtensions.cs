using Savoia.Desencalha.Host.WebApi.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Savoia.Desencalha.Host.WebApi.Extensions
{
    public static class ControllerExtensions
    {
        public static ActionResult GetHttpResponse(this ControllerBase controller, BaseResponse response = null)
        {
            if (response == null || !response.Valido)
                return controller.BadRequest(response);

            if(controller.HttpContext.Request.Method == "POST")
                return controller.StatusCode(StatusCodes.Status201Created, response);
            else
                return controller.Ok(response);
        }

        public static ActionResult GetHttpResponseError(this ControllerBase controller, BaseResponse response = null)
        {
            return controller.StatusCode(StatusCodes.Status500InternalServerError, response);
        }


        public static T GetHttpResponseJsonP<T>(this ControllerBase controller, BaseResponse response = null) where T : BaseResponse
        {
            if (response == null || !response.Valido)
                controller.Response.StatusCode = StatusCodes.Status400BadRequest;
            
            if (controller.HttpContext.Request.Method == "POST")
                controller.Response.StatusCode = StatusCodes.Status201Created;
            else
                controller.Response.StatusCode = StatusCodes.Status200OK;

            return (T)response;
        }


    }
}

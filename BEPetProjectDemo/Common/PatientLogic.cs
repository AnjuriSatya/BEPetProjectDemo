using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPetProjectDemo
{
    public class PatientLogic
    {
        
      public static IActionResult CreateResponse(object data, string message)
            {
                dynamic responseData = new ExpandoObject();
                responseData.success = true;
                responseData.message = message;
                responseData.data = data;
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(responseData);
                return new OkObjectResult(json);
            }
            public static IActionResult CreateBadResponse(string message)
            {
                dynamic responseData = new ExpandoObject();
                responseData.success = false;
                responseData.message = message;
                responseData.Data = null;
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(responseData);
                return new BadRequestObjectResult(json);
            }
        }
    }



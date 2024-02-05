using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPetProjectDemo
{
    public class HTTPRoutes
    {
        public const string GetRoute = "getallpatients";
        public const string GetByIdRoute = "getpatientsbyid/{id}";
        public const string CreateRoute = "createpatient";
        public const string UpdateRoute = "updatepatientbyid/{id}";
        public const string DeleteRoute = "delpatient/{id}";
    }
}

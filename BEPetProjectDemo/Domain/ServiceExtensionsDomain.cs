using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPetProjectDemo.Domain
{
    public  static class ServiceExtensionsDomain
    {
        public static void PatientDomainServices( this IServiceCollection services)
        {
            services.AddScoped<IPatientDomain,PatientDomain>();
        }
    }
}

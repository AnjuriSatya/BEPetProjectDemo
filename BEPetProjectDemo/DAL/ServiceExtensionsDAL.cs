using BEPetProjectDemo.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPetProjectDemo.DAL
{
    public static class ServiceExtensionsDAL
    {
        public static void PatientDALServices(this IServiceCollection services)
        {
            services.AddScoped<IPatientDAL,PatientDAL>();
        }
    }
}

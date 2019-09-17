using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace datingapp.api.Helpers
{
    public static class Extensions
    {
        public static void ApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");//cors
        }

        public static void AddPagination(this HttpResponse response, int currentPage, int itemsPerPage, int totalPages, int totaltems)
        {
            PaginationInit init = new PaginationInit(currentPage, totalPages, itemsPerPage, totaltems);
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            string json = JsonConvert.SerializeObject(init, new JsonSerializerSettings
            {
                ContractResolver = contractResolver
            });

            response.Headers.Add("Pagination", json);
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}
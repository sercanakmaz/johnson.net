using JohnsonNet;
using JohnsonNet.Data;
using JohnsonNet.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace Test.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var service = JohnsonManager.Config.Current.GetCommunicationObject<BrisaTypeService.TypesSoapChannel>())
            {
                var responseCity = service.GetCities(new BrisaTypeService.GetCitiesRequest(new BrisaTypeService.GetCitiesRequestBody
                {
                    xmlCity = "<GetCities><CountryId>a30ca95a-a22b-e111-b25c-005056b853b9</CountryId></GetCities>"
                }));

                System.Console.WriteLine(responseCity.Body);
            }
            System.Console.ReadKey();
        }
    }
}

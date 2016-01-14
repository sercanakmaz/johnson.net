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
            JohnsonManager.MultiThread.ExecuteAsync(() =>
            {
                for (int i = 0; i < int.MaxValue; i++)
                {

                }
            }, name: "BigLoop");
        }
        static void SaveProduct(Product product)
        {
            try
            {
                ParamDictionary parameters = product.ToParamDictionary();
                JohnsonManager.Data.ExecuteNonQuery("dbo.SaveProduct", parameters);
            }
            catch (Exception ex)
            {
                JohnsonManager.Logger.Log(product, "SaveProduct");
            }
        }
    }
    class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}

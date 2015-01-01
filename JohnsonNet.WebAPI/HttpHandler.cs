using JohnsonNet.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Authentication;
using System.Web;
using System.Web.Compilation;

namespace JohnsonNet.WebAPI
{
    public class HttpHandler : IHttpHandlerBase
    {
        public void ProcessRequest(HttpContext context)
        {
            ISerializer serializer = new JsonSerializer();
            ApiController instance = null;

            object responseTyped = null;
            string requestBody = null;
            string actionName = null;
            string permalinkParameter = null;

            try
            {
                string controllerName = RequestContext.RouteData.Values["controller"] as string;
                bool isPost = context.Request.HttpMethod.Equals("POST", StringComparison.CurrentCultureIgnoreCase);
                actionName = RequestContext.RouteData.Values["action"] as string ?? context.Request.HttpMethod;
                permalinkParameter = RequestContext.RouteData.Values["param"] as string;

                if (string.IsNullOrEmpty(controllerName))
                    throw new ArgumentNullException("controller");

                if (isPost)
                {
                    using (var reader = new StreamReader(context.Request.InputStream))
                    {
                        requestBody = reader.ReadToEnd();
                    }
                    //if (string.IsNullOrEmpty(requestBody))
                    //    throw new ArgumentNullException("RequestBody");
                }

                var assemlby = BuildManager.GetGlobalAsaxType().BaseType.Assembly;
                var controllerType = assemlby.GetTypes()
                            .Where(p => p.BaseType == typeof(ApiController))
                            .FirstOrDefault(p => p.Name.StartsWith(controllerName, StringComparison.InvariantCultureIgnoreCase));

                if (controllerType == null)
                    throw new ArgumentException("controller");

                instance = assemlby.CreateInstance(controllerType.FullName) as ApiController;
                if (instance.Serializer != null)
                    serializer = instance.Serializer;

                var method = controllerType.GetMethods().FirstOrDefault(p => p.Name.Equals(actionName, StringComparison.CurrentCultureIgnoreCase) && p.IsPublic);
               
                if (method == null)
                    throw new ArgumentException("action");
               
                var authenticate = method.GetAttribute<AuthenticateAttribute>();

                if (authenticate != null && instance.Authenticater == null)
                    throw new ArgumentNullException("Authenticater");

                var methodParameters = method.GetParameters();

                if (!method.ReturnType.GetInterfaces().Any(p => p == typeof(IOutput)))
                    throw new ArgumentException("Return object must be inherited IOutput");

                Type authenticateReturnType = null;

                if (instance.Authenticater != null && authenticate != null) 
                {
                    var authenticaterType = instance.Authenticater.GetType();
                    var authenticateMethod = authenticaterType.GetMethod("Authenticate");

                    authenticateReturnType = authenticateMethod.ReturnType;
                }

                List<object> parameters = new List<object>();

                var inputParameterInfo = methodParameters.FirstOrDefault(p => p.ParameterType != authenticateReturnType);
                if (inputParameterInfo != null)
                {
                    object inputParameter = null;

                    // TODO: Parametrenin türemeye zorlanması isteğe bağlı olmalı
                    /*if (!methodParameter.ParameterType.GetInterfaces().Any(p => p == typeof(IInput)))
                    {
                        throw new ArgumentException("Parameter must be inherited IInput");
                    }*/

                    if (!string.IsNullOrEmpty(permalinkParameter))
                        inputParameter = Core.ConvertObject(inputParameterInfo.ParameterType, (object)permalinkParameter);
                    else if (!string.IsNullOrEmpty(requestBody))
                        inputParameter = serializer.Deserialize(requestBody, inputParameterInfo.ParameterType);

                    parameters.Add(inputParameter);
                }

                if (instance.Authenticater != null && authenticate != null)
                {
                    var authenticateResult = instance.Authenticater.Authenticate(context);

                    if (authenticateResult == null)
                        throw new AuthenticationException();

                    var authenticateParameter = methodParameters.FirstOrDefault(p => p.ParameterType.BaseType == authenticateReturnType);

                    if (authenticateParameter != null)
                        parameters.Add(authenticateResult);
                }

                responseTyped = method.Invoke(instance, parameters.ToArray());
            }
            catch (Exception ex)
            {
                if (instance != null)
                {
                    var parameterException = ex;
                    if (parameterException is TargetInvocationException) parameterException = ex.InnerException;

                    responseTyped = instance.OnError(parameterException, actionName, requestBody ?? permalinkParameter);
                }

                if (responseTyped == null) responseTyped = ex;
            }
            string response = null;
            if (responseTyped != null)
                response = serializer.Serialize(responseTyped);

            context.Response.ContentType = "application/json";
            context.Response.Write(response);
        }

        public bool IsReusable { get { return false; } }

        public System.Web.Routing.RequestContext RequestContext { get; set; }
    }
}
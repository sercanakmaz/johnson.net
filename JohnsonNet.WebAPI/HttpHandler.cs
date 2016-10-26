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
        private static List<Type> p_Controllers = null;
        private static List<Type> Controllers
        {
            get
            {
                if (p_Controllers == null)
                {
                    p_Controllers = AppDomain.CurrentDomain.GetAssemblies()
                    .Extract((p) =>
                    {
                        Type[] returnList = null;

                        try
                        {
                            returnList = p.GetTypes();
                        }
                        catch (System.Reflection.ReflectionTypeLoadException ex)
                        {
                            returnList = ex.Types;
                        }

                        return returnList
                            .Where(t => t != null)
                            .Where(t => t.IsSubclassOf(typeof(JohnsonNet.WebAPI.ApiController)))
                            .ToList();
                    })
                    .ToList();
                }

                return p_Controllers;
            }
        }

        private static int GetInheritenceCount(Type t)
        {
            int result = 0;
            while (t != typeof(object))
            {
                t = t.BaseType;
                result++;
            }
            return result;
        }

        public void ProcessRequest(HttpContext context)
        {
            ISerializer serializer = JohnsonManager.Json;
            ApiController instance = null;

            object responseTyped = null;
            string requestBody = null;
            string actionName = null;
            string permalinkParameter = null;

            try
            {
                string controllerName = RequestContext.RouteData.Values["controller"] as string;
                bool isPost = context.Request.HttpMethod.Equals("POST", StringComparison.InvariantCultureIgnoreCase);
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

                var controllerType = Controllers
                    .Where(p => p.Name.Equals(controllerName + "Controller", StringComparison.InvariantCultureIgnoreCase))
                    .OrderByDescending(p => GetInheritenceCount(p))
                    .FirstOrDefault();

                if (controllerType == null)
                    throw new ArgumentException("controller");

                instance = controllerType.Assembly.CreateInstance(controllerType.FullName) as ApiController;
                if (instance.Serializer != null)
                    serializer = instance.Serializer;

                var method = controllerType.GetMethods().FirstOrDefault(p => p.Name.Equals(actionName, StringComparison.InvariantCultureIgnoreCase) && p.IsPublic);

                if (method == null)
                    throw new ArgumentException("action");

                var authenticate = method.GetAttribute<AuthenticateAttribute>();

                if (authenticate != null && instance.Authenticater == null)
                    throw new ArgumentNullException("Authenticater");

                var methodParameters = method.GetParameters();

                if (!method.ReturnType.GetInterfaces().Any(p => p == typeof(IOutput)))
                    throw new ArgumentException("Return object must be inherited IOutput");

                if (methodParameters.Length > 2)
                    throw new ArgumentException("You can't specify more than 2 parameter");

                List<object> parameters = new List<object>();
                ParameterInfo authenticateParameterInfo = null;
                ParameterInfo inputParameterInfo = null;

                if (instance.Authenticater != null && authenticate != null)
                {
                    var authenticateResult = instance.Authenticater.Authenticate(context);

                    if (authenticateResult == null)
                        throw new AuthenticationException();

                    authenticateParameterInfo = methodParameters.FirstOrDefault(p => p.ParameterType == authenticateResult.GetType());

                    if (authenticateParameterInfo != null)
                        parameters.Add(authenticateResult);
                }

                if (authenticateParameterInfo == null)
                    inputParameterInfo = methodParameters.FirstOrDefault();
                else
                    inputParameterInfo = methodParameters.FirstOrDefault(p => p.ParameterType != authenticateParameterInfo.ParameterType);

                if (inputParameterInfo != null)
                {
                    object inputParameter = null;

                    // TODO: Parametrenin türemeye zorlanması isteğe bağlı olmalı
                    /*if (!methodParameter.ParameterType.GetInterfaces().Any(p => p == typeof(IInput)))
                    {
                        throw new ArgumentException("Parameter must be inherited IInput");
                    }*/

                    if (!string.IsNullOrEmpty(permalinkParameter))
                        inputParameter = JohnsonManager.Convert.To(inputParameterInfo.ParameterType, (object)permalinkParameter);
                    else if (!string.IsNullOrEmpty(requestBody))
                        inputParameter = serializer.Deserialize(requestBody, inputParameterInfo.ParameterType);

                    int inputParameterIndex = methodParameters.ToList().IndexOf(inputParameterInfo);

                    if (inputParameterIndex == 0 && authenticateParameterInfo != null)
                    {
                        parameters.Insert(0, inputParameter);
                    }
                    else
                    {
                        parameters.Add(inputParameter);
                    }
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
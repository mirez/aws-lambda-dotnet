﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Amazon.Lambda.Core;

namespace TestServerlessApp
{
    public class SimpleCalculator_Multiply_Generated
    {
        private readonly ServiceProvider serviceProvider;

        public SimpleCalculator_Multiply_Generated()
        {
            SetExecutionEnvironment();
            var services = new ServiceCollection();

            // By default, Lambda function class is added to the service container using the singleton lifetime
            // To use a different lifetime, specify the lifetime in Startup.ConfigureServices(IServiceCollection) method.
            services.AddSingleton<SimpleCalculator>();

            var startup = new TestServerlessApp.Startup();
            startup.ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        public Amazon.Lambda.APIGatewayEvents.APIGatewayProxyResponse Multiply(Amazon.Lambda.APIGatewayEvents.APIGatewayProxyRequest __request__, Amazon.Lambda.Core.ILambdaContext __context__)
        {
            // Create a scope for every request,
            // this allows creating scoped dependencies without creating a scope manually.
            using var scope = serviceProvider.CreateScope();
            var simpleCalculator = scope.ServiceProvider.GetRequiredService<SimpleCalculator>();

            var validationErrors = new List<string>();

            var x = default(int);
            if (__request__.PathParameters?.ContainsKey("x") == true)
            {
                try
                {
                    x = (int)Convert.ChangeType(__request__.PathParameters["x"], typeof(int));
                }
                catch (Exception e) when (e is InvalidCastException || e is FormatException || e is OverflowException || e is ArgumentException)
                {
                    validationErrors.Add($"Value {__request__.PathParameters["x"]} at 'x' failed to satisfy constraint: {e.Message}");
                }
            }

            var y = default(int);
            if (__request__.PathParameters?.ContainsKey("y") == true)
            {
                try
                {
                    y = (int)Convert.ChangeType(__request__.PathParameters["y"], typeof(int));
                }
                catch (Exception e) when (e is InvalidCastException || e is FormatException || e is OverflowException || e is ArgumentException)
                {
                    validationErrors.Add($"Value {__request__.PathParameters["y"]} at 'y' failed to satisfy constraint: {e.Message}");
                }
            }

            // return 400 Bad Request if there exists a validation error
            if (validationErrors.Any())
            {
                var errorResult = new Amazon.Lambda.APIGatewayEvents.APIGatewayProxyResponse
                {
                    Body = @$"{{""message"": ""{validationErrors.Count} validation error(s) detected: {string.Join(",", validationErrors)}""}}",
                    Headers = new Dictionary<string, string>
                    {
                        {"Content-Type", "application/json"},
                        {"x-amzn-ErrorType", "ValidationException"}
                    },
                    StatusCode = 400
                };
                return errorResult;
            }

            var response = simpleCalculator.Multiply(x, y);

            return new Amazon.Lambda.APIGatewayEvents.APIGatewayProxyResponse
            {
                Body = response,
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "text/plain"}
                },
                StatusCode = 200
            };
        }

        private static void SetExecutionEnvironment()
        {
            const string envName = "AWS_EXECUTION_ENV";

            var envValue = new StringBuilder();

            // If there is an existing execution environment variable add the annotations package as a suffix.
            if(!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(envName)))
            {
                envValue.Append($"{Environment.GetEnvironmentVariable(envName)}_");
            }

            envValue.Append("amazon-lambda-annotations_0.12.0.0");

            Environment.SetEnvironmentVariable(envName, envValue.ToString());
        }
    }
}
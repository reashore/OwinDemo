
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Owin;

namespace OwinDemo
{
	using AppFunc = Func<IDictionary<string, object>, Task>;

	class Program
	{
		static void Main()
		{
			const string uri = "http://localhost:8080";

			using (WebApp.Start<Startup4>(uri))
			{
				Console.WriteLine("Started");
				Console.ReadKey();
				Console.WriteLine("Stopping");
			}
		}
	}

	public class Startup1
	{
		public void Configuration(IAppBuilder appBuilder)
		{
			appBuilder.Run(context => context.Response.WriteAsync("Hello World"));
		}
	}

	public class Startup2
	{
		public void Configuration(IAppBuilder appBuilder)
		{
			appBuilder.UseWelcomePage();
		}
	}

	public class Startup3
	{
		public void Configuration(IAppBuilder appBuilder)
		{
			appBuilder.Use<HelloWorldComponent>();
		}
	}

	public class Startup4
	{
		public void Configuration(IAppBuilder appBuilder)
		{
			appBuilder.UseHelloWorld();
		}
	}

	public class HelloWorldComponent
	{
		private readonly AppFunc _next;

		public HelloWorldComponent(AppFunc next)
		{
			_next = next;
		}

		public Task Invoke(IDictionary<string, object> environment)
		{
			Stream responseBody = environment["owin.ResponseBody"] as Stream;

			if (responseBody == null)
			{
				throw new Exception("ResponseBody was null");
			}

			using (var streamWriter = new StreamWriter(responseBody))
			{
				string owinRequestVariables = GetOwinRequestVariables(environment);

				return streamWriter.WriteAsync(owinRequestVariables);
			}
		}

		private static string GetOwinRequestVariables(IDictionary<string, object> environment)
		{
			Stream requestBodyStream = (Stream) environment["owin.RequestBody"];
			Dictionary<string, string[]> requestHeaders = environment["owin.RequestHeaders"] as Dictionary<string, string[]>;
			string requestMethod = (string) environment["owin.RequestMethod"];
			string requestPath = (string) environment["owin.RequestPath"];
			string requestPathBase = (string) environment["owin.RequestPathBase"];
			string requestProtocol = (string) environment["owin.RequestProtocol"];
			string requestQueryString = (string) environment["owin.RequestQueryString"];
			string requestScheme = (string) environment["owin.RequestScheme"];
			string requestBody = string.Empty;

			if (requestBodyStream != null)
			{
				StreamReader streamReader = new StreamReader(requestBodyStream);
				requestBody = streamReader.ReadToEnd();
			}

			StringBuilder stringBuilder = new StringBuilder();

			stringBuilder.AppendFormat("*** Request Variables ***\n");
			stringBuilder.AppendFormat("RequestMethod = {0}\n", requestMethod);
			stringBuilder.AppendFormat("RequestProtocol = {0}\n", requestProtocol);
			stringBuilder.AppendFormat("RequestScheme = {0}\n", requestScheme);
			stringBuilder.AppendFormat("RequestPath = {0}\n", requestPath);
			stringBuilder.AppendFormat("RequestPathBase = {0}\n", requestPathBase);
			stringBuilder.AppendFormat("RequestQueryString = {0}\n", requestQueryString);
			stringBuilder.AppendFormat("RequestBody = {0}\n", requestBody);
			stringBuilder.AppendFormat("Request Headers:");

			if (requestHeaders != null)
			{
				foreach (var header in requestHeaders)
				{
					string key = header.Key;
					string[] values = header.Value;
					string value = values.Aggregate(string.Empty, (current, item) => current + (item + " "));
					stringBuilder.AppendFormat("HeaderKey = {0}, headerValue = {1}", key, value);
				}
			}

			return stringBuilder.ToString();
		}

		private static string GetOwinResponseVariables(IDictionary<string, object> environment)
		{
			//Stream respondBody = (Stream) environment["owin.ResponseBody"];
			Dictionary<string, string[]> responseHeaders = environment["owin.ResponseHeaders"] as Dictionary<string, string[]>;
			int reponseStatusCode = (int) environment["owin.ResponseStatusCode"];
			string responseReasonPhrase = (string) environment["owin.ResponseReasonPhrase"];
			string responseProtocol = (string) environment["owin.ResponseProtocol"];

			StringBuilder stringBuilder = new StringBuilder();

			stringBuilder.AppendFormat("*** Reponse Variables ***");
			stringBuilder.AppendFormat("ResponseStatusCode = {0}", reponseStatusCode);
			stringBuilder.AppendFormat("ResponseReasonPhrase = {0}", responseReasonPhrase);
			stringBuilder.AppendFormat("ResponseProtocol = {0}", responseProtocol);
			stringBuilder.AppendFormat("Reponse Headers:");

			if (responseHeaders != null)
			{
				foreach (var header in responseHeaders)
				{
					string key = header.Key;
					string[] values = header.Value;
					string value = values.Aggregate(string.Empty, (current, item) => current + (item + " "));
					stringBuilder.AppendFormat("HeaderKey = {0}, headerValue = {1}", key, value);
				}
			}

			return stringBuilder.ToString();
		}

		//public async Task Invoke(IDictionary<string, object> environment)
		//{
		//	await _next(environment);
		//}
	}

	public static class AppBuilderExtensions
	{
		public static void UseHelloWorld(this IAppBuilder appBuilder)
		{
			appBuilder.Use<HelloWorldComponent>();
		}
	}

}

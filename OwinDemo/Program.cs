
using System;
using System.Collections.Generic;
using System.IO;
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
			var response = environment["owin.ResponseBody"] as Stream;

			if (response == null)
			{
				throw new Exception("ResponseBody was null");
			}

			using (var streamWriter = new StreamWriter(response))
			{
				return streamWriter.WriteAsync("Hello World!");
			}
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

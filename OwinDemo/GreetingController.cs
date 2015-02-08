
using System.Web.Http;

namespace OwinDemo
{
	public class GreetingController :ApiController
	{
		[HttpGet]
		public Greeting Get()
		{
			return new Greeting {Text = "Hello World"};
		}
	}
}

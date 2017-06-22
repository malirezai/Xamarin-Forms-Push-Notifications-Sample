using System;
using Microsoft.WindowsAzure.MobileServices;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace pushsample.Helpers
{
    public class AuthHandler //: DelegatingHandler
    {

    }
	public enum AuthOption
	{
		Facebook, Google, Custom
	}
}

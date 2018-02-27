using System;
using System;
using Xamarin.Forms;
using System.Linq;

namespace pushsample.Helpers
{
    public static class NavigationHelpers
    {
		public static T GetPage<T>(this INavigation nav) where T : Page
		{
			var pageLists = nav?.NavigationStack?.ToList().Concat(nav?.ModalStack?.ToList());

			return pageLists.Where(x => x.GetType() == typeof(T)).FirstOrDefault() as T ?? null;
		}
    }
}

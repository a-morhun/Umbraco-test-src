using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Caching;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;
using Umbraco_test_src.Models;
using Umbraco.Web;

namespace Umbraco_test_src.Controllers
{
    public class SiteLayoutController : SurfaceController
    {
        private const string PartialViewFolder = "~/Views/Partials/SiteLayout/";


        public ActionResult RenderHeader()
        {
            List<NavigationListItem> nav = GetObjectFromCache("mainNav", 5, GetNavigationModelFromDatabase);
            return PartialView($"{PartialViewFolder}_Header.cshtml", nav);
        }

        public ActionResult RenderFooter()
        {
            return PartialView($"{ PartialViewFolder }_Footer.cshtml");
        }

        public ActionResult RenderIntro()
        {
            return PartialView($"{PartialViewFolder}_Intro.cshtml");
        }

        public ActionResult RenderTitleControls()
        {
            return PartialView($"{PartialViewFolder}_TitleControls.cshtml");
        }

        #region Navigation

        /// <summary>
        /// Finds the home page and gets the navigation structure based on it and it's children
        /// </summary>
        /// <returns>A List of NavigationListItems, representing the structure of the site.</returns>
        private List<NavigationListItem> GetNavigationModelFromDatabase()
        {
            //const int HomePositionInPath = 1;
            //int homePageId = int.Parse(CurrentPage.Path.Split(',')[HomePositionInPath]);
            //IPublishedContent homePage = Umbraco.Content(homePageId);
            IPublishedContent homePage = CurrentPage.AncestorOrSelf(1).DescendantsOrSelf().Where(x => x.DocumentTypeAlias == "home").FirstOrDefault();
            List<NavigationListItem> nav = new List<NavigationListItem>();
            nav.Add(new NavigationListItem(new NavigationLink(homePage.Url, homePage.Name)));
            nav.AddRange(GetChildNavigationList(homePage));
            return nav;
        }

        /// <summary>
        /// Loops through the child pages of a given page and their children to get the structure of the site.
        /// </summary>
        /// <param name="page">The parent page which you want the child structure for</param>
        /// <returns>A List of NavigationListItems, representing the structure of the pages below a page.</returns>
        private List<NavigationListItem> GetChildNavigationList(IPublishedContent page)
        {
            List<NavigationListItem> listItems = null;
            var childPages = page.Children.Where("Visible");
            if (childPages != null && childPages.Any() && childPages.Count() > 0)
            {
                listItems = new List<NavigationListItem>();
                foreach (var childPage in childPages)
                {
                    NavigationListItem listItem = new NavigationListItem(new NavigationLink(childPage.Url, childPage.Name));
                    listItem.Items = GetChildNavigationList(childPage);
                    listItems.Add(listItem);
                }
            }
            return listItems;
        }

        private static T GetObjectFromCache<T>(string cacheItemName, int cacheTimeInMinutes, Func<T> objectSettingFunction)
        {
            ObjectCache cache = MemoryCache.Default;
            var cachedObject = (T)cache[cacheItemName];

            if (cachedObject == null)
            {
                var policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheTimeInMinutes);
                cachedObject = objectSettingFunction();
                cache.Set(cacheItemName, cachedObject, policy);
            }

            return cachedObject;
        }



        #endregion
    }
}
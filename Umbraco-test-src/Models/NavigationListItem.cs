using System.Collections.Generic;
using System.Linq;

namespace Umbraco_test_src.Models
{
    public class NavigationListItem
    {
        public string Text { get; set; }

        public NavigationLink Link { get; set; }

        public List<NavigationListItem> Items { get; set; }

        public bool HasChildred
        {
            get { return Items != null && Items.Any(); }
        }

        public NavigationListItem()
        {
        }

        public NavigationListItem(NavigationLink link)
        {
            Link = link;
        }

        public NavigationListItem(string text)
        {
            Text = text;
        }
    }
}
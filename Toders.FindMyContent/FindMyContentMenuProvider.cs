using EPiServer.Shell.Navigation;
using System.Collections.Generic;

namespace Toders.FindMyContent
{
    [MenuProvider]
    public class FindMyContentMenuProvider : IMenuProvider
    {
        public IEnumerable<MenuItem> GetMenuItems()
        {
            return new List<MenuItem>(1)
            {
                new UrlMenuItem("Find My Content", "/global/cms/admin/findmycontent", "/FindMyContent/") {
                    IsAvailable = context => true,
                    SortIndex = 100,
                }
            };
        }
    }
}

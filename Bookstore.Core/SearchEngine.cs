using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookstore.Core
{
    public class SearchEngine
    {
        public List<IBookProvider> Providers { get; set; }

        public SearchEngine(IEnumerable<IBookProvider> providers)
        {
            Providers = new List<IBookProvider>(providers);
        }

        public async Task<IEnumerable<IBook>> Search(string title)
        {
            var tasks = Providers.Select(p => p.Search(title)).ToArray();
            await Task.WhenAll(tasks);
            return tasks.SelectMany(t => t.Result);
         }
    }
}

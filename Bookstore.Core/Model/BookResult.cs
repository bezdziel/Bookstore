using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Core.Model
{
    public class BookResult
    {
        [JsonProperty("data")]
        public List<Book> Books { get; set; }
    }
}

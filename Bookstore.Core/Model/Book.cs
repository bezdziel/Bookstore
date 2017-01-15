using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Core
{
    public class Book
    {
        public string Title { get; set; }
        public string ISBN { get; set; }
        public int Price { get; set; }

        public string Currency { get; set; }

        public string BookDetailsUrl
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}

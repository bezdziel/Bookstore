using Bookstore.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Bookstore.Provider.Model
{
  
    public class AmazonBook : IBook
    {

        public string Isbn { get; set; }
        public decimal Price { get; set; }
        public string Title { get; set; }
        public string BookDetailsUrl { get; set; }
        public string Currency { get; set; }
        public int Position { get; set; }
        public string Store { get; set; }
    }
}

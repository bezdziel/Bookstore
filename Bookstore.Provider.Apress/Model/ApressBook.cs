using Bookstore.Core;

namespace Bookstore.Provider.Model
{
    public class ApressBook : IBook
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

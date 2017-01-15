using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Core
{
    public delegate void BookSearchCompletedDelegate(object sender, IEnumerable<IBook> books);

    public interface IBookProvider
    {
        string Name { get; }
        Task<IEnumerable<IBook>> Search(string title);
    }

    public interface IBook
    {
        string Isbn { get; set; }
        string Title { get; set; }
        decimal Price { get; set; }
        string Currency { get; set; }
        string BookDetailsUrl { get; set; }
        int Position { get; set; }
        string Store { get; set; }
    }
}

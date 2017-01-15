using Bookstore.Core;
using Bookstore.Provider.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bookstore.Provider
{
    public class Apress : IBookProvider
    {
        public string Name { get { return "Apress"; } }

        private string _searchUrl = "https://www.apress.com/us/product-search?query=";
        private string _detailsUrl = "https://www.apress.com/us/book/";
        private WebClient _client;
        private Regex _isbnRegex;
        private Regex _detailsRegex;
        private Stack<string> _stack;

        private struct TitleIsbn
        {
            public string Isbn;
            public int Position;
            public string Title;
        }

        public Apress()
        {
            _client = new WebClient();
            _isbnRegex = new Regex("<a href=\"\\/us\\/book\\/(?<isbn>.*?)\">(?<title>.*?)<\\/a>", RegexOptions.Multiline | RegexOptions.Compiled);
            _detailsRegex = new Regex(@"<script\b[^>]*>\s*dataLayer\s*=\s*(?<data>[\s\S]*?)<\/script>", RegexOptions.Multiline | RegexOptions.Compiled);
            _stack = new Stack<string>();
        }

        public async Task<IEnumerable<IBook>> Search(string title)
        {
            var books = await GetBooks(title);
            return books.Where(a => a != null);
        }

        private async Task<IEnumerable<IBook>> GetBooks(string title)
        {
            var data = _client.DownloadString(new Uri(_searchUrl + title.Replace(' ', '+')));
            var list = new List<Task<IBook>>();
            var position = 0;
            foreach (Match m in _isbnRegex.Matches(data))
            {
                var isbn = m.Groups["isbn"].Value.Replace("-", "");
                var fullTitle = m.Groups["title"].Value;
                var ti = new TitleIsbn()
                {
                    Isbn = isbn,
                    Position = position++,
                    Title = fullTitle
                };
                list.Add(GetDetails(ti));
            }
            await Task.WhenAll(list);

            return list.Select(t => t.Result);
        }

        private async Task<IBook> GetDetails(TitleIsbn ti)
        {
            var client = new WebClient();
            var data = string.Empty;
            try
            {
                data = await client.DownloadStringTaskAsync(new Uri(_detailsUrl + ti.Isbn));
            }
            catch (WebException ex)
            {
                //Return 'null' when 404 Not Fount
                return null;
            }

            var json = _detailsRegex.Match(data).Groups["data"].Value;
            var resultString = Regex.Replace(json, @"[\ ](?=[^""]*(?:""[^""]*""[^""]*)*$)", "", RegexOptions.IgnorePatternWhitespace).Replace("\n", "");
            resultString = resultString.Substring(1, resultString.Length - 3);

            dynamic obj = JsonConvert.DeserializeObject(resultString);
            return new ApressBook()
            {
                Title = ti.Title,
                Price = Convert.ToDecimal(obj.pPriceGross),
                Isbn = obj.isbn,
                Position = ti.Position,
                BookDetailsUrl = obj.url,
                Store = Name,
                Currency = obj.currency
            };
        }
    }
}

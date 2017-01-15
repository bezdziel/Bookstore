
using Bookstore.Core;
using Bookstore.Provider.Model;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Bookstore.Provider
{
    public class Amazon : IBookProvider
    {
        private WebClient _client;
        private QueryBuilder _queryBuilder;

        public string Name { get { return "Amazon"; } }

        public Amazon(string apiKey, string secret, string associateTag)
        {
            _queryBuilder = new QueryBuilder(apiKey, secret, associateTag);
            _client = new WebClient();
        }

        public async Task<IEnumerable<IBook>> Search(string title)
        {
            var query = _queryBuilder.Build(title);
            if(query == null)
            {
                return await Task.Factory.StartNew(() => Enumerable.Empty<IBook>());
            }
            var books = await Task.Factory.StartNew(() => GetBooks(query));
            return books;
        }

        private IEnumerable<IBook> GetBooks(string url)
        {

            var settings = new XmlReaderSettings
            {
                ConformanceLevel = ConformanceLevel.Fragment
            };
            var position = 0;
            Stream stream = null; 
            try {
                stream = _client.OpenRead(url);
            }
            catch(WebException ex)
            {
                //Handle Amazon service error, do nothing
            }

            if (stream != null)
            {
                using (var data = stream)
                {
                    using (var reader = XmlReader.Create(data, settings))
                    {
                        var book = new AmazonBook();
                        while (reader.Read())
                        {
                            switch (reader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    if (reader.Name.Equals("Item"))
                                    {
                                        var doc = XDocument.Load(reader.ReadSubtree());
                                        var ns = "{" + doc.Root.GetDefaultNamespace().NamespaceName + "}";
                                        var isbn13Tag = doc.Descendants(ns + "EAN").FirstOrDefault();
                                        var isbn = string.Empty;
                                        if (isbn13Tag == null)
                                        {
                                            var asin = doc.Descendants(ns + "ASIN").FirstOrDefault();
                                            if (asin == null) continue;
                                            isbn = asin.Value;
                                        }
                                        else
                                        {
                                            isbn = isbn13Tag.Value;
                                        }
                                        var title = doc.Descendants(ns + "Title").First().Value;
                                        var detailsUrl = doc.Descendants(ns + "DetailPageURL").First().Value;
                                        decimal price = -1;
                                        var currency = string.Empty;
                                        var price1Tag = doc.Descendants(ns + "Price").FirstOrDefault();
                                        var price2Tag = doc.Descendants(ns + "ListPrice").FirstOrDefault();
                                        var price3Tag = doc.Descendants(ns + "LowestNewPrice").FirstOrDefault();
                                        if (price1Tag != null)
                                        {
                                            var priceStr = price1Tag.Descendants(ns + "Amount").First();
                                            price = decimal.Parse(priceStr.Value.Insert(priceStr.Value.Length - 2, NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator));
                                            currency = price1Tag.Descendants(ns + "CurrencyCode").First().Value;
                                        }
                                        else if (price2Tag != null)
                                        {
                                            var priceStr = price2Tag.Descendants(ns + "Amount").First();
                                            price = decimal.Parse(priceStr.Value.Insert(priceStr.Value.Length - 2, NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator));
                                            currency = price2Tag.Descendants(ns + "CurrencyCode").First().Value;
                                        }
                                        else if (price3Tag != null)
                                        {
                                            var priceStr = price3Tag.Descendants(ns + "Amount").First();
                                            price = decimal.Parse(priceStr.Value.Insert(priceStr.Value.Length - 2, NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator));
                                            currency = price3Tag.Descendants(ns + "CurrencyCode").First().Value;
                                        }
                                        if (price == -1)
                                        {
                                            //It's only a Kindle version avaiable
                                            //This book will be omitted, because it's doesn't have ISBN number
                                            continue;
                                        }
                                        yield return new AmazonBook()
                                        {
                                            Title = title,
                                            Isbn = isbn,
                                            Price = price,
                                            BookDetailsUrl = detailsUrl,
                                            Currency = currency,
                                            Position = position++,
                                            Store = Name
                                        };
                                    }
                                    break;
                            }
                        }
                    }

                }
            }
        }
    }
}

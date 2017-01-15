using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Bookstore.Provider
{
    public class QueryBuilder
    {
        public string Service { get; set; } = "AWSECommerceService";
        public string AssociateTag { get; set; }
        public string AWSAccessKeyId { get; set; }
        public string Operation { get; set; } = "ItemSearch";
        public string SearchIndex { get; set; } = "Books";
        public string ResponseGroup { get; set; } = "Large";
        public string Version { get; set; } = "2013-08-01";
        public string Timestamp { get { return DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"); } }
        public string Title { get; set; }
        public string Sort { get; set; } = "relevancerank";

        private string Secret = string.Empty;
        private string Host = "webservices.amazon.com";
        private string HostPath = "/onca/xml";

        public QueryBuilder(string apikey, string secret, string associateTag)
        {
            AWSAccessKeyId = apikey;
            Secret = secret;
            AssociateTag = associateTag;
        }

        public string Build(string title)
        {
            if(string.IsNullOrEmpty(AWSAccessKeyId) || string.IsNullOrEmpty(Secret) || string.IsNullOrEmpty(AssociateTag))
            {
                return null;
            }
            Title = EncodeString(title);
            var props = GetType().GetProperties().OrderBy(p => p.Name, StringComparer.Ordinal).Select(p => string.Format("{0}={1}", p.Name, EncodeString(p.GetValue(this, null) as string)));
            var query = string.Join("&", props);
            var url = string.Format("http://{0}{1}?{2}&Signature={3}", Host, HostPath, query, GetSignature(query));
            return url;
        }

        private string GetSignature(string query)
        {
            var toSign = Encoding.UTF8.GetBytes(string.Format("GET\n{0}\n{1}\n{2}", Host, HostPath, query));
            var signer = new HMACSHA256(Encoding.UTF8.GetBytes(Secret));
            var sigBytes = signer.ComputeHash(toSign);
            var signature = EncodeString(Convert.ToBase64String(sigBytes));

            return signature;
        }

        private string EncodeString(string str)
        {
            str = System.Net.WebUtility.UrlEncode(str);
            str.Replace("'", "%27").Replace("(", "%28").Replace(")", "%29").Replace("*", "%2A").Replace("!", "%21").Replace("%7e", "~");

            StringBuilder sbuilder = new StringBuilder(str);
            for (int i = 0; i <= sbuilder.Length - 1; i++)
            {
                if (sbuilder[i] == '%')
                {
                    if (char.IsDigit(sbuilder[i + 1]) && char.IsLetter(sbuilder[i + 2]))
                    {
                        sbuilder[i + 2] = char.ToUpper(sbuilder[i + 2]);
                    }
                }
            }
            return sbuilder.ToString();
        }
    }
}

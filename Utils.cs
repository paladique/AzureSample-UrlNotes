using HtmlAgilityPack;
using System.Linq;

namespace contextual_notes
{
    public class Utils
    {
        public static string Unfurl(string url)
        {

            try
            {
                string src = string.Empty;
                HtmlDocument doc;
                try
                {
                    var h = new HtmlWeb();
                    doc = h.Load(url);
                }
                catch
                {
                    return "cannot connect";
                }

                var metaTags = doc.DocumentNode.SelectNodes("//meta");

                var tags = from x in metaTags
                        where (x.Attributes["property"] != null && x.Attributes["property"].Value == "og:title") 
                        select x;

                return tags.FirstOrDefault().Attributes["content"].Value;
            }


            catch
            {
                return "null";
            }
            
        }
    }

}



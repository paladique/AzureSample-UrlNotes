using HtmlAgilityPack;
using System.Linq;
using contextual_notes.Pages;
using System;

namespace contextual_notes
{
    public class Utils
    {
        public static void Unfurl<T>(ref T item,string url) where T:Item
        {

            try
            {
                string src = string.Empty;
                var doc = new HtmlDocument();
                try
                {
                    var h = new HtmlWeb();
                    doc = h.Load(url);
                }
                catch
                {
                }

                var metaTags = doc.DocumentNode.SelectNodes("//meta");

                var title = (from x in metaTags
                        where (x.Attributes["property"] != null && x.Attributes["property"].Value == "og:title") 
                        select x).FirstOrDefault().Attributes["content"].Value;

                var keywords = ((from x in metaTags
                             where (x.Attributes["name"] != null && x.Attributes["name"].Value == "keywords")
                             select x).FirstOrDefault().Attributes["content"].Value).Split(',');

                item.Name = title;
                item.Keywords = keywords.Select(x => new Keyword() { name = x }).ToList<Keyword>() ?? null;

                if (item.GetType() == typeof(Video))
                {
                    var i = item as Video;
                    
                    var comments = (from x in metaTags
                                    where (x.Attributes["name"] != null && x.Attributes["name"].Value == "CommentCount")
                                    select x).FirstOrDefault().Attributes["content"].Value;
                    int count;
                    i.CommentCount = int.TryParse(comments, out count) ? count : 0;

                    var img = (from x in metaTags
                               where (x.Attributes["property"] != null && x.Attributes["property"].Value == "og:image")
                               select x).FirstOrDefault().Attributes["content"].Value;
                    i.Screencap = new Uri(img);
                }
                
            }


            catch
            {
                
            }
            
        }
    }

}



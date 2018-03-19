using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace contextual_notes
{
    public class Utils
    {
        private string Unfurl(string url) // use HTML Agility, run good with all page which have Meta Desc tag
        {

            //Get Meta Tags
            try
            {
                string src = string.Empty;
                HtmlDocument doc;
                try
                {
                    //src = HtmlSrc(url);
                    var h = new HtmlWeb();
                     //src = h.Load(url);
                    doc = h.Load(url);
                }
                catch
                {
                    return "cannot connect";
                }

                //string tempSrc = src.Replace("<div", "|<div");
                ////tempSrc = Replace(tempSrc, " ");
                //tempSrc = tempSrc.Trim();

                //var doc = new HtmlAgilityPack.HtmlDocument();

                //doc.LoadHtml(tempSrc.Replace("<", " <"));
                //doc.LoadHtml();


                try
                {
                    var metaTags = doc.DocumentNode.SelectNodes("//meta");

                    if (metaTags != null)
                    {
                        foreach (var tag in metaTags)
                        {
                            if (((tag.Attributes["name"] != null) || (tag.Attributes["Name"] != null) || (tag.Attributes["NAME"] != null)) && ((tag.Attributes["content"] != null) || (tag.Attributes["Content"] != null) || (tag.Attributes["CONTENT"] != null)) && ((tag.Attributes["name"].Value == "description") || (tag.Attributes["name"].Value == "Description") || (tag.Attributes["name"].Value == "DESCRIPTION") || (tag.Attributes["Name"].Value == "description") || (tag.Attributes["Name"].Value == "Description") || (tag.Attributes["Name"].Value == "DESCRIPTION") || (tag.Attributes["NAME"].Value == "description") || (tag.Attributes["NAME"].Value == "Description") || (tag.Attributes["NAME"].Value == "DESCRIPTION")))
                            {
                                string temp = tag.Attributes["content"].Value;
                                if (temp == null) temp = tag.Attributes["Content"].Value;
                                if (temp == null) temp = tag.Attributes["CONTENT"].Value;
                                return temp;
                            }
                        }
                    }
                }
                catch
                {
                    return "null";
                }
            }
            catch
            {
                return "null";
            }
            return "null";
        }
    }

}



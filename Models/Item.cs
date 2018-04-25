using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace contextual_notes.Models
{

    public class Item
    {
        [JsonProperty(PropertyName = "url")]
        public Uri Url { get; set; } 
        [JsonProperty(PropertyName = "notes")]
        public string Notes { get; set; }
        [JsonProperty(PropertyName = "tutorial")]
        public bool IsTutorial { get; set; }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "keywords")]
        public List<Keyword> Keywords { get; set; }
        //helper property to convert strings to uri
        [JsonIgnore]
        public string stringUrl { get; set;} 

        public Item() { }
    }

    public class Keyword
    {
        public string name;
    }


    public class Video : Item
    {
        [JsonProperty(PropertyName = "comment_count")]
        public int CommentCount { get; set; }
        [JsonProperty(PropertyName = "screencap")]
        public Uri Screencap { get; set; }

        internal Video(Item i, bool unfurl = true)
        {
            Id = i.Id;
            Name = i.Name;
            Url = i.Url ?? new Uri(i.stringUrl);
            Notes = i.Notes;
            IsTutorial = i.IsTutorial;
            Keywords = i.Keywords;

            if (unfurl)
            {
                var video = this;
                Utils.Unfurl(ref video);
               
                Name = video.Name;
                CommentCount = video.CommentCount;
                Keywords = video.Keywords;
                Screencap = video.Screencap;
            }

        }

        [JsonConstructor]
        public Video() { }
    }

    public class Doc : Item
    {
        internal Doc(Item i, bool unfurl = true)
        {
            Id = i.Id;
            Name = i.Name;
            Url = i.Url ?? new Uri(i.stringUrl);
            Notes = i.Notes;
            IsTutorial = i.IsTutorial;
            Keywords = i.Keywords;

            if (unfurl)
            {
                var doc = this;
                Utils.Unfurl(ref doc);

                Name = doc.Name;
                Keywords = doc.Keywords;
            }
        }

        [JsonConstructor]
        public Doc() { }
    }

    

}

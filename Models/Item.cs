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
        [JsonProperty(PropertyName = "comments")]
        public string Comments { get; set; }
        [JsonProperty(PropertyName = "tutorial")]
        public bool Tutorial { get; set; }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "keywords")]
        public List<Keyword> Keywords { get; set; }

        //helper property to convert strings to uri
        public string stringUrl { get; set;}
    }

    public class Keyword
    {
        public string name;
    }


    public class Video : Item
    {
        internal Video(Item i)
        {
            Id = i.Id;
            Name = i.Name;
            Url = i.Url;
            Comments = i.Comments;
            Tutorial = i.Tutorial;
            Keywords = i.Keywords;
        }


        [JsonConstructor]
        public Video(bool unfurl = false)
        {
            if (unfurl)
            {   
                Url = Url ?? new Uri(stringUrl);
                var video = this;
                Utils.Unfurl(ref video, Url);

                Name = video.Name;
                CommentCount = video.CommentCount;
                Keywords = video.Keywords;
                Screencap = video.Screencap;
            }
        }

        [JsonProperty(PropertyName = "comment_count")]
        public int CommentCount { get; set; }
        [JsonProperty(PropertyName = "screencap")]
        public Uri Screencap { get; set; }
    }

    public class Doc : Item
    {
        internal Doc(Item i)
        {
            Id = i.Id;
            Name = i.Name;
            Url = i.Url;
            Comments = i.Comments;
            Tutorial = i.Tutorial;
            Keywords = i.Keywords;
        }

        [JsonConstructor]
        public Doc(bool unfurl = false)
        {
            if (unfurl)
            {   
                Url = Url ?? new Uri(stringUrl);
                var video = this;
                Utils.Unfurl(ref video, Url);
                Name = video.Name;
                Keywords = video.Keywords;
            }
        }
    }

    

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using contextual_notes.Models;

namespace contextual_notes
{
    public class SampleData
    {

        public static PopulateData()
        {
            var Videos = new List<Video>();

            Video[] seedVids =  {
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/The-Open-Source-Show/Part-1-of-4-Overview-of-existing-tools-to-create-Azure-Container-Service-clusters" }, 
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/The-Open-Source-Show/Part-2-of-4-Create-an-Azure-Container-Instance-with-the-container-image" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/The-Open-Source-Show/Part-3-of-4-Create-a-Web-App-for-Containers-and-set-up-a-web-hook" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/The-Open-Source-Show/Part-4-of-4-Use-Azure-Container-Registry" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/This+Week+On+Channel+9/TWC9-Windows-Developer-Awards-Voting-Opens-AI-courses-from-Microsoft-Serial-Console-Access-in-Azure-" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/Azure-Friday/Metaparticle" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/XamarinShow/Building-the-New-MSN-News-App-with-Xamarin" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/5-Things/Episode-12-Five-Things-About-Docker" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/XamarinShow/Scalable--Service-Data-with-CosmosDB-for-Mobile" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/Tech-Crumbs/Starting-with-CosmosDB" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/Visual-Studio-Toolbox/CosmosDB-Serverless-NoSQL-for-the-NET-Developer" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/On-NET/Jeremy-Likness-CosmosDB-and-NET-Core" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/Azure-Friday/Whats-New-in-Azure-Cosmos-DB" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/Level-Up/Azure-Cosmos-DB-Comprehensive-Overview" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/Level-Up/Azure-Container-Instances-for-Multiplayer-Gaming-Theater" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/Level-Up/Planning-and-building-games-using-the-full-power-of-VSTS-Agile-CI--CD-end-to-end-demo" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/Internet-of-Things-Show/Azure-IoT-Toolkit-extension-for-Visual-Studio-Code" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/This+Week+On+Channel+9/TWC9-Connect-Next-Week-Picking-the-Right-Azure-VM-Star-Wars-Video-Cards-Life-After-the-Uniform-and-m" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Blogs/Azure/Log-Alerts-for-Application-Insights-Preview" },
                new Video(true) { stringUrl = "https://channel9.msdn.com/Shows/AI-Show/The-Simplest-Machine-Learning" }
            };

            Videos.AddRange(seedVids);

            var Docs = new List<Doc>();

            Doc[] seedDocs =  {
                new Doc(true) { stringUrl = "https://docs.microsoft.com/en-us/azure/event-grid/", Tutorial = false, Comments = "Event Grid is very interesting! Look at this later"},
                new Doc(true) { stringUrl = "https://docs.microsoft.com/en-us/azure/cosmos-db/create-sql-api-dotnet", Tutorial = true},
                new Doc(true) { stringUrl = "https://docs.microsoft.com/en-us/azure/app-service/app-service-web-tutorial-rest-api", Tutorial = true},
                new Doc(true) { stringUrl = "https://docs.microsoft.com/en-us/azure/app-service/scripts/app-service-cli-backup-onetime", Tutorial = true},
                new Doc(true) { stringUrl = "https://azure.microsoft.com/en-us/overview/serverless-computing/", Tutorial = false, Comments = "Intro to serverless computing"},
                new Doc(true) { stringUrl = "https://docs.microsoft.com/en-us/azure/app-service/app-service-web-overview"},
                new Doc(true) { stringUrl = " "},
                new Doc(true) { stringUrl = " "},
                new Doc(true) { stringUrl = " "},
                new Doc(true) { stringUrl = " "},
                new Doc(true) { stringUrl = " "},
                new Doc(true) { stringUrl = " "},
                new Doc(true) { stringUrl = " "},
                new Doc(true) { stringUrl = " "},
                new Doc(true) { stringUrl = " "},
                new Doc(true) { stringUrl = " "},
                new Doc(true) { stringUrl = " "},
                new Doc(true) { stringUrl = " "},
                new Doc(true) { stringUrl = " "},
                new Doc(true) { stringUrl = " "}    
            };

            Docs.AddRange(seedDocs);
        }
    }
}

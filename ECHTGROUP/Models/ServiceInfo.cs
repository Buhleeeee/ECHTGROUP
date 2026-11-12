namespace ECHTGROUP.Models
{ //start of namespace
    public class ServiceInfo
    { //start of class
        public string Slug { get; set; } = "";
        public string Title { get; set; } = "";
        public string Summary { get; set; } = "";
        public List<string> WhatWeDo { get; set; } = new();
        public List<string> WhatYouGet { get; set; } = new();
    } //end of class
} //end of namespace

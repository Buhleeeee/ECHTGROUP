namespace ECHTGROUP.Models
{ //start of naemspace

    public class SearchResult
    { //start of class
        public string Title { get; set; } = "";
        public string Summary { get; set; } = "";
        public string Type { get; set; } = "";
        public string Controller { get; set; } = "Home";
        public string Action { get; set; } = "Index";
        public string? Id { get; set; }
    } //end of class
     
    public class SearchViewModel
    { //end of namespcae
        public string Query { get; set; } = "";
        public List<SearchResult> Results { get; set; } = new();
    } //end of class
} //end of namespace
namespace DatingApp.API.Models
{
    public class Value
    {   
        //.net will infer that int id is the key by convention
        public int id { get; set; }
        public string Name { get; set; }
    }
}
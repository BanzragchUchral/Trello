using TrelloProject.Constants;

namespace TrelloProject.Models
{
    public class CardModel
    {
        public string id { get; set; }
        public Status Status { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string OwnerEmail { get; set; }
    }
}

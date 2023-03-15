using Airbnb.Models;

namespace Airbnbfinal.Models
{
    public class ViewMessagesViewModel
    {
        public string UserId { get; set; }
        public Dictionary<string, List<Message>> GroupedMessages { get; set; }
    }
}

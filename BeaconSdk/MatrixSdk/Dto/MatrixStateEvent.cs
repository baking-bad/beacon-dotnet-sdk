namespace MatrixSdk.Dto
{
    using System.Collections.Generic;

    public record MatrixStateEvent(string Type, Content Content, string Sender, string EventId);

    public class Content
    {
        public List<object> actions { get; set; }
        public string pattern { get; set; }
        public string rule_id { get; set; }
        public bool @default { get; set; }
        public bool enabled { get; set; }
        public Global global { get; set; }
        public Device device { get; set; }
        public string room_version { get; set; }
        public string creator { get; set; }
        public string membership { get; set; }
        public string displayname { get; set; }
        public object avatar_url { get; set; }
        public Users users { get; set; }

        public int? users_default { get; set; }

        // public Events events { get; set; }
        public int? events_default { get; set; }
        public int? state_default { get; set; }
        public int? ban { get; set; }
        public int? kick { get; set; }
        public int? redact { get; set; }
        public int? invite { get; set; }
        public string join_rule { get; set; }
        public string history_visibility { get; set; }
        public string guest_access { get; set; }
    }
    
    // public class Event
    // {
    //     public string type { get; set; } //
    //     public Content content { get; set; } //
    //     public string sender { get; set; } //
    //     public string event_id { get; set; } //
    // }
}
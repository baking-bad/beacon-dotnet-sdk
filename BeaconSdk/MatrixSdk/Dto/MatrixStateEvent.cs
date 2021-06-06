namespace MatrixSdk.Dto
{
    using System.Collections.Generic;
    using Newtonsoft.Json.Linq;

    public record MatrixStateEvent(string Type, JObject Content, string Sender, string EventId);

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
    
    public class Content3
    {
        public string avatar_url { get; set; }
        public int last_active_ago { get; set; }
        public string presence { get; set; }
        public bool currently_active { get; set; }
        public string status_msg { get; set; }
        public string custom_config_key { get; set; }
        public string membership { get; set; }
        public string displayname { get; set; }
        public string body { get; set; }
        public string msgtype { get; set; }
        public string format { get; set; }
        public string formatted_body { get; set; }
        public List<string> user_ids { get; set; }
        // public Tags tags { get; set; }
        public string name { get; set; }
    }
}
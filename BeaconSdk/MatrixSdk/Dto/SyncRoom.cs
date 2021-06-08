namespace MatrixSdk.Dto
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Newtonsoft.Json;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public record IsExternalInit;
    
    public class Condition
    {
        public string kind { get; set; }
        public string key { get; set; }
        public string pattern { get; set; }
        public string @is { get; set; }
    }

    public class Underride
    {
        public List<Condition> conditions { get; set; }
        public List<object> actions { get; set; }
        public string rule_id { get; set; }
        public bool @default { get; set; }
        public bool enabled { get; set; }
    }

    public class Content2
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

    public class Override
    {
        public List<Condition> conditions { get; set; }
        public List<object> actions { get; set; }
        public string rule_id { get; set; }
        public bool @default { get; set; }
        public bool enabled { get; set; }
    }

    public class Global
    {
        public List<Underride> underride { get; set; }
        public List<object> sender { get; set; }
        public List<object> room { get; set; }
        // public List<Content> content { get; set; }
        public List<Override> @override { get; set; }
    }

    public class Device
    {
    }

    public class Event
    {
        public string type { get; set; }
        // public Content content { get; set; }

        [JsonProperty("m.room.name")] public int MRoomName { get; set; }

        [JsonProperty("m.room.power_levels")] public int MRoomPowerLevels { get; set; }

        [JsonProperty("m.room.history_visibility")]
        public int MRoomHistoryVisibility { get; set; }

        [JsonProperty("m.room.canonical_alias")]
        public int MRoomCanonicalAlias { get; set; }

        [JsonProperty("m.room.avatar")] public int MRoomAvatar { get; set; }

        public string sender { get; set; }
        public string state_key { get; set; }
        public object origin_server_ts { get; set; }
        public Unsigned unsigned { get; set; }
        public string event_id { get; set; }
    }

    public class AccountData
    {
        public List<Event> events { get; set; }
    }

    public class ToDevice
    {
        public List<object> events { get; set; }
    }

    public class DeviceLists
    {
        public List<object> changed { get; set; }
        public List<object> left { get; set; }
    }

    public class Presence
    {
        public List<object> events { get; set; }
    }

    public class Users
    {
        [JsonProperty("@50e1e86581ed9468fa48cee9acba5a645d048164666cc8808fbdb4626c015a1f:matrix.papers.tech")]
        public int _50e1e86581ed9468fa48cee9acba5a645d048164666cc8808fbdb4626c015a1fMatrixPapersTech { get; set; }
    }

    public class Unsigned
    {
        public int age { get; set; }
    }

    public class Timeline
    {
        public List<Event> events { get; set; }
        public string prev_batch { get; set; }
        public bool limited { get; set; }
    }

    public class State2
    {
        public List<object> events { get; set; }
    }

    public class Ephemeral
    {
        public List<object> events { get; set; }
    }

    public class UnreadNotifications
    {
    }

    public class Summary
    {
    }

    public class LbPEdYsHNYhdeSrPwKMatrixPapersTech
    {
        public Timeline timeline { get; set; }

        public State State { get; set; }
        // public AccountData account_data { get; set; }
        // public Ephemeral ephemeral { get; set; }
        // public UnreadNotifications unread_notifications { get; set; }
        // public Summary summary { get; set; }
    }

    public class Join
    {
        [JsonProperty("!LbPEdYsHNYhdeSrPwK:matrix.papers.tech")]
        public LbPEdYsHNYhdeSrPwKMatrixPapersTech LbPEdYsHNYhdeSrPwKMatrixPapersTech { get; set; }
    }

    public class Invite
    {
    }

    public class Leave
    {
    }

    public class Rooms2
    {
        public Join join { get; set; }
        public Invite invite { get; set; }
        public Leave leave { get; set; }
    }

    public class Groups
    {
        public Join join { get; set; }
        public Invite invite { get; set; }
        public Leave leave { get; set; }
    }

    public class DeviceOneTimeKeysCount
    {
    }

    public class Root
    {
        public AccountData account_data { get; set; }
        public ToDevice to_device { get; set; }
        public DeviceLists device_lists { get; set; }
        public Presence presence { get; set; }
        public Rooms Rooms { get; set; }
        public Groups groups { get; set; }
        public DeviceOneTimeKeysCount device_one_time_keys_count { get; set; }
        public string next_batch { get; set; }
    }
}
namespace MatrixSdk
{
    using System.Collections.Generic;

    public class MatrixRoom
    {
        public MatrixRoom(string id, MatrixRoomStatus status, List<string> memberUserIds)
        {
            Id = id;
            Status = status;
            MemberUserIds = memberUserIds;
        }

        public MatrixRoom(string id, MatrixRoomStatus status)
        {
            Id = id;
            Status = status;
            MemberUserIds = new List<string>();
        }

        public string Id { get; }
        public MatrixRoomStatus Status { get; set; }
        public List<string> MemberUserIds { get; }
    }
}
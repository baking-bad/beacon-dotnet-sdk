namespace MatrixSdk
{
    using System.Collections.Generic;

    public class MatrixRoom
    {
        public MatrixRoom(string id, MatrixRoomStatus status, List<string> joinedUserIds)
        {
            Id = id;
            Status = status;
            JoinedUserIds = joinedUserIds;
        }
        public string Id { get; }

        public MatrixRoomStatus Status { get; }

        public List<string> JoinedUserIds { get; }
    }
}
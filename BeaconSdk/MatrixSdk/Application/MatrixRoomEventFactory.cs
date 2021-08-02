namespace MatrixSdk.Application
{
    using System.Collections.Generic;
    using Domain.Room;
    using Infrastructure.Dto.Sync;

    public class MatrixRoomEventFactory
    {
        public List<BaseRoomEvent> CreateFromJoined(string roomId, JoinedRoom joinedRoom)
        {
            var roomEvents = new List<BaseRoomEvent>();

            foreach (var timelineEvent in joinedRoom.Timeline.Events)
                if (JoinRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var joinRoomEvent))
                    roomEvents.Add(joinRoomEvent!);
                else if (CreateRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var createRoomEvent))
                    roomEvents.Add(createRoomEvent!);
                else if (InviteToRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var inviteToRoomEvent))
                    roomEvents.Add(inviteToRoomEvent!);
                else if (TextMessageEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var textMessageEvent))
                    roomEvents.Add(textMessageEvent);

            return roomEvents;
        }

        public List<BaseRoomEvent> CreateFromInvited(string roomId, InvitedRoom invitedRoom)
        {
            var roomEvents = new List<BaseRoomEvent>();

            foreach (var inviteStateEvent in invitedRoom.InviteState.Events)
                if (JoinRoomEvent.Factory.TryCreateFromStrippedState(inviteStateEvent, roomId, out var joinRoomEvent))
                    roomEvents.Add(joinRoomEvent!);
                else if (CreateRoomEvent.Factory.TryCreateFromStrippedState(inviteStateEvent, roomId, out var createRoomEvent))
                    roomEvents.Add(createRoomEvent!);
                else if (InviteToRoomEvent.Factory.TryCreateFromStrippedState(inviteStateEvent, roomId, out var inviteToRoomEvent))
                    roomEvents.Add(inviteToRoomEvent!);
                else if (TextMessageEvent.Factory.TryCreateFromStrippedState(inviteStateEvent, roomId, out var textMessageEvent))
                    roomEvents.Add(textMessageEvent);

            return roomEvents;
        }

        public List<BaseRoomEvent> CreateFromLeft(string roomId, LeftRoom leftRoom)
        {
            var roomEvents = new List<BaseRoomEvent>();

            foreach (var timelineEvent in leftRoom.Timeline.Events)
                if (JoinRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var joinRoomEvent))
                    roomEvents.Add(joinRoomEvent!);
                else if (CreateRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var createRoomEvent))
                    roomEvents.Add(createRoomEvent!);
                else if (InviteToRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var inviteToRoomEvent))
                    roomEvents.Add(inviteToRoomEvent!);
                else if (TextMessageEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var textMessageEvent))
                    roomEvents.Add(textMessageEvent);

            return roomEvents;
        }
    }
}
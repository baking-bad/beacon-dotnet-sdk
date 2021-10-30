namespace Matrix.Sdk.Core.Domain.Network
{
    using System;

    public record SendMessageRequest(Uri NodeAddress, string AccessToken, string RoomId, string TransactionId,
        string Message);
}
namespace Matrix.Sdk.Core.Domain.Network
{
    using System;

    public record SendMessageRequest(Uri BaseAddress, string AccessToken, string RoomId, string TransactionId,
        string Message);
}
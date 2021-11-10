// namespace Beacon.Sdk.Core
// {
//     using System;
//     using System.Collections.Generic;
//     using Matrix.Sdk.Core.Domain.Room;
//     using Matrix.Sdk.Core.Utils;
//     using Matrix.Sdk.Listener;
//
//     
//
//     //;// &&
//     //senderUserId.StartsWith($"@{hexHash}");
//
//     // public interface IPeerRepository
//     // {
//     //     BeaconPeer? TryRead(HexString hexHash);
//     //     BeaconPeer? TryRead(HexString hexHash);
//     // }
//
//     // public class BeaconPeerRepository : IPeerRepository
//     // {
//     //     public readonly ConcurrentDictionary<>
//     //     public BeaconPeer? TryRead(HexString hexHash)
//     //     {
//     //         
//     //     }
//     // }
//
//     public class EncryptedEventListener : MatrixEventListener<List<BaseRoomEvent>>
//     {
//         private readonly IPeerRepository _peerRepository;
//
//         // public MatrixEventListener(IPeerRepository peerRepository)
//         // {
//         //     _peerRepository = peerRepository;
//         // }
//
//         public override void OnCompleted()
//         {
//             throw new NotImplementedException();
//         }
//
//         public override void OnError(Exception error) => throw error;
//
//         public override void OnNext(List<BaseRoomEvent> value)
//         {
//             foreach (BaseRoomEvent matrixRoomEvent in value)
//                 if (matrixRoomEvent is TextMessageEvent textMessageEvent)
//                 {
//                     string userSenderId = textMessageEvent.SenderUserId;
//                     // _peerRepository.TryRead()
//                 }
//
//             // if (SenderIdMatchesPublicKeyToListen(textMessageEvent.SenderUserId, _publicKeyToListen) &&
//             //     _cryptographyService.Validate(textMessageEvent.Message)) // Todo: implement validate
//             //     _onNewTextMessage(textMessageEvent);
//         }
//
//         // private bool SenderIdMatchesPublicKeyToListen(string senderUserId, HexString publicKey)
//         // {
//         //     byte[] hash = _cryptographyService.Hash(publicKey.ToByteArray());
//         //
//         //     return HexString.TryParse(hash, out HexString hexHash) &&
//         //            senderUserId.StartsWith($"@{hexHash}");
//         // }
//     }
//
//     public class EncryptedMessageListener : MatrixEventListener<List<BaseRoomEvent>>
//     {
//         private readonly ICryptographyService _cryptographyService;
//         private readonly Action<TextMessageEvent> _onNewTextMessage;
//         private readonly HexString _publicKeyToListen;
//
//         public EncryptedMessageListener(ICryptographyService cryptographyService,
//             HexString publicKeyToListen, Action<TextMessageEvent> onNewTextMessage)
//         {
//             _cryptographyService = cryptographyService;
//             _publicKeyToListen = publicKeyToListen;
//             _onNewTextMessage = onNewTextMessage;
//         }
//
//         public override void OnCompleted() => throw new NotImplementedException();
//
//         public override void OnError(Exception error) => throw error;
//
//         public override void OnNext(List<BaseRoomEvent> value)
//         {
//             foreach (BaseRoomEvent matrixRoomEvent in value)
//                 if (matrixRoomEvent is TextMessageEvent textMessageEvent)
//                     if (SenderIdMatchesPublicKeyToListen(textMessageEvent.SenderUserId, _publicKeyToListen) &&
//                         _cryptographyService.Validate(textMessageEvent.Message)) // Todo: implement validate
//                         _onNewTextMessage(textMessageEvent);
//         }
//
//         private bool SenderIdMatchesPublicKeyToListen(string senderUserId, HexString publicKey)
//         {
//             byte[] hash = _cryptographyService.Hash(publicKey.ToByteArray());
//
//             return HexString.TryParse(hash, out HexString hexHash) &&
//                    senderUserId.StartsWith($"@{hexHash}");
//         }
//     }
// }
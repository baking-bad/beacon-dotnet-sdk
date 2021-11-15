namespace Beacon.Sdk.Core
{
    // public interface IMatrixEventListenerFactory
    // {
    //     EncryptedMessageListener CreateEncryptedMessageListener(HexString publicKeyToListen,
    //         Action<TextMessageEvent> onNewTextMessage);
    // }
    //
    // public class MatrixMatrixEventListenerFactory : IMatrixEventListenerFactory
    // {
    //     private readonly ICryptographyService _cryptographyService;
    //
    //     public MatrixMatrixEventListenerFactory(ICryptographyService cryptographyService)
    //     {
    //         _cryptographyService = cryptographyService;
    //     }
    //
    //     public EncryptedMessageListener CreateEncryptedMessageListener(HexString publicKeyToListen,
    //         Action<TextMessageEvent> onNewTextMessage) =>
    //         new(_cryptographyService, publicKeyToListen, onNewTextMessage);
    // }
}
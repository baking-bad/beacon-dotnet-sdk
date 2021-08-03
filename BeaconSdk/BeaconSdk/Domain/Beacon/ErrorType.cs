namespace BeaconSdk.Domain.Beacon
{
    public class ErrorType
    {
        ///<summary>
        /// Indicates that the transaction broadcast failed.
        /// </summary>
        public const string BroadcastError = "BROADCAST_ERROR";

        ///<summary>
        /// Indicates that the specified network is not supported by the wallet.
        /// </summary>
        public const string NetworkNotSupported = "NETWORK_NOT_SUPPORTED_ERROR";

        ///<summary>
        /// Indicates that there is no address present for the protocol or specified network.
        /// </summary>
        public const string NoAddressError = "NO_ADDRESS_ERROR";
        
        ///<summary>
        /// Indicates that a private key matching the address provided in the request could not be found.
        /// </summary>
        public const string NoPrivateKeyFound = "NO_PRIVATE_KEY_FOUND_ERROR";
        
        ///<summary>
        /// Indicates that the signature was blocked and could not be completed (`Beacon.Request.signPayload`)
        /// or the permissions requested by the dApp were rejected (`Beacon.Request.permission`).
        /// </summary>
        public const string NotGranted = "NOT_GRANTED_ERROR";
        
        ///<summary>
        /// Indicates that any of the provided parameters are invalid.
        /// </summary>
        public const string ParametersInvalid = "PARAMETERS_INVALID_ERROR";
        
        ///<summary>
        /// Indicates that too many operation details were included in the request
        /// and they could not be included into a single operation group.
        /// </summary>
        public const string TooManyOperations = "TOO_MANY_OPERATIONS_ERROR";
        
        ///<summary>
        /// Indicates that the transaction included in the request could not be parsed or was rejected by the node.
        /// </summary>
        public const string TransactionInvalid = "TRANSACTION_INVALID_ERROR";
        
        ///<summary>
        /// Indicates that the requested type of signature is not supported in the client.
        /// </summary>
        public const string SignatureTypeNotSupported = "SIGNATURE_TYPE_NOT_SUPPORTED";

        ///<summary>
        /// Indicates that the request execution has been aborted by the user or the wallet.
        /// </summary>
        public const string Aborted = "ABORTED_ERROR";

        ///<summary>
        /// Indicates that an unexpected error occurred.
        /// </summary>
        public const string Unknown = "UNKNOWN_ERROR";
    }
}
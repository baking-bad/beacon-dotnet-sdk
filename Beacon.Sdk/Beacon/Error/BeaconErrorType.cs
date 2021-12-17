namespace Beacon.Sdk.Beacon.Error
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum BeaconErrorType
    {
        /// <summary>
        ///     Indicates that the transaction broadcast failed.
        /// </summary>
        BROADCAST_ERROR,

        /// <summary>
        ///     Indicates that the specified network is not supported by the wallet.
        /// </summary>
        NETWORK_NOT_SUPPORTED_ERROR,

        /// <summary>
        ///     Indicates that there is no address present for the protocol or specified network.
        /// </summary>
        NO_ADDRESS_ERROR,

        /// <summary>
        ///     Indicates that a private key matching the address provided in the request could not be found.
        /// </summary>
        NO_PRIVATE_KEY_FOUND_ERROR,

        /// <summary>
        ///     Indicates that the signature was blocked and could not be completed (`Beacon.Request.signPayload`)
        ///     or the permissions requested by the dApp were rejected (`Beacon.Request.permission`).
        /// </summary>
        NOT_GRANTED_ERROR,

        /// <summary>
        ///     Indicates that any of the provided parameters are invalid.
        /// </summary>
        PARAMETERS_INVALID_ERROR,

        /// <summary>
        ///     Indicates that too many operation details were included in the request
        ///     and they could not be included into a single operation group.
        /// </summary>
        TOO_MANY_OPERATIONS_ERROR,

        /// <summary>
        ///     Indicates that the transaction included in the request could not be parsed or was rejected by the node.
        /// </summary>
        TRANSACTION_INVALID_ERROR,

        /// <summary>
        ///     Indicates that the requested type of signature is not supported in the client.
        /// </summary>
        SIGNATURE_TYPE_NOT_SUPPORTED,

        /// <summary>
        ///     Indicates that the request execution has been aborted by the user or the wallet.
        /// </summary>
        ABORTED_ERROR,

        /// <summary>
        ///     Indicates that an unexpected error occurred.
        /// </summary>
        UNKNOWN_ERROR
    }
}
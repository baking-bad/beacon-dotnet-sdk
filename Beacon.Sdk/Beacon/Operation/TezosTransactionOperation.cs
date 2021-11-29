namespace Beacon.Sdk.Beacon.Operation
{
    public record TezosTransactionOperation(
        TezosOperationType kind,
        string source,
        string fee,
        string counter, 
        string gas_limit, 
        string storage_limit,
        string amount,
        string destination) 
        : TezosBaseOperation(kind) , IPartialTezosOperation;
}
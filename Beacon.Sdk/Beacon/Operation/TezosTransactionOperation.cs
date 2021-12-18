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
        : TezosBaseOperation(kind); //, IPartialTezosOperation;

    public record PartialTezosTransactionOperation(
            string Amount,
            string Destination,
            string Parameters)
        : TezosBaseOperation(TezosOperationType.transaction), IPartialTezosOperation;
}
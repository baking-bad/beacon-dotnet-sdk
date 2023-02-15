namespace Beacon.Sdk.Beacon.Operation
{
    public record TezosDelegationOperation(string Delegate) : TezosBaseOperation(TezosOperationType.delegation),
        IPartialTezosOperation;
}
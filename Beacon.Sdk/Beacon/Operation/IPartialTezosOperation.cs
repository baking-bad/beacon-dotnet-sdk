namespace Beacon.Sdk.Beacon.Operation
{
    public interface IPartialTezosOperation
    {
        public TezosOperationType Kind { get; }
    }
}
using Unity.Netcode;
using Unity.Collections;
using System;

public struct CardProposalNetwork :
    INetworkSerializable,
    IEquatable<CardProposalNetwork>
{
    public int proposalId;
    public FixedString64Bytes cardTitle;
    public ulong proposerClientId;

    public bool Equals(CardProposalNetwork other)
    {
        return proposalId == other.proposalId
            && proposerClientId == other.proposerClientId
            && cardTitle.Equals(other.cardTitle);
    }

    public override bool Equals(object obj)
    {
        return obj is CardProposalNetwork other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(proposalId, proposerClientId, cardTitle);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer)
        where T : IReaderWriter
    {
        serializer.SerializeValue(ref proposalId);
        serializer.SerializeValue(ref cardTitle);
        serializer.SerializeValue(ref proposerClientId);
    }
}

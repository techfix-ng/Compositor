using System.Buffers.Binary;

namespace Compositor.Psd;

public sealed record PsdHeader(bool IsPsb, ushort Channels, int Width, int Height, ushort Depth, ushort ColorMode);
public static class PsdHeaderReader
{
    public static PsdHeader Read(Stream stream)
    {
        Span<byte> header = stackalloc byte[26];
        stream.ReadExactly(header);
        if (!header[..4].SequenceEqual("8BPS"u8)) throw new InvalidDataException("Not a Photoshop document.");
        var version = BinaryPrimitives.ReadUInt16BigEndian(header[4..6]);
        if (version is not (1 or 2)) throw new InvalidDataException($"Unsupported PSD version {version}.");
        return new(version == 2,
            BinaryPrimitives.ReadUInt16BigEndian(header[12..14]),
            checked((int)BinaryPrimitives.ReadUInt32BigEndian(header[18..22])),
            checked((int)BinaryPrimitives.ReadUInt32BigEndian(header[14..18])),
            BinaryPrimitives.ReadUInt16BigEndian(header[22..24]),
            BinaryPrimitives.ReadUInt16BigEndian(header[24..26]));
    }
}

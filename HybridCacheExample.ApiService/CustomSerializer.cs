using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.Caching.Hybrid;

namespace HybridCacheExample.ApiService;

public class CustomSerializer<T> : IHybridCacheSerializer<T>
{
    private static readonly XmlSerializer XmlSerializer = new XmlSerializer(typeof(T));

    public T Deserialize(ReadOnlySequence<byte> source)
    {
        try
        {
            using var stream = new MemoryStream();
            foreach (var segment in source)
            {
                stream.Write(segment.Span);
            }
            stream.Position = 0;

            using var xmlReader = XmlReader.Create(stream);
            return (T)XmlSerializer.Deserialize(xmlReader)!;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Deserialization failed.", ex);
        }
    }

    public void Serialize(T value, IBufferWriter<byte> target)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            using (var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { Encoding = Encoding.UTF8 }))
            {
                XmlSerializer.Serialize(writer, value);
            }

            var buffer = memoryStream.GetBuffer().AsSpan(0, (int)memoryStream.Length);
            target.Write(buffer);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Serialization failed.", ex);
        }
    }
}

class SerializerFactory : IHybridCacheSerializerFactory
{
    public bool TryCreateSerializer<T>([NotNullWhen(true)] out IHybridCacheSerializer<T>? serializer)
    {
        serializer = new CustomSerializer<T>();
        return true;
    }
}
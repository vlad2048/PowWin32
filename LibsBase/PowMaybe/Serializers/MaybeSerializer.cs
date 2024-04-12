using System.Text.Json;
using System.Text.Json.Serialization;

namespace PowMaybe.Serializers;

public sealed class MaybeSerializer<T> : JsonConverter<Maybe<T>>
{
	private sealed class Nfo
	{
		public bool HasVal { get; }
		public T? V { get; }
		public Nfo(bool hasVal, T? v)
		{
			HasVal = hasVal;
			V = v;
		}
	}
	
	public override Maybe<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		using var doc = JsonDocument.ParseValue(ref reader);
		var nfo = doc.Deserialize<Nfo>(options)!;
		return nfo.HasVal switch
		{
			false => May.None<T>(),
			true => May.Some(nfo.V)!
		};
	}

	public override void Write(Utf8JsonWriter writer, Maybe<T> value, JsonSerializerOptions options)
	{
		var hasValue = value.IsSome(out var val);
		var nfo = new Nfo(hasValue, val);
		JsonSerializer.Serialize(writer, nfo, options);
	}
}
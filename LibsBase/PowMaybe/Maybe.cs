namespace PowMaybe;

public abstract class Maybe<T> : IEquatable<Maybe<T>>
{
	public sealed class Some : Maybe<T>
	{
		public T V { get; }
		internal Some(T v)
		{
			V = v;
		}
		public override string ToString() => $"Some<{typeof(T).Name}>({V})";
	}

	public sealed class None : Maybe<T>
	{
		internal None()
		{
		}
		public override string ToString() => $"None<{typeof(T).Name}>()";
	}


	/// <summary>
	/// Allows writing:
	///		from subNode in parent.QueryNode(xPath)
	///		select subNode.InnerText;					// fun = subNode => subNode.InnerText
	///
	/// otherwise you can only 'select subNode', not 'select fun(subNode)'
	///
	/// in FP, this is called map()
	/// </summary>
	public Maybe<V> Select<V>(Func<T, V> fun) =>
		this switch
		{
			Some { V: var val } => May.Some(fun(val)),
			None => May.None<V>(),
			_ => throw new ArgumentException()
		};

	/// <summary>
	/// Allows writing:
	///		from subNode in parent.QueryNode(xPath)
	///		from attrValue in subNode.GetAttrFromNode(attrName)
	///		select attrValue;
	///
	/// otherwise only a single 'from' statement is allowed
	/// 
	/// in FP, this is called bind()
	/// </summary>
	public Maybe<V> SelectMany<U, V>(Func<T, Maybe<U>> mapper, Func<T, U, V> getResult) =>
		this switch
		{
			Some { V: var val } => mapper(val) switch
			{
				Maybe<U>.Some { V: var valFun } => May.Some(getResult(val, valFun)),
				Maybe<U>.None => May.None<V>(),
				_ => throw new ArgumentException()
			},
			None => May.None<V>(),
			_ => throw new ArgumentException()
		};
	
	public Maybe<T> Where(Func<T, bool> predicate) =>
		this switch
		{
			Some { V: var val } => predicate(val) switch
			{
				true => this,
				false => May.None<T>()
			},
			None => this,
			_ => throw new ArgumentException()
		};

	public bool Equals(Maybe<T>? other)
	{
		if (other == null) return false;
		if (other.IsNone() && this.IsNone()) return true;
		if (other.IsSome(out var otherVal) && this.IsSome(out var thisVal))
			return otherVal.Equals(thisVal);
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != GetType()) return false;
		return Equals((Maybe<T>)obj);
	}

	public override int GetHashCode() => this.IsSome(out var val) switch
	{
		true => val!.GetHashCode(),
		false => 0
	};
	public static bool operator ==(Maybe<T>? left, Maybe<T>? right) => Equals(left, right);
	public static bool operator !=(Maybe<T>? left, Maybe<T>? right) => !Equals(left, right);
}
using System;

namespace MMI_SP.PatternMatching
{
    public abstract class Option<T>
    {
        public bool is_some() => this is Some<T>;
        public bool is_none() => this is None<T>;

        public abstract Option<TResult> and_then<TResult>(Func<T, Option<TResult>> func);
        public abstract TResult match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone);
    }

    public sealed class Some<T> : Option<T>
    {
        public T Value { get; }
        public Some(T value) => Value = value;

        public override Option<TResult> and_then<TResult>(Func<T, Option<TResult>> func) => func(Value);
        public override TResult match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone) => onSome(Value);
    }

    public sealed class None<T> : Option<T>
    {
        public override Option<TResult> and_then<TResult>(Func<T, Option<TResult>> func) => new None<TResult>();
        public override TResult match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone) => onNone();
    }

    public static class Option
    {
        public static Option<T> from_nullable<T>(T value) where T : class
            => value != null ? new Some<T>(value) : (Option<T>)new None<T>();

        public static None<T> new_none<T>() => new None<T>();
    }
}
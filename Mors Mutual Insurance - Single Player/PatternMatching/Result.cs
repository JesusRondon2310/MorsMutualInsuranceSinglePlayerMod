using System;

namespace MMI_SP.PatternMatching
{
    public abstract class Result<T>
    {
        public bool is_ok() => this is Ok<T>;
        public bool is_err() => this is Err<T>;

        public abstract TResult match<TResult>(Func<T, TResult> onOk, Func<string, TResult> onErr);

        public Result<TResult> and_then<TResult>(Func<T, Result<TResult>> func)
        {
            return this.match<Result<TResult>>(
                onOk: func,
                onErr: error => new Err<TResult>(error)
            );
        }

        public T unwrap_or(T defaultValue)
        {
            return this.match<T>(
                onOk: value => value,
                onErr: _ => defaultValue
            );
        }
    }

    public sealed class Ok<T> : Result<T>
    {
        public T Value { get; }
        public Ok(T value) => Value = value;

        public override TResult match<TResult>(Func<T, TResult> onOk, Func<string, TResult> onErr) => onOk(Value);
    }

    public sealed class Err<T> : Result<T>
    {
        public string Message { get; }
        public Err(string message) => Message = message;

        public override TResult match<TResult>(Func<T, TResult> onOk, Func<string, TResult> onErr) => onErr(Message);
    }
}
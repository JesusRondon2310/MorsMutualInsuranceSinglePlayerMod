using System;

namespace MMI_SP.PatternMatching
{
    public abstract class Result<T>
    {
        // ==========================================
        // BLOQUE 1: Estado
        // ==========================================
        public bool is_ok()  => this is Ok<T>;
        public bool is_err() => this is Err<T>;

        // ==========================================
        // BLOQUE 2: Operaciones principales
        // ==========================================

        /// Ejecuta onOk si es Ok, onErr si es Err. Siempre devuelve TResult.
        public abstract TResult match<TResult>(Func<T, TResult> onOk, Func<string, TResult> onErr);

        /// Encadena operaciones que pueden fallar. Propaga Err sin ejecutar func.
        public Result<TResult> and_then<TResult>(Func<T, Result<TResult>> func)
            => match<Result<TResult>>(onOk: func, onErr: error => new Err<TResult>(error));

        /// Extrae el valor si es Ok, o devuelve defaultValue si es Err.
        public T unwrap_or(T defaultValue)
            => match<T>(onOk: value => value, onErr: _ => defaultValue);

        /// Extrae el valor si es Ok, o evalúa el factory si es Err (evita evaluar el default innecesariamente).
        public T unwrap_or_else(Func<string, T> factory)
            => match<T>(onOk: value => value, onErr: factory);

        /// Transforma el valor interno si es Ok, sin tocar el Err.
        public Result<TResult> map<TResult>(Func<T, TResult> transform)
            => match<Result<TResult>>(
                onOk:  value => new Ok<TResult>(transform(value)),
                onErr: error => new Err<TResult>(error));

        /// Ejecuta una acción si es Ok sin romper la cadena. Útil para logs.
        public Result<T> tap_ok(Action<T> action)
        {
            if (this is Ok<T> ok) action(ok.Value);
            return this;
        }

        /// Ejecuta una acción si es Err sin romper la cadena. Útil para logs.
        public Result<T> tap_err(Action<string> action)
        {
            if (this is Err<T> err) action(err.Message);
            return this;
        }

        /// Si es Err, intenta recuperarse con una alternativa.
        public Result<T> or_else(Func<string, Result<T>> fallback)
            => this is Err<T> err ? fallback(err.Message) : this;   // ✅ PATTERN MATCHING
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
        public Err(string message) => Message = message ?? string.Empty;

        public override TResult match<TResult>(Func<T, TResult> onOk, Func<string, TResult> onErr) => onErr(Message);
    }
}
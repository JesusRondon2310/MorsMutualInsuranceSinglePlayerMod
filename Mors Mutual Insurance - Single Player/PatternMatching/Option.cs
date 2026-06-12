using System;

namespace MMI_SP.PatternMatching
{
    public abstract class Option<T>
    {
        // ==========================================
        // BLOQUE 1: Estado
        // ==========================================
        public bool is_some() => this is Some<T>;
        public bool is_none() => this is None<T>;

        // ==========================================
        // BLOQUE 2: Operaciones principales
        // ==========================================

        /// Ejecuta onSome si hay valor, onNone si no hay. Siempre devuelve TResult.
        public abstract TResult match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone);

        /// Encadena operaciones sobre el valor. Si es None, propaga None sin ejecutar func.
        public abstract Option<TResult> and_then<TResult>(Func<T, Option<TResult>> func);

        /// Extrae el valor si es Some, o devuelve defaultValue si es None.
        public T unwrap_or(T defaultValue)
            => match<T>(onSome: value => value, onNone: () => defaultValue);

        /// Extrae el valor si es Some, o evalúa el factory si es None (evita evaluar el default innecesariamente).
        public T unwrap_or_else(Func<T> factory)
            => match<T>(onSome: value => value, onNone: factory);

        /// Transforma el valor interno si es Some, sin tocar None.
        public Option<TResult> map<TResult>(Func<T, TResult> transform)
            => match<Option<TResult>>(
                onSome: value => new Some<TResult>(transform(value)),
                onNone: () => None<TResult>.Instance);

        /// Filtra el valor con una condición. Si no la cumple, devuelve None.
        public Option<T> filter(Func<T, bool> predicate)
            => match<Option<T>>(
                onSome: value => predicate(value) ? this : (Option<T>)None<T>.Instance,
                onNone: () => None<T>.Instance);

        /// Ejecuta una acción si es Some sin romper la cadena. Útil para logs.
        public Option<T> tap(Action<T> action)
        {
            if (this is Some<T> some) action(some.Value);
            return this;
        }

        /// Si es None, devuelve una alternativa.
        public Option<T> or_else(Func<Option<T>> fallback)
            => is_some() ? this : fallback();

        // ==========================================
        // BLOQUE 3: Conversión a Result<T>
        // ==========================================

        /// Convierte Option<T> en Result<T>. Si es Some, devuelve Ok; si es None, devuelve Err con el mensaje personalizado.
        public Result<T> ok_or(string errorMessage)
        {
            if (this is Some<T> some)
                return new Ok<T>(some.Value);
            return new Err<T>(errorMessage);
        }
    }

    public sealed class Some<T> : Option<T>
    {
        public T Value { get; }
        public Some(T value) => Value = value;

        public override TResult match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone) => onSome(Value);
        public override Option<TResult> and_then<TResult>(Func<T, Option<TResult>> func) => func(Value);
    }

    public sealed class None<T> : Option<T>
    {
        // Singleton — evita crear una nueva instancia None en cada llamada
        public static readonly None<T> Instance = new None<T>();
        private None() { }

        public override TResult match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone) => onNone();
        public override Option<TResult> and_then<TResult>(Func<T, Option<TResult>> func) => None<TResult>.Instance;
    }

    public static class Option
    {
        /// Convierte un valor de referencia nullable en Some o None.
        public static Option<T> from_nullable<T>(T value) where T : class
            => value != null ? (Option<T>)new Some<T>(value) : None<T>.Instance;

        /// Convierte un valor de tipo valor nullable en Some o None.
        public static Option<T> from_nullable_value<T>(T? value) where T : struct
            => value.HasValue ? (Option<T>)new Some<T>(value.Value) : None<T>.Instance;

        /// Crea un None<T> sin escribir new None<T>() manualmente.
        public static None<T> new_none<T>() => None<T>.Instance;
    }
}
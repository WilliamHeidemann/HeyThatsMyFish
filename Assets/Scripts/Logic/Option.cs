namespace Logic
{
    public readonly struct Option<T>
    {
        public static Option<T> None => default;
        public static Option<T> Some(T value) => new(value);

        private readonly bool _isSome;
        private readonly T _value;

        private Option(T value)
        {
            _value = value;
            _isSome = _value is { };
        }

        public bool IsSome(out T value)
        {
            value = _value;
            return _isSome;
        }
    }
}
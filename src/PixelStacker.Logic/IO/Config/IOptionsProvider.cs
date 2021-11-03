﻿namespace PixelStacker.Logic.IO.Config
{
    public interface IOptionsProvider
    {
        Options Load();
        void Save(Options t);
    }

    public class MemoryOptionsProvider : IOptionsProvider
    {
        public Options Value;
        public Options Load()
        {
            Value ??= new Options(this);
            return Value;
        }

        public void Save(Options t)
        {
            Value = t;
        }
    }

    public class DefaultOptionsProvider : IOptionsProvider
    {
        public Options Value;
        public Options Load()
        {
            Value ??= new Options(this);
            return Value;
        }

        public void Save(Options t)
        {
            Value = t;
        }
    }
}

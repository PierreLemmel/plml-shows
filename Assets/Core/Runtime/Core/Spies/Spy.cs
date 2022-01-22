using System;

namespace Plml
{
    public static class Spy
    {
        public static ISpy CreateNew() => new SpyImpl();
    }
}
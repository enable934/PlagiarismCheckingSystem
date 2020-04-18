using System;
using System.Collections.Generic;

namespace PlagiarismCheckingSystem.Util
{
    public static class IOCContainer
    {
        private static readonly Dictionary<Type, Type> _registeredObjects = new Dictionary<Type, Type>();

        public static dynamic Resolve<TKey>()
        {
            return Activator.CreateInstance(_registeredObjects[typeof(TKey)]);
        }

        public static void Register<TKey, TConcrete>() where TConcrete : TKey
        {
            _registeredObjects[typeof(TKey)] = typeof(TConcrete);
        }
    }
}

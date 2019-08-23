namespace LoadEncrypt
{
    using System;
    using System.Reflection;
    using System.Security.Permissions;

    using EnsoulSharp.SDK;
    using Memorykatarina.Properties;
    using EnsoulSharp;

    internal class Program
    {
        private static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        private static void OnGameLoad()
        {
            try
            {
                var a = Assembly.Load(Resources.Memorykatarina);
                var myType = a.GetType("Memorykatarina.Program");// namespace + class name
                var methon = myType.GetMethod("Main", BindingFlags.Public | BindingFlags.Static); // methon

                if (methon != null)
                {
                    methon.Invoke(null, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        
    }
}
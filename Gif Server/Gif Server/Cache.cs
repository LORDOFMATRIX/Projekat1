using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Gif_Server
{
    internal class Cache
    {
        private static readonly ReaderWriterLockSlim dictionaryLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        public static readonly Dictionary<string, byte[]> cache = new Dictionary<string, byte[]>(4);

        public static byte[] ReadFromCache(string key)
        {
            byte[] def = null;
            dictionaryLock.EnterReadLock();
            try
            {
                if (cache.TryGetValue(key, out byte[] value))
                {
                    return value;
                }
                else
                {
                    return def;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                throw;
            }
            finally
            {
                dictionaryLock.ExitReadLock();
            }
        }
        public static bool WriteToCache(string key, byte[] value)
        {
            dictionaryLock.EnterWriteLock();
            try
            {
                if (cache.Count < 3)
                {
                   cache[key] = value;
                   return true;

                }
                else
                {
                    RemoveFromCache();
                    cache[key] = value;
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                return false;
            }
            finally
            {
                dictionaryLock.ExitWriteLock();
            }
        }
        
        public static void RemoveFromCache()
        {
            Random rand = new Random();
            List<string> key = Enumerable.ToList(cache.Keys);
            int size = cache.Count;
            string keyForRemoval= key[rand.Next(size)];
            dictionaryLock.EnterWriteLock();
            try
            {
                cache.Remove(keyForRemoval);
                Console.WriteLine();
                Console.WriteLine($"Iz keša je izbačena slika: {keyForRemoval}");
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
            finally
            {
                dictionaryLock.ExitWriteLock();
            }
        }
    }
}

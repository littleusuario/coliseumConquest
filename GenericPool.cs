using System;
using System.Collections.Generic;

namespace MyGame
{
    public delegate void InitializeDelegate<T>(T obj, Vector2 position, Vector2 velocity, string image, bool horizontalCheck);

    public class GenericPool<T> where T : class
    {
        private List<T> objectsInUse = new List<T>();
        private List<T> objectsAvailable = new List<T>();
        private InitializeDelegate<T> initializer;

        public GenericPool(InitializeDelegate<T> data)
        {
            this.initializer = data;
        }

        public T GetObject(Vector2 position, Vector2 velocity, string image, bool horizontalCheck)
        {
            T newObject = null;

            if (objectsAvailable.Count > 0)
            {
                newObject = objectsAvailable[0];
                objectsAvailable.RemoveAt(0);
                initializer(newObject, position, velocity, image, horizontalCheck);
            }
            else
            {
                newObject = (T)Activator.CreateInstance(typeof(T), (int)position.x, (int)position.y, velocity, image, horizontalCheck);
                if (newObject is Bullet bullet)
                {
                    bullet.OnDestroy += (b) => RecycleObject(b as T);
                }
            }

            objectsInUse.Add(newObject);
            //Debug();
            return newObject;
        }

        public void RecycleObject(T obj)
        {
            if (objectsInUse.Contains(obj))
            {
                objectsInUse.Remove(obj);
                objectsAvailable.Add(obj);
            }
        }

        private void Debug()
        {
            Console.WriteLine("Objetos disponibles: " + objectsAvailable.Count);
            Console.WriteLine("Objetos en uso: " + objectsInUse.Count);
        }
    }
}
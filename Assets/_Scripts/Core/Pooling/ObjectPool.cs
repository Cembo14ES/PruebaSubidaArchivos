using UnityEngine;
using System.Collections.Generic;

namespace Huellas26.Core.Pooling
{
    /// <summary>
    /// Object Pool genérico reutilizable.
    /// Best Practice VR: Evitar Instantiate/Destroy en runtime (causa stutters y drops de FPS).
    /// Usa este pool para proyectiles, efectos de partículas, etc.
    /// Referencia: Unity Performance Optimization + VR Best Practices
    /// </summary>
    /// <typeparam name="T">Tipo del componente a poolear</typeparam>
    public class ObjectPool<T> where T : Component
    {
        private readonly T _prefab;
        private readonly Queue<T> _pool;
        private readonly Transform _parent;
        private readonly int _initialSize;

        public ObjectPool(T prefab, int initialSize = 10, Transform parent = null)
        {
            _prefab = prefab;
            _initialSize = initialSize;
            _parent = parent;
            _pool = new Queue<T>(initialSize);

            // Pre-warm pool
            for (int i = 0; i < initialSize; i++)
            {
                T instance = Object.Instantiate(_prefab, _parent);
                instance.gameObject.SetActive(false);
                _pool.Enqueue(instance);
            }
        }

        /// <summary>
        /// Obtiene un objeto del pool (o crea uno nuevo si está vacío).
        /// </summary>
        public T Get()
        {
            T instance;
            
            if (_pool.Count > 0)
            {
                instance = _pool.Dequeue();
            }
            else
            {
                Debug.LogWarning($"[ObjectPool<{typeof(T).Name}>] Pool exhausted, creating new instance.");
                instance = Object.Instantiate(_prefab, _parent);
            }

            instance.gameObject.SetActive(true);
            return instance;
        }

        /// <summary>
        /// Devuelve el objeto al pool.
        /// </summary>
        public void Return(T instance)
        {
            if (instance == null) return;
            
            instance.gameObject.SetActive(false);
            _pool.Enqueue(instance);
        }

        /// <summary>
        /// Obtiene objeto en una posición y rotación específica.
        /// </summary>
        public T Get(Vector3 position, Quaternion rotation)
        {
            T instance = Get();
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            return instance;
        }

        /// <summary>
        /// Vacía el pool destruyendo todos los objetos.
        /// </summary>
        public void Clear()
        {
            while (_pool.Count > 0)
            {
                T instance = _pool.Dequeue();
                if (instance != null)
                {
                    Object.Destroy(instance.gameObject);
                }
            }
        }

        public int AvailableCount => _pool.Count;
    }
}

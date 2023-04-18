using System;
using System.Collections.Concurrent;
using UnityEngine;
namespace GameClient
{
    public class ExecuteOnMainthread : MonoBehaviour
    {
        public static readonly ConcurrentQueue<Action> RunOnMainThread = new ConcurrentQueue<Action>();

        void Update()
        {
            if (!RunOnMainThread.IsEmpty)
            {
                while (RunOnMainThread.TryDequeue(out var action))
                {
                    action?.Invoke();
                }
            }
        }
    }
}
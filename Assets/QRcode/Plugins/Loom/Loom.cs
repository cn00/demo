using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;

//Loom handles threading
public class Loom : MonoBehaviour
{
    private static Loom _current;

    public static Loom Current
    {
        get
        {
            if (_current == null && Application.isPlaying)
            {

                var g = GameObject.Find("Loom");
                if (g == null)
                {
                    g = new GameObject("Loom");
                }

                _current = g.GetComponent<Loom>() ?? g.AddComponent<Loom>();
            }

            return _current;
        }
    }

    void Awake()
    {
        if (_current != null && _current != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _current = this;
        }
    }

    private List<Action> _actions = new List<Action>();
    public class DelayedQueueItem
    {
		public int runTimes = 1;
        public float time;
        public Action action;
        public string name;
    }
    private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();

    public static void DelayedOnMainThread(Action action, float time, string name, int runTimes = 1)
    {
        lock (Current._delayed)
        {
            if (Current._delayed.Any(d => d.name == name))
                return;
            QueueOnMainThread(action, time, runTimes);
        }
    }

    public static void QueueOnMainThread(Action action, float time, int runTimes = 1)
    {
        if (time != 0)
        {
            lock (Current._delayed)
            {
                Current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action, runTimes = runTimes });
            }
        }
        else
        {
            lock (Current._actions)
            {
                Current._actions.Add(action);
            }
        }
    }

    public static void QueueOnMainThread(Action action)
    {
        lock (Current._actions)
        {
            Current._actions.Add(action);
        }
    }

    public static void RunAsync(Action a)
    {
        var t = new Thread(RunAction);
        t.Priority = System.Threading.ThreadPriority.Normal;
        t.Start(a);
    }

    private static void RunAction(object action)
    {
        ((Action)action)();
    }


    List<Action> toBeRun = new List<Action>();
    List<DelayedQueueItem> toBeDelayed = new List<DelayedQueueItem>();

    void Update()
    {
        //Process the non-delayed actions
        lock (_actions)
        {
            toBeRun.AddRange(_actions);
            _actions.Clear();
        }
        foreach (var a in toBeRun)
        {
            try
            {
                a();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Queued Exception: " + e.ToString());
            }
        }
        toBeRun.Clear();
        lock (_delayed)
        {
            toBeDelayed.AddRange(_delayed);
        }
        foreach (var delayed in toBeDelayed.Where(d => d.time <= Time.time))
        {
            lock (_delayed)
            {
                _delayed.Remove(delayed);
            }
            try
            {
                delayed.action();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Delayed Exception:" + e.ToString());
            }
        }
        toBeDelayed.Clear();

    }
}
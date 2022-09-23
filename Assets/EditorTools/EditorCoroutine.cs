#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Reflection;

using UnityEngine;
using UnityEditor;
public static class EditorCoroutine
{
    const string Tag = "EditorCoroutine";
    private class Coroutine : IEnumerator
    {
        private Stack<IEnumerator> executionStack;

        public Coroutine(IEnumerator iterator)
        {
            this.executionStack = new Stack<IEnumerator>();
            this.executionStack.Push(iterator);
        }

        public bool MoveNext()
        {
            IEnumerator i = this.executionStack.Peek();

            if (i.MoveNext())
            {
                object result = i.Current;
                if (result != null && result is IEnumerator)
                {
                    this.executionStack.Push((IEnumerator)result);
                }

                return true;
            }
            else
            {
                if (this.executionStack.Count > 1)
                {
                    this.executionStack.Pop();
                    return true;
                }
            }

            return false;
        }

        public void Reset()
        {
            throw new System.NotSupportedException("This Operation Is Not Supported.");
        }

        public object Current
        {
            get { return this.executionStack.Peek().Current; }
        }

        public bool Find(IEnumerator iterator)
        {
            return this.executionStack.Contains(iterator);
        }
    }

    private static List<Coroutine> editorCoroutineList;
    private static List<IEnumerator> buffer;

    public static IEnumerator StartEditorCoroutineTest()
    {
        var www = new WWW("https://www.youtube.com/results?search_query=The+Magic+Flute%2C+Queen+of+the+Night");
        yield return www;
        Debug.Log("StartEditorCoroutineTest " + www.text);

        yield return null;
    }

    [MenuItem("Tools/==ClearDeveloperConsole")]
    public static void CallStartEditorCoroutineTest()
    {
        EditorCoroutine.Run(StartEditorCoroutineTest());
    }

    public static IEnumerator Run(IEnumerator iterator)
    {
        if (editorCoroutineList == null)
        {
            editorCoroutineList = new List<Coroutine>();
        }
        if (buffer == null)
        {
            buffer = new List<IEnumerator>();
        }
        if (editorCoroutineList.Count == 0)
        {
            EditorApplication.update += Update;
        }

        // add iterator to buffer first
        buffer.Add(iterator);

        return iterator;
    }

    private static bool Find(IEnumerator iterator)
    {
        // If this iterator is already added
        // Then ignore it this time
        foreach (Coroutine editorCoroutine in editorCoroutineList)
        {
            if (editorCoroutine.Find(iterator))
            {
                return true;
            }
        }

        return false;
    }

    private static void Update()
    {
        // EditorCoroutine execution may append new iterators to buffer
        // Therefore we should run EditorCoroutine first
        editorCoroutineList.RemoveAll(coroutine => { return coroutine.MoveNext() == false;});

        // If we have iterators in buffer
        if (buffer.Count > 0)
        {
            foreach (IEnumerator iterator in buffer)
            {
                // If this iterators not exists
                if (!Find(iterator))
                {
                    // Added this as new EditorCoroutine
                    editorCoroutineList.Add(new Coroutine(iterator));
                }
            }

            // Clear buffer
            buffer.Clear();
        }

        // If we have no running EditorCoroutine
        // Stop calling update anymore
        if (editorCoroutineList.Count == 0)
        {
            EditorApplication.update -= Update;
        }
    }
}
#endif
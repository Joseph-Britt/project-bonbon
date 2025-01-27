using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoStartDialogue : MonoBehaviour {
    public float waitToStart = 0.0f;

    public EventObject campEvent;

    // testing
    public EventSequencer evSeq;

    void Start() {
        StartCoroutine(Wait());
    }

    IEnumerator Wait() {
        yield return new WaitForSeconds(waitToStart);
        evSeq.AddEvent(campEvent);
        evSeq.RunNextEvent();
    }
}

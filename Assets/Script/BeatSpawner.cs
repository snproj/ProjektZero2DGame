using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BeatSpawner : MonoBehaviour
{
    private BeatBar currBar;
    private int beatPerBar = 4;
    private decimal bpm = 80M;
    private decimal bps;
    private decimal secondsPerBeat;
    private decimal barTimer;
    private decimal graceTimer;
    private decimal secondsPerBar;
    private decimal timeResolution = 0.1M;

    private class BeatBar
    {
        public class Beat
        {
            public decimal beatNum;
            public int beatType;
            public Beat((decimal, int) tup)
            {
                beatNum = tup.Item1;
                beatType = tup.Item2;
            }
        }
        public Queue<Beat> beats = new Queue<Beat>();
        public int repeat;
        public BeatBar((decimal, int)[] beatTups, int rep = 0)
        {
            foreach ((decimal, int) tup in beatTups)
            {
                beats.Enqueue(new Beat(tup));
            }
            repeat = rep;
        }
    }

    //private Queue<BeatBar> beatScript = new Queue<BeatBar>(new[]
    //{
    //    new BeatBar(new[] {(2M,0)}),
    //    new BeatBar(new[] {(3M,0), (3.5M,1)}),
    //    new BeatBar(new[] {(0M,0), (1M,1), (2M,0), (3M,1)}),
    //    new BeatBar(new[] {(0M,0), (1M,1), (2M,0), (3M,1)}),
    //    new BeatBar(new[] {(0M,0), (1M,1), (2M,0), (3M,1)}),
    //});

    private Queue<BeatBar> beatScript = new Queue<BeatBar>();

    //private Queue<BeatBar> beatScriptParser(string script)
    //{
    //    Dictionary<string, Queue<BeatBar>> dict;

    //    Queue<BeatBar> resQ = new Queue<BeatBar>();
    //    string[] phrases = script.Split('/').ToArray();
    //    foreach (string phrase in phrases)
    //    {
    //        if (char.IsLetter(phrase[0])){
    //            string key = "";
    //            for (int ptr = 0; char.IsLetter(phrase[ptr]); ptr++)
    //            {
    //                key += phrase[ptr];
    //            }
    //        }
    //        string[] lines = script.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
    //        foreach (string line in lines)
    //        {
    //            if (char.IsLetter(line[0])){
    //                continue;
    //            }
    //            Queue<(decimal, int)> tupQ = new Queue<(decimal, int)>();
    //            string[] pairs = line.Split(';');
    //            foreach (string pair in pairs)
    //            {
    //                string[] vals = pair.Split(',');
    //                tupQ.Enqueue((System.Convert.ToDecimal(vals[0]), System.Convert.ToInt32(vals[1])));
    //            }
    //            resQ.Enqueue(new BeatBar(tupQ.ToArray()));
    //        }
    //    }

    //    return resQ;
    //}

    private Queue<BeatBar> beatScriptParser(string script)
    {
        Queue<BeatBar> resQ = new Queue<BeatBar>();
        string[] lines = script.Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        foreach (string line in lines)
        {
            if (char.IsLetter(line[0]))
            {
                continue;
            }
            Queue<(decimal, int)> tupQ = new Queue<(decimal, int)>();
            string[] pairs = line.Split(';').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            foreach (string pair in pairs)
            {
                string[] vals = pair.Split(',');
                tupQ.Enqueue((System.Convert.ToDecimal(vals[0].Trim()), System.Convert.ToInt32(vals[1].Trim())));
            }
            resQ.Enqueue(new BeatBar(tupQ.ToArray()));
        }

        return resQ;
    }

    private string bamtakbam = string.Concat(Enumerable.Repeat(@"

0,0;    1.5,1;  3,0", 4));

    private string test = @"

2,0;    2.5,0


1,1;    1.5,1;  1.75,1;
3,0;    3.5,0";

    // Start is called before the first frame update
    void Start()
    {
        beatScript = beatScriptParser(bamtakbam);
        print(beatScript.Count);

        currBar = beatScript.Dequeue();
        barTimer = 0;
        graceTimer = 0M;
        bps = bpm / 60;
        secondsPerBar = beatPerBar / bps;
        secondsPerBeat = 1 / bps;
    }

    // Update is called once per frame
    void Update()
    {
        if (isBarOverAndUpdateTimer())
        {
            currBar = beatScript.Dequeue();
        }

        currBarAction();

    }

    bool isBarOverAndUpdateTimer()
    {
        barTimer += (decimal)Time.deltaTime;
        if (barTimer > secondsPerBar)
        {
            print("BAR OVER at " + barTimer);
            barTimer = 0;
            return true;
        }
        return false;
    }


    static decimal DIFFUPPERBOUND_CONSTANT = 0.005M;
    void currBarAction()
    {
        if (graceTimer > 0)
        {
            graceTimer -= (decimal)Time.deltaTime;
        }
        if (currBar.beats.Count > 0)
        {
            decimal targetBeatNum = currBar.beats.Peek().beatNum;
            decimal diff = barTimer - secondsPerBeat * targetBeatNum;
            if (diff >= 0 && diff < DIFFUPPERBOUND_CONSTANT) // correct beat within bar has been reached
            {
                if (graceTimer <= 0)
                {
                    print("SPAWN WITH TYPE: " + currBar.beats.Peek().beatType + " AT " + barTimer);
                    graceTimer = timeResolution;
                    currBar.beats.Dequeue();
                }
            }
        }

    }
}
using UnityEngine;
using System.Collections.Generic;
using System;

public class TimerManager : MonoBehaviour
{
    protected class TimerInfo
    {
        public int uuid;
        public long tick;
        public bool stop;
        public bool delete;
        public Action Func;
        public long tickend;
        public bool loop;
        public TimerInfo(Action func, float second)
        {
            this.Func = func;
            tickend = (int)(second / timerInterval);
            delete = false;
        }
    }
    private float interval = 0;
    static float timerInterval = 0.01f;

    private static List<TimerInfo> objects = new List<TimerInfo>();
    public int timerNum = 0;

    static int uuid = 0;
    public float Interval
    {
        get { return interval; }
        set { interval = value; }
    }
    public static TimerManager Instance;

    // Use this for initialization
    void Start()
    {
        Instance = this;
        //LoggerHelper.Info("----- TimerManager:Start -----");
        StartTimer(timerInterval);
    }
    void OnDestroy()
    {
        objects.Clear();
    }

    /// <summary>
    /// 启动计时器
    /// </summary>
    /// <param name="interval"></param>
    public void StartTimer(float value)
    {
        interval = value;
        InvokeRepeating("Run", 0, interval);
    }

    /// <summary>
    /// 停止计时器
    /// </summary>
    public void StopTimer()
    {
        CancelInvoke("Run");
    }

    //<summary>
    //添加计时器事件
    //</summary>
    public static int CreateTimerEvent(Action Func, float second, bool loop = false)
    {
        if (Func == null)
        {
            //Debug.LogError("CreateTimerEvent param is " + Func.ToString());
            return 0;
        }
        ++uuid;
        TimerInfo info = new TimerInfo(Func, second);

        info.uuid = uuid;
        info.loop = loop;
        if (!objects.Contains(info))
        {
            objects.Add(info);
            if (Instance != null)
                Instance.timerNum = objects.Count;
        }
        //Debug.Log(Time.realtimeSinceStartup + " " + info.tick + " " + info.tickend);
        return info.uuid;
    }

    /// <summary>
    /// 修改倒计时的结束时间
    /// </summary>
    /// <param name="id"></param>
    /// <param name="second"></param>
    public static void ModifyTimerEvent(int id, int second)
    {
        foreach (var o in objects)
        {
            if (o.uuid == id)
            {
                o.tickend = (int)(second / timerInterval);
                break;
            }
        }
    }

    /// <summary>
    /// 获得运行的时间
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static int GetRunTimerEvent(int id)
    {
        foreach (var o in objects)
        {
            if (o.uuid == id)
                return (int)(o.tick * timerInterval);
        }
        return 0;
    }

    /// <summary>
    /// 删除计时器事件
    /// </summary>
    /// <param name="timerid"></param>
    public static void RemoveTimerEvent(int id)
    {
        foreach (var o in objects)
        {
            if (o.uuid == id)
                o.delete = true;
        }
    }

    /// <summary>
    /// 停止计时器事件
    /// </summary>
    /// <param name="timerid"></param>
    public static void StopTimerEvent(int id)
    {
        foreach (var o in objects)
        {
            if (o.uuid == id)
                o.stop = true;
        }
    }

    public static void StopAll()
    {
        //Debug.LogWarning("TimerManager:StopAll");
        foreach (var o in objects)
        {
            o.stop = true;
        }
    }

    /// <summary>
    /// 继续计时器事件
    /// </summary>
    /// <param name="timerid"></param>
    public static void ResumeTimerEvent(int id)
    {
        foreach (var o in objects)
        {
            if (o.uuid == id)
                o.stop = false;
        }
    }

    /// <summary>
    /// 计时器运行
    /// </summary>
    void Run()
    {
        if (objects.Count == 0) return;
        for (int i = 0, imax = objects.Count; i < imax; i++)
        {
            TimerInfo o = objects[i];
            if (o.delete || o.stop) { continue; }
            o.tick++;
            if (o.tick > o.tickend)
            {
                o.Func();
                //Debug.Log(Time.realtimeSinceStartup + " " + o.tick + " " + o.tickend);
                if (!o.loop)
                    o.delete = true;
                else
                    o.tick = 0;
            }
        }
        /////////////////////////清除标记为删除或强制停止的事件///////////////////////////
        for (int i = objects.Count - 1; i >= 0; i--)
        {
            if (objects[i].delete)// || objects[i].stop)
            {
                objects[i].Func = null;
                objects.Remove(objects[i]);
                if (Instance != null)
                    Instance.timerNum = objects.Count;
                break;
            }
        }
    }
}


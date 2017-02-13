using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class VoidPlayScript : MonoBehaviour {


    ActMaker actMaker = new ActMaker();
    protected void StartAct()
    {
        StartCoroutine(actMaker.Execute());
    }

    IEnumerator Execute(IEnumerator execute)
    {
        bool isLast = !execute.MoveNext();
        if (!isLast)
        {
            yield return execute.Current;
        }
    }


    // Actionを登録する

    protected void Play(Action func) { actMaker.AddFunc(() => { func(); return false; }); }
    protected void Play<A>(Action<A> func, A a) { actMaker.AddFunc(() => { func(a); return false; }); }
    protected void Play<A, B>(Action<A, B> func, A a, B b) { actMaker.AddFunc(() => { func(a, b); return false; }); }
    protected void Play<A, B, C>(Action<A, B, C> func, A a, B b, C c) { actMaker.AddFunc(() => { func(a, b, c); return false; }); }
    protected void Play<A, B, C, D>(Action<A, B, C, D> func, A a, B b, C c, D d) { actMaker.AddFunc(() => { func(a, b, c, d); return false; }); }
    
    protected Func<bool> BOOL(Func<bool> func) { return () => func(); }
    protected Func<bool> BOOL<A>(Func<A, bool> func, A a) { return () => func(a); }
    protected Func<bool> BOOL<A, B>(Func<A, B, bool> func, A a, B b) { return () => func(a, b); }
    protected Func<bool> BOOL<A, B, C>(Func<A, B, C, bool> func, A a, B b, C c) { return () => func(a, b, c); }
    protected Func<bool> BOOL<A, B, C, D>(Func<A, B, C, D, bool> func, A a, B b, C c, D d) { return () => func(a, b, c, d); }

    protected Func<bool> NOT(Func<bool> func) { return () => !func(); }
    protected Func<bool> NOT<A>(Func<A, bool> func, A a) { return () => !func(a); }
    protected Func<bool> NOT<A, B>(Func<A, B, bool> func, A a, B b) { return () => !func(a, b); }
    protected Func<bool> NOT<A, B, C>(Func<A, B, C, bool> func, A a, B b, C c) { return () => !func(a, b, c); }
    protected Func<bool> NOT<A, B, C, D>(Func<A, B, C, D, bool> func, A a, B b, C c, D d) { return () => !func(a, b, c, d); }

    protected Func<bool> TRUE { get { return () => true; } }
    protected Func<bool> FALSE { get { return () => false; } }

    protected Func<bool> AND(params Func<bool>[] funcs)
    {
        return () =>
        {
            bool ret = true;
            foreach (var a in funcs) ret &= a();
            return ret;
        };
    }
    protected Func<bool> OR(params Func<bool>[] funcs)
    {
        return () =>
        {
            bool ret = false;
            foreach (var a in funcs) ret |= a();
            return ret;
        };
    }


    // 構文の定義
    protected class Mode : IDisposable
    {
        Action end;
        public Mode(Action start, Action end) { start(); this.end = end; }
        public Mode() { end = () => { }; }
        public void Dispose() { end(); }
    }
    protected Mode PARALLEL
    {
        get
        {
            return new Mode(
                () => { actMaker.AddBox(new ActMaker.FunctionBoxParallel()); },
                () => { actMaker.RemoveBox(); });
        }
    }
    protected Mode SERIAL
    {
        get
        {
            return new Mode(
                () => { actMaker.AddBox(new ActMaker.FunctionBoxSerial()); },
                () => { actMaker.RemoveBox(); });
        }
    }
    protected Mode IF<A, B, C, D>(Func<A, B, C, D, bool> func, A a, B b, C c, D d) { return IF(() => func(a, b, c, d)); }
    protected Mode IF<A, B, C>(Func<A, B, C, bool> func, A a, B b, C c) { return IF(() => func(a, b, c)); }
    protected Mode IF<A, B>(Func<A, B, bool> func, A a, B b) { return IF(() => func(a, b)); }
    protected Mode IF<A>(Func<A, bool> func, A a) { return IF(() => func(a)); }
    protected Mode IF(Func<bool> func)
    {
        return new Mode(
                () => { actMaker.AddBox(new ActMaker.FunctionBoxIf(func)); },
                () => { actMaker.RemoveBox(); });
    }
    protected Mode ELSE
    {
        get
        {
            return new Mode(
                () => { actMaker.AddBox_ElseIf(new ActMaker.FunctionBoxIf(()=> true)); },
                () => { actMaker.RemoveBox_ElseIf(); });
        }
    }
    protected Mode ELSEIF<A, B, C, D>(Func<A, B, C, D, bool> func, A a, B b, C c, D d) { return ELSEIF(() => func(a, b, c, d)); }
    protected Mode ELSEIF<A, B, C>(Func<A, B, C, bool> func, A a, B b, C c) { return ELSEIF(() => func(a, b, c)); }
    protected Mode ELSEIF<A, B>(Func<A, B, bool> func, A a, B b) { return ELSEIF(() => func(a, b)); }
    protected Mode ELSEIF<A>(Func<A, bool> func, A a) { return ELSEIF(() => func(a)); }
    protected Mode ELSEIF(Func<bool> func)
    {
        return new Mode(
                () => { actMaker.AddBox_ElseIf(new ActMaker.FunctionBoxIf(func)); },
                () => { actMaker.RemoveBox_ElseIf(); });
    }
    protected Mode WHILE<A, B, C, D>(Func<A, B, C, D, bool> func, A a, B b, C c, D d) { return WHILE(() => func(a, b, c, d)); }
    protected Mode WHILE<A, B, C>(Func<A, B, C, bool> func, A a, B b, C c) { return WHILE(() => func(a, b, c)); }
    protected Mode WHILE<A, B>(Func<A, B, bool> func, A a, B b) { return WHILE(() => func(a, b)); }
    protected Mode WHILE<A>(Func<A, bool> func, A a) { return WHILE(() => func(a)); }
    protected Mode WHILE(Func<bool> func)
    {
        return new Mode(
                () => { actMaker.AddBox(new ActMaker.FunctionBoxWhile(func)); },
                () => { actMaker.RemoveBox(); }
            );
    }
    protected Mode WHILETIME(float waitTime)
    {
        return new Mode(
                () => { actMaker.AddBox(new ActMaker.FunctionBoxWhileTimer(waitTime)); },
                () => { actMaker.RemoveBox(); }
            );
    }
    protected void BREAK()
    {
        actMaker.AddBreak();
    }



    // 構文の処理


    class ActMaker
    {
        public abstract class FunctionBox
        {
            bool breakFlag = false;
            protected List<FunctionBox> childs = new List<FunctionBox>();
            public abstract IEnumerator Execute();

            public void AddFunc(Func<bool> func)
            {
                childs.Add(new FunctionBoxSingle(func));
            }
            public void AddBox(FunctionBox box)
            {
                childs.Add(box);
            }
            public void AddBreak()
            {
                childs.Add(new FunctionBoxBreak());
            }
            public FunctionBox PeekChild() { return childs[childs.Count - 1]; }

            protected void SetBreak() { breakFlag = true; }
            public bool IsBreaked() { return breakFlag; }
            public void ResetBreak() { breakFlag = false; }
        }

        public class FunctionBoxSingle : FunctionBox
        {
            Func<bool> func;
            public FunctionBoxSingle(Func<bool> func)
            {
                this.func = func;
            }
            public override IEnumerator Execute()
            {
                while (true)
                {
                    bool isEnd = !func();
                    if (isEnd) break;
                    yield return null;
                }
            }
        }

        public class FunctionBoxSerial : FunctionBox
        {
            public override IEnumerator Execute()
            {
                bool breaked = false;
                for (int i = 0; i < childs.Count; i++)
                {
                    var iterator = childs[i].Execute();

                    while (true)
                    {
                        bool isFunctionLast = !iterator.MoveNext();
                        if (childs[i].IsBreaked()) { breaked = true; childs[i].ResetBreak(); break; }
                        if (isFunctionLast) break;
                        yield return null;
                    }
                    if (breaked) break;
                }

                if (breaked) this.SetBreak();
            }
        }

        public class FunctionBoxParallel : FunctionBox
        {
            public override IEnumerator Execute()
            {
                List<IEnumerator> iterators = new List<IEnumerator>();
                for (int i = 0; i < childs.Count; i++) iterators.Add(childs[i].Execute());

                bool breaked = false;
                while (true)
                {
                    bool endAll = true;

                    for (int i = 0; i < childs.Count; i++)
                    {
                        bool isEnd = !iterators[i].MoveNext();
                        if (childs[i].IsBreaked()) { childs[i].ResetBreak(); breaked = true; break; }
                        endAll &= isEnd;
                    }

                    if (breaked) break;
                    if (endAll) break;
                    yield return null;
                }

                if (breaked) this.SetBreak();
            }
        }

        public class FunctionBoxIf : FunctionBoxSerial
        {
            Func<bool> judgeFunc;
            FunctionBoxIf elseBox;
            public FunctionBoxIf(Func<bool> judgeFunc)
            {
                this.judgeFunc = judgeFunc;
            }
            public override IEnumerator Execute()
            {
                if (judgeFunc())
                {
                    return base.Execute();
                }
                if (elseBox == null) elseBox = new FunctionBoxIf(() => true);
                return elseBox.Execute();
            }
            public void SetElseBox(FunctionBoxIf box)
            {
                if (elseBox == null) elseBox = box;
                else elseBox.SetElseBox(box);
            }
        }

        public class FunctionBoxWhile : FunctionBoxSerial
        {
            Func<bool> judgeFunc;
            Action startFunc;
            Action endFunc;
            public FunctionBoxWhile(Func<bool> judgeFunc)
            {
                this.judgeFunc = judgeFunc;
            }
            public void SetJudgeFunc(Func<bool> nextJudgeFunc)
            {
                judgeFunc = nextJudgeFunc;
            }
            public void SetStartFunc(Action startFunc) { this.startFunc = startFunc; }
            public void SetEndFunc(Action endFunc) { this.startFunc = endFunc; }
            public override IEnumerator Execute()
            {
                bool doBreak = false;
                if (startFunc != null) startFunc();
                while (true)
                {
                    if (!judgeFunc()) break;

                    for (int i = 0; i < childs.Count; i++)
                    {
                        var iterator = childs[i].Execute();

                        while (true)
                        {
                            bool isFunctionLast = !iterator.MoveNext();
                            if (childs[i].IsBreaked()) { doBreak = true; childs[i].ResetBreak(); break; }
                            if (isFunctionLast) break;
                            yield return null;
                        }
                        if (doBreak) break;
                    }
                    if (doBreak) break;

                    yield return null;
                }
                if (endFunc != null) endFunc();
            }
        }

        public class FunctionBoxWhileTimer : FunctionBoxWhile
        {
            float startTime;
            float waitTime;
            public FunctionBoxWhileTimer(float waitTime) : base(() => true)
            {
                this.SetJudgeFunc(new Func<bool>(JudgeFunc));
                this.SetStartFunc(StartFunc);
                this.waitTime = waitTime;
            }
            bool JudgeFunc()
            {
                return (Time.time - startTime) < waitTime;
            }
            void StartFunc()
            {
                this.startTime = Time.time;
            }
        }
        
        public class FunctionBoxBreak : FunctionBox
        {
            public override IEnumerator Execute()
            {
                this.SetBreak();
                yield break;
            }
        }

        List<FunctionBox> boxes = new List<FunctionBox>();
        FunctionBox headBox;

        public ActMaker()
        {
            headBox = new FunctionBoxSerial();
            boxes.Add(headBox);
        }
        public void AddBox(FunctionBox box) { Peek(boxes).AddBox(box); boxes.Add(box); }
        public void RemoveBox() { Pop(boxes); }

        public void AddBox_ElseIf(FunctionBoxIf box)
        {
            var prev = Peek(boxes).PeekChild();
            if (prev is ActMaker.FunctionBoxIf)
            {
                ((FunctionBoxIf)prev).SetElseBox(box);
                boxes.Add(box);
            }
            else { Debug.LogError("Script 'ELSE' can't add"); }
        }
        public void RemoveBox_ElseIf() { RemoveBox(); }

        public IEnumerator Execute() { return headBox.Execute(); }
        public void AddFunc(Func<bool> func) { Peek(boxes).AddFunc(func); }
        public void AddBreak() { Peek(boxes).AddBreak(); }

        T Peek<T>(List<T> list) { return list[list.Count - 1]; }
        void Pop<T>(List<T> list) { list.RemoveAt(list.Count - 1); }

    }


    
}

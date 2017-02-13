using UnityEngine;
using System.Collections;

public class VoidPlayScript_Sample : VoidPlayScript
{

    void Start()
    {

        /*
        using (WHILE(Timer(5)))
        {
            using (WHILE(TestFuncKey, KeyCode.A)) Play(NullFunc);
            using (WHILE(TestFuncKey, KeyCode.S)) Play(NullFunc);

            using (IF(OR(BOOL(TestFuncFrame, 200), NOT(TestFuncKey, KeyCode.T))))
            {
                using (WHILE(TestFuncKey, KeyCode.D)) Play(NullFunc);
            }
            using (ELSE)
            {
                using (WHILE(TestFuncKey, KeyCode.V)) Play(NullFunc);
            }
        }
        */

        using (WHILETIME(10))
        {
            /*
            using (PARALLEL)
            {
                using (WHILE(Timer(0.5f))) Play(Print, "0.5 no hou");
                using (WHILE(Timer(1.5f))) Play(Print, "1.5 no hou");
            }*/
            //using (WHILE(TestFuncKey, KeyCode.A)) Play(Print, "test yadeee");
            //using (WHILE(BOOL(TestFuncKey, KeyCode.A))) Play(Print, "test yadeee");

            using (PARALLEL)
            {
                using (WHILETIME(0.5f)) Play(Print, "test yadeee");
                using (WHILETIME(1.5f)) Play(Print, "test suggeeee");
            }
            for (int i = 0; i < 10; i++) Play(Print, "AAA");
        }
        StartAct();
    }

    void Print(string s)
    {
        Debug.Log(Time.frameCount + " " + s);
    }

    void NullFunc()
    {

    }

    bool TestFuncFrame(int num)
    {
        Debug.Log(Time.frameCount + " TestFunc0 " + num);
        return Time.frameCount > num;
    }

    bool TestFuncKey(KeyCode key)
    {
        Debug.Log(Time.frameCount + " TestFunc1 " + key);
        return !Input.GetKey(key);
    }
    bool TestFuncKey2(KeyCode key, KeyCode key2)
    {
        Debug.Log(Time.frameCount + " TestFunc2 " + key + " " + key2);
        return !Input.GetKey(key) || !Input.GetKey(key2);
    }
}

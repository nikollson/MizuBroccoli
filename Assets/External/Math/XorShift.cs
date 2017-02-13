using UnityEngine;
using System.Collections;

namespace Library
{
    public class Xorshift
    {
        // 内部メモリ
        private uint x;
        private uint y;
        private uint z;
        private uint w;


        public Xorshift(ulong seed)
        {
            SetSeed(seed);
        }
        public Xorshift(int seed)
        {
            SetSeed((ulong)seed);
        }

        /// <summary>
        /// シード値を設定
        /// </summary>
        /// <param name="seed">シード値</param>
        public void SetSeed(ulong seed)
        {
            // x,y,z,wがすべて0にならないようにする
            x = 521288629u;
            y = (uint)(seed >> 32) & 0xFFFFFFFF;
            z = (uint)(seed & 0xFFFFFFFF);
            w = x ^ z;
        }

        /// <summary>
        /// 乱数を取得
        /// </summary>
        /// <returns>乱数</returns>
        public uint Next()
        {
            uint t = x ^ (x << 11);
            x = y;
            y = z;
            z = w;
            w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));
            return w;
        }

        /// <summary>
        /// min 以上 max 以下の乱数を生成
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public int Range(int min, int max)
        {
            int a = (int)Next();
            a *= a < 0 ? -1 : 1;
            a = (a % (max - min + 1) + min);
            return a;
        }
    }
}
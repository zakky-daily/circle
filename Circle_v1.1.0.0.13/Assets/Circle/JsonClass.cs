using System;
using UnityEngine;


//online Exclusion
[Serializable] public class onlineprojects
{
    public string[] SKL;
    public bool[] R;
}


//プロジェクト情報
[Serializable] public class projectsinformation
{
    public int D_NC;
    public int[] l;
    public ProjectsList[] L;
}

[Serializable] public class ProjectsList
{
    public string N;
    public string D;
    public string U;
    public bool O;
    public string SK;
}


//プロジェクト
[Serializable] public class projectdata
{
    public int[] s;
    public int Ns;
    public Scene[] S;
    public int[] i;
    public Images[] I;
    public int[] si;
    public Images[] SI;
    public Information Ifm;
}

[Serializable] public class Scene
{
    public string N;
    public Vector2 P;
    public float S;
    public int si;
    public int[] o;
    public Objects[] O;
    public int[] m;
    public Message[] M;
}

[Serializable] public class Objects
{
    public string N;
    public string T;
    public Vector2 P;
    public float A;
    public Vector2 S;
    public ColorInt C;
    public int[] i;
    public int Ni;
    public Option O;
    public MessageGroup G;
}

[Serializable] public class MessageGroup
{
    public int[] r;
    public ExecuteMessage[] R;
    public int[] s;
    public ExecuteMessage[] S;
}

[Serializable] public class ExecuteMessage
{
    public int i;
    public int[] p;
    public ProgrammingIcon[] P;
}

[Serializable] public class ProgrammingIcon
{
    public string T;
    public Option O;
}

[Serializable] public class Images
{
    public int W;
    public int H;
}

[Serializable] public class Message
{
    public string N;
}

[Serializable] public class Information
{
    public string N;
    public string D;
    public string U;
}



[Serializable] public class ColorInt
{
    public int R;
    public int G;
    public int B;
    public int A;
}

[Serializable] public class Option
{
    public string[] S;
    public int[] I;
    public float[] F;
    public bool[] B;
}